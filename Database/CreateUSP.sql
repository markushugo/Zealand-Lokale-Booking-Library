USE ZealandBooking;
GO


-- UserStoredProcedures

IF OBJECT_ID('dbo.usp_GetFilteredBookings', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetFilteredBookings;
GO

IF OBJECT_ID('dbo.GetFilterOptionsForUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetFilterOptionsForUser;
GO

IF OBJECT_ID('dbo.usp_GetAvailableBookingSlots', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetAvailableBookingSlots;
GO

IF OBJECT_ID('dbo.usp_GetBookingsByUserID', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetBookingsByUserID;
GO
IF OBJECT_ID('dbo.usp_DeleteBooking', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_DeleteBooking;
GO


CREATE PROCEDURE dbo.usp_GetFilteredBookings
    @UserID        INT,                              
    @Date          DATE,                             
    @DepartmentIds dbo.IntList     READONLY,   
    @BuildingIds   dbo.IntList     READONLY,      
    @RoomIds       dbo.IntList     READONLY,       
    @RoomTypeIds   dbo.IntList     READONLY,       
    @Levels        dbo.LevelList   READONLY,      
    @Times         dbo.TimeList    READONLY    
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
        AND (
            NOT EXISTS (SELECT 1 FROM @DepartmentIds)
            OR d.DepartmentID IN (SELECT Id FROM @DepartmentIds)
        )
        AND (
            NOT EXISTS (SELECT 1 FROM @BuildingIds)
            OR bu.BuildingID IN (SELECT Id FROM @BuildingIds)
        )
        AND (
            NOT EXISTS (SELECT 1 FROM @RoomIds)
            OR r.RoomID IN (SELECT Id FROM @RoomIds)
        )
        AND (
            NOT EXISTS (SELECT 1 FROM @RoomTypeIds)
            OR rt.RoomTypeID IN (SELECT Id FROM @RoomTypeIds)
        )
        AND (
            NOT EXISTS (SELECT 1 FROM @Levels)
            OR r.Level IN (SELECT Level FROM @Levels)
        )
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

----------------------------------------------------------------------------

CREATE PROCEDURE dbo.GetFilterOptionsForUser
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    ----------------------------------------------------------------------
    -- Departments the user has access to
    ----------------------------------------------------------------------
    SELECT 
        d.DepartmentID AS [Value],
        d.Name         AS [Text]
    FROM dbo.Department d
    INNER JOIN dbo.UserDepartmentMapping udm 
        ON udm.DepartmentID = d.DepartmentID
    WHERE udm.UserID = @UserID
    ORDER BY d.Name;

    ----------------------------------------------------------------------
    -- Buildings in the departments
    ----------------------------------------------------------------------
    SELECT 
        b.BuildingID AS [Value],
        b.Name       AS [Text]
    FROM dbo.Building b
    INNER JOIN dbo.UserDepartmentMapping udm
        ON udm.DepartmentID = b.DepartmentID
    WHERE udm.UserID = @UserID
    ORDER BY b.Name;
    ----------------------------------------------------------------------
    -- Room types
    ----------------------------------------------------------------------
    SELECT 
        rt.RoomTypeID AS [Value],
        rt.RoomType   AS [Text]
    FROM dbo.RoomType rt
    ORDER BY rt.RoomType;
END;
GO
---------------------------------------------------------------------------

CREATE PROCEDURE dbo.usp_GetAvailableBookingSlots
    @UserID        INT,                
    @Date          DATE,            
    @DepartmentIds dbo.IntList   READONLY,
    @BuildingIds   dbo.IntList   READONLY,
    @RoomIds       dbo.IntList   READONLY,
    @RoomTypeIds   dbo.IntList   READONLY,
    @Levels        dbo.LevelList READONLY,
    @Times         dbo.TimeList  READONLY
AS
BEGIN
    SET NOCOUNT ON;

    IF @Date IS NULL
    BEGIN
        RAISERROR('Date is required.', 16, 1);
        RETURN;
    END

    ----------------------------------------------------------------------
    -- Temporary table for available rooms
    ----------------------------------------------------------------------
    ;WITH FilteredRooms AS (
        SELECT 
            r.RoomID,
            r.Name AS RoomName,
            r.Level,
            rt.RoomTypeID,
            rt.RoomType,
            rt.Capacity,
            bu.BuildingID,
            bu.Name AS BuildingName,
            d.DepartmentID,
            d.Name AS DepartmentName
        FROM dbo.Room r
        INNER JOIN dbo.RoomType rt   ON r.RoomTypeID = rt.RoomTypeID
        INNER JOIN dbo.Building bu   ON r.BuildingID = bu.BuildingID
        INNER JOIN dbo.Department d  ON bu.DepartmentID = d.DepartmentID
        INNER JOIN dbo.UserDepartmentMapping udm
            ON udm.DepartmentID = d.DepartmentID
           AND udm.UserID       = @UserID
        WHERE
            (
                NOT EXISTS (SELECT 1 FROM @DepartmentIds)
                OR d.DepartmentID IN (SELECT Id FROM @DepartmentIds)
            )
            AND (
                NOT EXISTS (SELECT 1 FROM @BuildingIds)
                OR bu.BuildingID IN (SELECT Id FROM @BuildingIds)
            )
            AND (
                NOT EXISTS (SELECT 1 FROM @RoomIds)
                OR r.RoomID IN (SELECT Id FROM @RoomIds)
            )
            AND (
                NOT EXISTS (SELECT 1 FROM @RoomTypeIds)
                OR rt.RoomTypeID IN (SELECT Id FROM @RoomTypeIds)
            )
            AND (
                NOT EXISTS (SELECT 1 FROM @Levels)
                OR r.Level IN (SELECT Level FROM @Levels)
            )
    ),
    ----------------------------------------------------------------------
    -- Timeslots (default 08–14)
    ----------------------------------------------------------------------
    DefaultTimes AS (
        SELECT CAST('08:00' AS TIME) AS StartTime
        UNION ALL SELECT CAST('10:00' AS TIME)
        UNION ALL SELECT CAST('12:00' AS TIME)
        UNION ALL SELECT CAST('14:00' AS TIME)
    ),
    TimeSlots AS (
        SELECT StartTime
        FROM DefaultTimes
        WHERE NOT EXISTS (SELECT 1 FROM @Times)
        UNION
        SELECT StartTime
        FROM @Times
    )
    ----------------------------------------------------------------------
    -- Rooms x times 
    ----------------------------------------------------------------------
    SELECT
        CAST(NULL AS INT)          AS BookingID,
        @Date                      AS [Date],
        ts.StartTime               AS StartTime,
        CAST(NULL AS INT)          AS UserID,
        CAST(NULL AS VARCHAR(100)) AS UserName,
        fr.RoomID,
        fr.RoomName,
        fr.Level,
        fr.RoomTypeID,
        fr.RoomType,
        fr.Capacity,
        fr.BuildingID,
        fr.BuildingName,
        fr.DepartmentID,
        fr.DepartmentName,
        CAST(NULL AS INT)          AS SmartBoardID
    FROM FilteredRooms fr
    CROSS JOIN TimeSlots ts
    LEFT JOIN dbo.Booking b
        ON  b.RoomID    = fr.RoomID
        AND b.[Date]    = @Date
        AND b.StartTime = ts.StartTime
    WHERE
        b.BookingID IS NULL
    ORDER BY
        [Date],
        ts.StartTime,
        fr.DepartmentName,
        fr.BuildingName,
        fr.Level,
        fr.RoomName;
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

---------------------------------------------------------------------
CREATE PROCEDURE dbo.usp_DeleteBooking
    @BookingID INT,
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    -----------------------------------------------------
    -- 1. GET BOOKINGDATA
    -----------------------------------------------------
    DECLARE @BookingDate DATE;
    DECLARE @BookingOwner INT;

    SELECT 
        @BookingDate = [Date],
        @BookingOwner = UserID
    FROM dbo.Booking
    WHERE BookingID = @BookingID;

    IF @BookingDate IS NULL
    BEGIN
        RAISERROR('Bookingen findes ikke.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------
    -- 2. FIND USER ROLE
    -----------------------------------------------------
    DECLARE @UserTypeID INT;

    SELECT @UserTypeID = UserTypeID
    FROM dbo.[User]
    WHERE UserID = @UserID;

    IF @UserTypeID IS NULL
    BEGIN
        RAISERROR('Bruger findes ikke.', 16, 1);
        RETURN;
    END

    -----------------------------------------------------
    -- 3. STUDENT (UserTypeID = 1)
    -----------------------------------------------------
    IF @UserTypeID = 1
    BEGIN
        -- Studerende må kun slette egne bookings
        IF @BookingOwner <> @UserID
        BEGIN
            RAISERROR('Studerende må kun slette deres egne bookinger.', 16, 1);
            RETURN;
        END

        DELETE FROM dbo.Booking WHERE BookingID = @BookingID;
        RETURN;
    END

    -----------------------------------------------------
    -- 4. TEACHER (UserTypeID = 2)
    -----------------------------------------------------
    IF @UserTypeID = 2
    BEGIN
        -- TEACHER CAN DELETE EVERYONES BOOKINGS BUT NEEDS TO FOLLOW 3-DAY RULE
        IF DATEDIFF(DAY, CONVERT(date, GETDATE()), @BookingDate) < 3
        BEGIN
            RAISERROR('Undervisere kan kun slette bookinger hvis der er mindst 3 dage til.', 16, 1);
            RETURN;
        END

        DELETE FROM dbo.Booking WHERE BookingID = @BookingID;
        RETURN;
    END

    -----------------------------------------------------
    -- 5. OTHER ROLES (Admin)
    -----------------------------------------------------
    RAISERROR('Denne bruger har ikke lov til at slette bookinger.', 16, 1);
END;
GO
GO


--------------------------------------------------------------
CREATE PROCEDURE dbo.usp_GetBookingsByUserID
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @UserID IS NULL
    BEGIN
        RAISERROR('UserID is required.', 16, 1);
        RETURN;
    END

    SELECT
        b.BookingID,
        b.[Date], 
        b.StartTime,
        u.UserID,
        u.Name AS UserName,
        r.RoomID,
        r.Name AS RoomName,
        r.Level,
        rt.RoomTypeID,
        rt.RoomType,
        rt.Capacity,
        bu.BuildingID,
        bu.Name AS BuildingName,
        d.DepartmentID,
        d.Name AS DepartmentName,
        b.SmartBoardID
    FROM dbo.Booking b
    INNER JOIN dbo.[User] u          ON b.UserID = u.UserID
    INNER JOIN dbo.Room r            ON b.RoomID = r.RoomID
    INNER JOIN dbo.RoomType rt       ON r.RoomTypeID = rt.RoomTypeID
    INNER JOIN dbo.Building bu       ON r.BuildingID = bu.BuildingID
    INNER JOIN dbo.Department d      ON bu.DepartmentID = d.DepartmentID
    LEFT  JOIN dbo.SmartBoard sb     ON b.SmartBoardID = sb.SmartBoardID
    WHERE
        b.UserID = @UserID
    ORDER BY
        b.[Date],
        b.StartTime,
        r.Name;
END;
GO
GO

IF OBJECT_ID('dbo.usp_LoginUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_LoginUser;
GO

CREATE PROCEDURE dbo.usp_LoginUser
    @Email      NVARCHAR(255),
    @Password   NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    --------------------------------------------------------
    -- 1. Input validation
    --------------------------------------------------------
    IF @Email IS NULL OR LTRIM(RTRIM(@Email)) = ''
    BEGIN
        RAISERROR('Email is required.', 16, 1);
        RETURN;
    END

    IF @Password IS NULL OR LTRIM(RTRIM(@Password)) = ''
    BEGIN
        RAISERROR('Password is required.', 16, 1);
        RETURN;
    END


    --------------------------------------------------------
    -- 2. Validate user exists AND password matches
    --------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM dbo.[User]
        WHERE Email = @Email
          AND [Password] = @Password
    )
    BEGIN
        -- Login successful
        -- Then Create guid
        -- wrtie guid to db
        -- write guid to cookie(in service)
        -- return guid
        DECLARE @SessionID uniqueidentifier = NEWID();
        SELECT CONVERT(CHAR(255), @SessionID) AS 'char';
        UPDATE [User] set LoggedinSessioonID=@SessionID WHERE Email=@Email
        SELECT
            1 AS IsAuthenticated,
            @SessionID AS SessionID;
        RETURN;
    END
    
    --------------------------------------------------------
    -- 3. Login failed
    --------------------------------------------------------
    SELECT CAST(0 AS BIT) AS IsAuthenticated,
           NULL AS UserID,
           NULL AS Name,
           NULL AS Email;
END;
GO
