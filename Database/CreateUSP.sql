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

CREATE PROCEDURE dbo.GetFilterOptionsForUser
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    ----------------------------------------------------------------------
    -- 1) Departments this user has access to
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
    -- 2) Buildings in those departments
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
    -- 3) Room types – user can select ANY room type
    ----------------------------------------------------------------------
    SELECT 
        rt.RoomTypeID AS [Value],
        rt.RoomType   AS [Text]
    FROM dbo.RoomType rt
    ORDER BY rt.RoomType;
END;
GO

-----------


CREATE PROCEDURE dbo.usp_GetAvailableBookingSlots
    @UserID        INT,                -- UserID (mandatory)
    @Date          DATE,               -- Date (mandatory)
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
    -- 1) Rum brugeren har adgang til via UserDepartmentMapping + filtre
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
            -- Department filter
            (
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
    ),
    ----------------------------------------------------------------------
    -- 2) Timeslots: default 08–14 hvis @Times er tom
    ----------------------------------------------------------------------
    DefaultTimes AS (
        SELECT CAST('08:00' AS TIME) AS StartTime
        UNION ALL SELECT CAST('09:00' AS TIME)
        UNION ALL SELECT CAST('10:00' AS TIME)
        UNION ALL SELECT CAST('11:00' AS TIME)
        UNION ALL SELECT CAST('12:00' AS TIME)
        UNION ALL SELECT CAST('13:00' AS TIME)
        UNION ALL SELECT CAST('14:00' AS TIME)
    ),
    TimeSlots AS (
        -- Hvis @Times er tom, brug default tider
        SELECT StartTime
        FROM DefaultTimes
        WHERE NOT EXISTS (SELECT 1 FROM @Times)

        UNION

        -- Ellers brug dem der er givet
        SELECT StartTime
        FROM @Times
    )

    ----------------------------------------------------------------------
    -- 3) Rum x tider LEFT JOIN Booking -> kun rækker uden booking
    ----------------------------------------------------------------------
    SELECT
        -- Disse felter mapper direkte til din Booking DTO
        CAST(NULL AS INT)          AS BookingID,     -- altid ledig -> null
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
        b.BookingID IS NULL  -- KUN ledige tider
    ORDER BY
        [Date],
        ts.StartTime,
        fr.DepartmentName,
        fr.BuildingName,
        fr.Level,
        fr.RoomName;
END;
GO