IF DB_ID('ZealandBooking') IS NULL
    CREATE DATABASE ZealandBooking;
GO
USE ZealandBooking;
GO

-- Drops tables
IF OBJECT_ID('dbo.Booking', 'U') IS NOT NULL DROP TABLE dbo.Booking;
IF OBJECT_ID('dbo.SmartBoard', 'U') IS NOT NULL DROP TABLE dbo.SmartBoard;
IF OBJECT_ID('dbo.Room', 'U') IS NOT NULL DROP TABLE dbo.Room;
IF OBJECT_ID('dbo.RoomType', 'U') IS NOT NULL DROP TABLE dbo.RoomType;
IF OBJECT_ID('dbo.Building', 'U') IS NOT NULL DROP TABLE dbo.Building;
IF OBJECT_ID('dbo.UserDepartmentMapping', 'U') IS NOT NULL DROP TABLE dbo.UserDepartmentMapping;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
IF OBJECT_ID('dbo.UserType', 'U') IS NOT NULL DROP TABLE dbo.UserType;
IF OBJECT_ID('dbo.Department', 'U') IS NOT NULL DROP TABLE dbo.Department;
GO

-- Creates tables
CREATE TABLE dbo.Department (
    DepartmentID INT IDENTITY(1,1) NOT NULL,
    Address      VARCHAR(255)      NOT NULL,
    Name         VARCHAR(100)      NOT NULL,
    CONSTRAINT PK_Department PRIMARY KEY CLUSTERED (DepartmentID),
    CONSTRAINT UQ_Department_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.UserType (
    UserTypeID INT IDENTITY(1,1) NOT NULL,
    UserType   VARCHAR(50)       NOT NULL,
    CONSTRAINT PK_UserType PRIMARY KEY CLUSTERED (UserTypeID),
    CONSTRAINT UQ_UserType_UserType UNIQUE (UserType)
);
GO

CREATE TABLE dbo.Building (
    BuildingID   INT IDENTITY(1,1) NOT NULL,
    DepartmentID INT               NOT NULL,
    Name         VARCHAR(100)      NOT NULL,
    CONSTRAINT PK_Building PRIMARY KEY CLUSTERED (BuildingID),
    CONSTRAINT UQ_Building_Department_Name UNIQUE (DepartmentID, Name),
    CONSTRAINT FK_Building_Department
        FOREIGN KEY (DepartmentID)
        REFERENCES dbo.Department(DepartmentID)
        ON DELETE CASCADE
);
GO

CREATE TABLE dbo.RoomType (
    RoomTypeID INT IDENTITY(1,1) NOT NULL,
    RoomType   VARCHAR(50)       NOT NULL,
    Capacity   INT               NOT NULL,
    CONSTRAINT PK_RoomType PRIMARY KEY CLUSTERED (RoomTypeID),
    CONSTRAINT UQ_RoomType UNIQUE (RoomType),
    CONSTRAINT CK_Capacity CHECK (Capacity > 0)
);
GO

CREATE TABLE dbo.Room (
    RoomID     INT IDENTITY(1,1) NOT NULL,
    BuildingID INT               NOT NULL,
    Name       VARCHAR(100)      NOT NULL,
    Level      VARCHAR(3)        NOT NULL,
    RoomTypeID INT               NOT NULL,
    CONSTRAINT PK_Room PRIMARY KEY CLUSTERED (RoomID),

    CONSTRAINT FK_Room_Building
        FOREIGN KEY (BuildingID)
        REFERENCES dbo.Building(BuildingID)
        ON DELETE CASCADE,
    CONSTRAINT FK_Room_RoomType
        FOREIGN KEY (RoomTypeID)
        REFERENCES dbo.RoomType(RoomTypeID)
);
GO

CREATE TABLE dbo.SmartBoard (
    SmartBoardID INT IDENTITY(1,1) NOT NULL,
    RoomID       INT               NOT NULL,
    CONSTRAINT PK_SmartBoard PRIMARY KEY CLUSTERED (SmartBoardID),
    CONSTRAINT UQ_SmartBoard_Room UNIQUE (RoomID),
    CONSTRAINT FK_SmartBoard_Room
        FOREIGN KEY (RoomID)
        REFERENCES dbo.Room(RoomID)
        ON DELETE CASCADE
);
GO
-- make email unique across ids     
CREATE TABLE dbo.[User] (
    UserID     INT IDENTITY(1,1) NOT NULL,
    Name       VARCHAR(100)      NOT NULL,
    Phone      VARCHAR(20)       NULL,
    Email      VARCHAR(100)      NOT NULL,
    Password   VARCHAR(255)      NOT NULL,
    UserTypeID INT               NOT NULL,
    LoggedinSessioonID uniqueidentifier NULL,
    CONSTRAINT PK_User PRIMARY KEY CLUSTERED (UserID),
    CONSTRAINT UQ_User_Email UNIQUE (Email),
    CONSTRAINT FK_User_UserType
        FOREIGN KEY (UserTypeID)
        REFERENCES dbo.UserType(UserTypeID)
);
GO

CREATE TABLE dbo.UserDepartmentMapping (
    UDMappingID  INT IDENTITY(1,1) NOT NULL,
    UserID       INT               NOT NULL,
    DepartmentID INT               NOT NULL,
    CONSTRAINT PK_UDM PRIMARY KEY CLUSTERED (UDMappingID),
    CONSTRAINT UQ_UDM UNIQUE (UserID, DepartmentID),
    CONSTRAINT FK_UDM_User
        FOREIGN KEY (UserID)
        REFERENCES dbo.[User](UserID)
        ON DELETE CASCADE,
    CONSTRAINT FK_UDM_Department
        FOREIGN KEY (DepartmentID)
        REFERENCES dbo.Department(DepartmentID)
        ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Booking (
    BookingID    INT IDENTITY(1,1) NOT NULL,
    RoomID       INT               NOT NULL,
    UserID       INT               NOT NULL,
    StartTime    TIME              NOT NULL,
    [Date]       DATE              NOT NULL,
    SmartBoardID INT               NULL,
    CONSTRAINT PK_Booking PRIMARY KEY CLUSTERED (BookingID),
    CONSTRAINT CK_Booking_Date CHECK ([Date] >= CAST(GETDATE() AS DATE)),
    CONSTRAINT CK_Booking_Time CHECK (StartTime >= '08:00' AND StartTime <= '14:00'),
    CONSTRAINT FK_Booking_Room
        FOREIGN KEY (RoomID)
        REFERENCES dbo.Room(RoomID)
        ON DELETE CASCADE,
    CONSTRAINT FK_Booking_User
        FOREIGN KEY (UserID)
        REFERENCES dbo.[User](UserID)
        ON DELETE CASCADE,
    CONSTRAINT FK_Booking_Smartboard
        FOREIGN KEY (SmartBoardID)
        REFERENCES dbo.SmartBoard(SmartBoardID),
);
GO
