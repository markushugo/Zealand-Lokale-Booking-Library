USE ZealandBooking;
GO

/* ===========================
   Department
   =========================== */

DECLARE @DeptRoskildeID INT;

INSERT INTO dbo.Department (Address, Name)
VALUES ('Maglegårdsvej 2, 4000 Roskilde', 'Roskilde');

SET @DeptRoskildeID = SCOPE_IDENTITY();


/* ===========================
   UserTypes
   =========================== */

INSERT INTO dbo.UserType (UserType)
VALUES ('Student'),
       ('Teacher'),
       ('Admin');

DECLARE @UT_StudentID INT  = (SELECT UserTypeID FROM dbo.UserType WHERE UserType = 'Student');
DECLARE @UT_TeacherID INT  = (SELECT UserTypeID FROM dbo.UserType WHERE UserType = 'Teacher');
DECLARE @UT_AdminID   INT  = (SELECT UserTypeID FROM dbo.UserType WHERE UserType = 'Admin');


/* ===========================
   Buildings (A and D in Roskilde)
   =========================== */

DECLARE @BuildingAID INT;
DECLARE @BuildingDID INT;

INSERT INTO dbo.Building (DepartmentID, Name)
VALUES (@DeptRoskildeID, 'A');
SET @BuildingAID = SCOPE_IDENTITY();

INSERT INTO dbo.Building (DepartmentID, Name)
VALUES (@DeptRoskildeID, 'D');
SET @BuildingDID = SCOPE_IDENTITY();


/* ===========================
   RoomTypes
   =========================== */

INSERT INTO dbo.RoomType (RoomType, Capacity)
VALUES ('Classroom', 2),
       ('Studyroom', 1),
       ('Auditorium', 3);

DECLARE @RT_ClassroomID  INT = (SELECT RoomTypeID FROM dbo.RoomType WHERE RoomType = 'Classroom');
DECLARE @RT_StudyroomID  INT = (SELECT RoomTypeID FROM dbo.RoomType WHERE RoomType = 'Studyroom');
DECLARE @RT_AuditoriumID INT = (SELECT RoomTypeID FROM dbo.RoomType WHERE RoomType = 'Auditorium');


/* ===========================
   Rooms
   (From your Excel screenshot)
   =========================== */

DECLARE @i INT;

-- Building D, Level 3, Classrooms 1-11
SET @i = 1;
WHILE @i <= 11
BEGIN
    INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
    VALUES (@BuildingDID, CAST(@i AS VARCHAR(10)), '3', @RT_ClassroomID);
    SET @i = @i + 1;
END

-- Building D, Level 2, Classrooms 1-11
SET @i = 1;
WHILE @i <= 11
BEGIN
    INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
    VALUES (@BuildingDID, CAST(@i AS VARCHAR(10)), '2', @RT_ClassroomID);
    SET @i = @i + 1;
END

-- Building D, Study rooms (3:1-2, 2:1-2)
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES (@BuildingDID, '1', '3', @RT_StudyroomID),
       (@BuildingDID, '2', '3', @RT_StudyroomID),
       (@BuildingDID, '1', '2', @RT_StudyroomID),
       (@BuildingDID, '2', '2', @RT_StudyroomID);

-- Building D, Auditoriums (Level 1, 1-4)
SET @i = 1;
WHILE @i <= 4
BEGIN
    INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
    VALUES (@BuildingDID, CAST(@i AS VARCHAR(10)), '1', @RT_AuditoriumID);
    SET @i = @i + 1;
END

-- Building A, Study rooms
-- Level 3: 13, 15, 17
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES (@BuildingAID, '13', '3', @RT_StudyroomID),
       (@BuildingAID, '15', '3', @RT_StudyroomID),
       (@BuildingAID, '17', '3', @RT_StudyroomID);

-- Level 2: 12, 13, 14, 15, 16, 17
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES (@BuildingAID, '12', '2', @RT_StudyroomID),
       (@BuildingAID, '13', '2', @RT_StudyroomID),
       (@BuildingAID, '14', '2', @RT_StudyroomID),
       (@BuildingAID, '15', '2', @RT_StudyroomID),
       (@BuildingAID, '16', '2', @RT_StudyroomID),
       (@BuildingAID, '17', '2', @RT_StudyroomID);


/* ===========================
   SmartBoards
   (one SmartBoard for every Room)
   =========================== */

INSERT INTO dbo.SmartBoard (RoomID)
SELECT RoomID
FROM dbo.Room;


/* ===========================
   Users
   =========================== */

DECLARE @User_MathiasID INT;
DECLARE @User_TheodorID INT;
DECLARE @User_MarkusID  INT;
DECLARE @User_JakobID   INT;
DECLARE @User_CamillaID INT;
DECLARE @User_JetteID   INT;

-- 3 Students
INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES ('Mathias', '11111111', 'mathias@zealand.dk', 'Password123!', @UT_StudentID);
SET @User_MathiasID = SCOPE_IDENTITY();

INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES ('Theodor', '22222222', 'theodor@zealand.dk', 'Password123!', @UT_StudentID);
SET @User_TheodorID = SCOPE_IDENTITY();

INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES ('Markus', '33333333', 'markus@zealand.dk', 'Password123!', @UT_StudentID);
SET @User_MarkusID = SCOPE_IDENTITY();

-- 2 Teachers
INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES ('Jakob', '44444444', 'jakob@zealand.dk', 'Password123!', @UT_TeacherID);
SET @User_JakobID = SCOPE_IDENTITY();

INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES ('Camilla', '55555555', 'camilla@zealand.dk', 'Password123!', @UT_TeacherID);
SET @User_CamillaID = SCOPE_IDENTITY();

-- 1 Admin
INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES ('Jette', '66666666', 'jette@zealand.dk', 'Password123!', @UT_AdminID);
SET @User_JetteID = SCOPE_IDENTITY();


/* ===========================
   User–Department mapping
   (all users belong to Roskilde)
   =========================== */

INSERT INTO dbo.UserDepartmentMapping (UserID, DepartmentID)
VALUES (@User_MathiasID,  @DeptRoskildeID),
       (@User_TheodorID,  @DeptRoskildeID),
       (@User_MarkusID,   @DeptRoskildeID),
       (@User_JakobID,    @DeptRoskildeID),
       (@User_CamillaID,  @DeptRoskildeID),
       (@User_JetteID,    @DeptRoskildeID);
