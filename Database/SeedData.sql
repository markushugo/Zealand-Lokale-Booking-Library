USE ZealandBooking;

------------------------------------------------------------
-- Department (Roksilde)
------------------------------------------------------------
DECLARE @Dept_Roskilde INT;

INSERT INTO dbo.Department (Address, Name)
VALUES ('Maglegårdsvej 2, 4000 Roskilde', 'Roskilde');

SELECT @Dept_Roskilde = DepartmentID FROM dbo.Department WHERE Name = 'Roskilde';

------------------------------------------------------------
-- User types (Student, Teacher, Admin)
------------------------------------------------------------
DECLARE @UT_Student INT,
        @UT_Teacher INT,
        @UT_Admin   INT;

INSERT INTO dbo.UserType (UserType)
VALUES ('Student'),
       ('Teacher'),
       ('Admin');

SELECT @UT_Student = UserTypeID FROM dbo.UserType WHERE UserType = 'Student';
SELECT @UT_Teacher = UserTypeID FROM dbo.UserType WHERE UserType = 'Teacher';
SELECT @UT_Admin   = UserTypeID FROM dbo.UserType WHERE UserType = 'Admin';

------------------------------------------------------------
-- Room types (Classroom, Studyroom, Auditorium)
------------------------------------------------------------
DECLARE @RT_Classroom  INT,
        @RT_Studyroom  INT,
        @RT_Auditorium INT;

INSERT INTO dbo.RoomType (RoomType, Capacity)
VALUES ('Classroom',  2),
       ('Studyroom',  1),
       ('Auditorium', 3);

SELECT @RT_Classroom  = RoomTypeID FROM dbo.RoomType WHERE RoomType = 'Classroom';
SELECT @RT_Studyroom  = RoomTypeID FROM dbo.RoomType WHERE RoomType = 'Studyroom';
SELECT @RT_Auditorium = RoomTypeID FROM dbo.RoomType WHERE RoomType = 'Auditorium';

------------------------------------------------------------
-- Buildings (A and D for Roskilde)
------------------------------------------------------------
DECLARE @Bld_A INT,
        @Bld_D INT;

INSERT INTO dbo.Building (DepartmentID, Name)
VALUES (@Dept_Roskilde, 'A'),
       (@Dept_Roskilde, 'D');

SELECT @Bld_A = BuildingID FROM dbo.Building WHERE DepartmentID = @Dept_Roskilde AND Name = 'A';
SELECT @Bld_D = BuildingID FROM dbo.Building WHERE DepartmentID = @Dept_Roskilde AND Name = 'D';

------------------------------------------------------------
-- Rooms
------------------------------------------------------------
--------------------------------
-- Rooms in Building D Roskilde
--------------------------------
-- Building D, 3rd floor, Classroom 1–11
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_D, '1',  '3', @RT_Classroom),
(@Bld_D, '2',  '3', @RT_Classroom),
(@Bld_D, '3',  '3', @RT_Classroom),
(@Bld_D, '4',  '3', @RT_Classroom),
(@Bld_D, '5',  '3', @RT_Classroom),
(@Bld_D, '6',  '3', @RT_Classroom),
(@Bld_D, '7',  '3', @RT_Classroom),
(@Bld_D, '8',  '3', @RT_Classroom),
(@Bld_D, '9',  '3', @RT_Classroom),
(@Bld_D, '10', '3', @RT_Classroom),
(@Bld_D, '11', '3', @RT_Classroom);
-- Building D, 2nd floor, Classroom 1–11
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_D, '1',  '2', @RT_Classroom),
(@Bld_D, '2',  '2', @RT_Classroom),
(@Bld_D, '3',  '2', @RT_Classroom),
(@Bld_D, '4',  '2', @RT_Classroom),
(@Bld_D, '5',  '2', @RT_Classroom),
(@Bld_D, '6',  '2', @RT_Classroom),
(@Bld_D, '7',  '2', @RT_Classroom),
(@Bld_D, '8',  '2', @RT_Classroom),
(@Bld_D, '9',  '2', @RT_Classroom),
(@Bld_D, '10', '2', @RT_Classroom),
(@Bld_D, '11', '2', @RT_Classroom);
-- Building D, 3rd floor, Studyroom 1–2
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_D, '1', '3', @RT_Studyroom),
(@Bld_D, '2', '3', @RT_Studyroom);
-- Building D, 2nd floor, Studyroom 1–2
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_D, '1', '2', @RT_Studyroom),
(@Bld_D, '2', '2', @RT_Studyroom);
-- Building D, 1st floor, Auditorium 1–4
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_D, '1', '1', @RT_Auditorium),
(@Bld_D, '2', '1', @RT_Auditorium),
(@Bld_D, '3', '1', @RT_Auditorium),
(@Bld_D, '4', '1', @RT_Auditorium);

--------------------------------
-- Rooms in Building A Roskilde
--------------------------------
-- Building A, 3rd floor, Study 13,15,17
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_A, '13', '3', @RT_Studyroom),
(@Bld_A, '15', '3', @RT_Studyroom),
(@Bld_A, '17', '3', @RT_Studyroom);
-- Building A, 2nd floor, Study 12–17
INSERT INTO dbo.Room (BuildingID, Name, Level, RoomTypeID)
VALUES
(@Bld_A, '12', '2', @RT_Studyroom),
(@Bld_A, '13', '2', @RT_Studyroom),
(@Bld_A, '14', '2', @RT_Studyroom),
(@Bld_A, '15', '2', @RT_Studyroom),
(@Bld_A, '16', '2', @RT_Studyroom),
(@Bld_A, '17', '2', @RT_Studyroom);

------------------------------------------------------------
-- SmartBoards (one per room)
------------------------------------------------------------
INSERT INTO dbo.SmartBoard (RoomID)
SELECT RoomID
FROM dbo.Room;

------------------------------------------------------------
-- Users
-- Students: Mathias, Theodor, Markus
-- Teachers: Jakob, Camilla
-- Admins: Jette
------------------------------------------------------------
DECLARE @User_Mathias INT,
        @User_Theodor INT,
        @User_Markus  INT,
        @User_Jakob   INT,
        @User_Camilla INT,
        @User_Jette   INT;

INSERT INTO dbo.[User] (Name, Phone, Email, Password, UserTypeID)
VALUES
('Mathias', '12345678', 'mathias@edu.zealand.dk', 'mathias123!', @UT_Student),
('Theodor', '23456789', 'theodor@edu.zealand.dk', 'theodor123!', @UT_Student),
('Markus',  '67676767', 'markus@edu.zealand.dk',  'markus123!',  @UT_Student),
('Jakob',   '12341234', 'jakob@edu.zealand.dk',   'jakob123!',   @UT_Teacher),
('Camilla', '43214321', 'camilla@edu.zealand.dk', 'camilla123!', @UT_Teacher),
('Jette',   '88888888', 'jette@edu.zealand.dk',   'jette123!',   @UT_Admin);

SELECT @User_Mathias = UserID FROM dbo.[User] WHERE Name = 'Mathias';
SELECT @User_Theodor = UserID FROM dbo.[User] WHERE Name = 'Theodor';
SELECT @User_Markus  = UserID FROM dbo.[User] WHERE Name = 'Markus';
SELECT @User_Jakob   = UserID FROM dbo.[User] WHERE Name = 'Jakob';
SELECT @User_Camilla = UserID FROM dbo.[User] WHERE Name = 'Camilla';
SELECT @User_Jette   = UserID FROM dbo.[User] WHERE Name = 'Jette';

------------------------------------------------------------
-- User–Department mapping (all users mapped to Roskilde)
------------------------------------------------------------
INSERT INTO dbo.UserDepartmentMapping (UserID, DepartmentID)
VALUES
(@User_Mathias, @Dept_Roskilde),
(@User_Theodor, @Dept_Roskilde),
(@User_Markus,  @Dept_Roskilde),
(@User_Jakob,   @Dept_Roskilde),
(@User_Camilla, @Dept_Roskilde),
(@User_Jette,   @Dept_Roskilde);

------------------------------------------------------------
-- Bookings
-- Times within 08:00–14:00, date = tomorrow 
------------------------------------------------------------
DECLARE @Tomorrow DATE = CAST(DATEADD(DAY, 1, GETDATE()) AS DATE);

-- RoomIDS used in bookings
DECLARE @Room_D3_1_Class INT,
        @Room_D2_1_Class INT,
        @Room_D3_1_Study INT,
        @Room_D3_2_Class INT,
        @Room_A3_13_Study INT,
        @Room_D1_1_Aud   INT,
        @Room_D2_2_Class INT,
        @Room_A2_12_Study INT,
        @Room_D1_2_Aud   INT;

SELECT @Room_D3_1_Class = RoomID FROM dbo.Room WHERE BuildingID = @Bld_D AND Level = '3' AND Name = '1' AND RoomTypeID = @RT_Classroom;
SELECT @Room_D2_1_Class = RoomID FROM dbo.Room WHERE BuildingID = @Bld_D AND Level = '2' AND Name = '1' AND RoomTypeID = @RT_Classroom;
SELECT @Room_D3_1_Study = RoomID FROM dbo.Room WHERE BuildingID = @Bld_D AND Level = '3' AND Name = '1' AND RoomTypeID = @RT_Studyroom;
SELECT @Room_D3_2_Class = RoomID FROM dbo.Room WHERE BuildingID = @Bld_D AND Level = '3' AND Name = '2' AND RoomTypeID = @RT_Classroom;
SELECT @Room_A3_13_Study = RoomID FROM dbo.Room WHERE BuildingID = @Bld_A AND Level = '3' AND Name = '13' AND RoomTypeID = @RT_Studyroom;
SELECT @Room_D2_2_Class = RoomID FROM dbo.Room WHERE BuildingID = @Bld_D AND Level = '2' AND Name = '2' AND RoomTypeID = @RT_Classroom;
SELECT @Room_A2_12_Study = RoomID FROM dbo.Room WHERE BuildingID = @Bld_A AND Level = '2' AND Name = '12' AND RoomTypeID = @RT_Studyroom;

-- SmartBoardIDS matching the rooms
DECLARE @SB_D3_1_Class   INT,
        @SB_D2_1_Class   INT,
        @SB_D3_1_Study   INT,
        @SB_D3_2_Class   INT,
        @SB_A3_13_Study  INT,
        @SB_D2_2_Class   INT,
        @SB_A2_12_Study  INT

SELECT @SB_D3_1_Class  = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_D3_1_Class;
SELECT @SB_D2_1_Class  = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_D2_1_Class;
SELECT @SB_D3_1_Study  = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_D3_1_Study;
SELECT @SB_D3_2_Class  = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_D3_2_Class;
SELECT @SB_A3_13_Study = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_A3_13_Study;
SELECT @SB_D2_2_Class  = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_D2_2_Class;
SELECT @SB_A2_12_Study = SmartBoardID FROM dbo.SmartBoard WHERE RoomID = @Room_A2_12_Study;

-- Creating the bookings
INSERT INTO dbo.Booking (RoomID, UserID, StartTime, [Date], SmartBoardID)
VALUES
-- Mathias
(@Room_D3_1_Class, @User_Mathias, '08:00', @Tomorrow, @SB_D3_1_Class),
(@Room_D2_1_Class, @User_Mathias, '12:00', @Tomorrow, @SB_D2_1_Class),
(@Room_D3_1_Study, @User_Mathias, '10:00', @Tomorrow, @SB_D3_1_Study),
-- Theodor
(@Room_D3_2_Class,  @User_Theodor, '08:00', @Tomorrow, @SB_D3_2_Class),
(@Room_A3_13_Study, @User_Theodor, '10:00', @Tomorrow, @SB_A3_13_Study),
-- Markus
(@Room_D2_2_Class,  @User_Markus, '08:00', @Tomorrow, @SB_D2_2_Class),
(@Room_A2_12_Study, @User_Markus, '10:00', @Tomorrow, @SB_A2_12_Study)
