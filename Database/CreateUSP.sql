USE ZealandBooking;
GO


-- UserStoredProcedures


IF OBJECT_ID('dbo.usp_GetFilteredBookings', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetFilteredBookings;
GO

CREATE PROCEDURE dbo.usp_GetFilteredBookings
    @UserID        INT,                              -- UserID (mandatory)
    @Date          DATE,                             -- Date (mandatory)
    @DepartmentIds dbo.IntList     READONLY,         -- list of DepartmentID (PK)
    @BuildingIds   dbo.IntList     READONLY,         -- list of BuildingID (PK)
    @RoomIds       dbo.IntList     READONLY,         -- list of RoomID (PK)
    @RoomTypeIds   dbo.IntList     READONLY,         -- list of RoomTypeID (PK)
    @Levels        dbo.LevelList   READONLY,         -- list of Level values (e.g. '1', '2')
    @Times         dbo.TimeList    READONLY          -- list of StartTime values (time)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Date IS NULL
    BEGIN
        RAISERROR('Date is required.', 16, 1);
        RETURN;
    END
    SELECT
        b.BookingID,
        b.[Date],
        b.StartTime,
        u.UserID,
        u.Name            AS UserName,
        r.RoomID,
        r.Name            AS RoomName,
        r.Level,
        rt.RoomTypeID,
        rt.RoomType,
        rt.Capacity,
        bu.BuildingID,
        bu.Name           AS BuildingName,
        d.DepartmentID,
        d.Name            AS DepartmentName,
        b.SmartBoardID
    FROM dbo.Booking   AS b
    INNER JOIN dbo.Room                AS r   ON b.RoomID = r.RoomID
    INNER JOIN dbo.Building            AS bu  ON r.BuildingID = bu.BuildingID
    INNER JOIN dbo.Department          AS d   ON bu.DepartmentID = d.DepartmentID
    INNER JOIN dbo.RoomType            AS rt  ON r.RoomTypeID = rt.RoomTypeID
    INNER JOIN dbo.[User]              AS u   ON b.UserID = u.UserID
    INNER JOIN dbo.UserDepartmentMapping AS udm
        ON udm.DepartmentID = d.DepartmentID
       AND udm.UserID       = @UserID
    LEFT  JOIN dbo.SmartBoard          AS sb  ON b.SmartBoardID = sb.SmartBoardID
    WHERE
        b.[Date] = @Date

        -- Department filter
        AND (
            NOT EXISTS (SELECT 1 FROM @DepartmentIds)
            OR d.DepartmentID IN (SELECT Id FROM @DepartmentIds)
        )
        -- Building filter
        AND (
            NOT EXISTS (SELECT 1 FROM @BuildingIds)
            OR bu.BuildingID IN (SELECT Id FROM @BuildingIds)
        )

        -- Room filter
        AND (
            NOT EXISTS (SELECT 1 FROM @RoomIds)
            OR r.RoomID IN (SELECT Id FROM @RoomIds)
        )

        -- RoomType filter
        AND (
            NOT EXISTS (SELECT 1 FROM @RoomTypeIds)
            OR rt.RoomTypeID IN (SELECT Id FROM @RoomTypeIds)
        )

        -- Level filter
        AND (
            NOT EXISTS (SELECT 1 FROM @Levels)
            OR r.Level IN (SELECT Level FROM @Levels)
        )

        -- StartTime filter
        AND (
            NOT EXISTS (SELECT 1 FROM @Times)
            OR b.StartTime IN (SELECT StartTime FROM @Times)
        )
    ORDER BY
        b.[Date],
        b.StartTime,
        d.Name,
        bu.Name,
        r.Level,
        r.Name;
END;
GO




IF OBJECT_ID('dbo.usp_CreateBooking', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CreateBooking;
GO

CREATE PROCEDURE dbo.usp_CreateBooking
    @UserID       INT,
    @RoomID       INT,
    @Date         DATE,
    @StartTime    TIME,
    @SmartBoardID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ------------------------------------------------------------------------
    -- 1. Input validation
    ------------------------------------------------------------------------
    IF @UserID IS NULL OR @RoomID IS NULL
    BEGIN
        RAISERROR('UserID and RoomID are required.', 16, 1);
        RETURN;
    END

    IF @Date IS NULL
    BEGIN
        RAISERROR('Date is required.', 16, 1);
        RETURN;
    END

    IF @Date < CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('Date cannot be in the past.', 16, 1);
        RETURN;
    END

    IF @StartTime NOT BETWEEN '08:00' AND '14:00'
    BEGIN
        RAISERROR('StartTime must be between 08:00 and 14:00.', 16, 1);
        RETURN;
    END

    ------------------------------------------------------------------------
    -- 2. Validate Room exists
    ------------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Room WHERE RoomID = @RoomID)
    BEGIN
        RAISERROR('Room does not exist.', 16, 1);
        RETURN;
    END

    ------------------------------------------------------------------------
    -- 3. Validate SmartBoard belongs to the room (if provided)
    ------------------------------------------------------------------------
    IF @SmartBoardID IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1 
            FROM dbo.SmartBoard 
            WHERE SmartBoardID = @SmartBoardID 
              AND RoomID = @RoomID
        )
        BEGIN
            RAISERROR('SmartBoard does not belong to the selected room.', 16, 1);
            RETURN;
        END
    END

    ------------------------------------------------------------------------
    -- 4. Validate user access to the department of the room
    ------------------------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Room r
        INNER JOIN dbo.Building b  ON r.BuildingID = b.BuildingID
        INNER JOIN dbo.Department d ON b.DepartmentID = d.DepartmentID
        INNER JOIN dbo.UserDepartmentMapping udm 
            ON udm.DepartmentID = d.DepartmentID
           AND udm.UserID       = @UserID
        WHERE r.RoomID = @RoomID
    )
    BEGIN
        RAISERROR('User does NOT have access to the department of the selected room.', 16, 1);
        RETURN;
    END

    ------------------------------------------------------------------------
    -- 5. Check for booking conflicts
    ------------------------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM dbo.Booking
        WHERE RoomID    = @RoomID
          AND [Date]    = @Date
          AND StartTime = @StartTime
    )
    BEGIN
        RAISERROR('The room is already booked at this time.', 16, 1);
        RETURN;
    END

    ------------------------------------------------------------------------
    -- 6. Insert booking
    ------------------------------------------------------------------------
    INSERT INTO dbo.Booking (RoomID, UserID, StartTime, [Date], SmartBoardID)
    VALUES (@RoomID, @UserID, @StartTime, @Date, @SmartBoardID);

    ------------------------------------------------------------------------
    -- 7. Return booking info
    ------------------------------------------------------------------------
    SELECT 
        b.BookingID,
        b.RoomID,
        r.Name AS RoomName,
        b.UserID,
        u.Name AS UserName,
        b.[Date],
        b.StartTime,
        b.SmartBoardID
    FROM dbo.Booking b
    INNER JOIN dbo.Room r ON b.RoomID = r.RoomID
    INNER JOIN dbo.[User] u ON b.UserID = u.UserID
    WHERE b.BookingID = SCOPE_IDENTITY();
END;
GO