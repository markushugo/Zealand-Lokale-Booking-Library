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
