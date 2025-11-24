
USE ZealandBooking;
GO

-- Table-Valued parameters
-- Drops list
IF TYPE_ID('dbo.IntList') IS NOT NULL
    DROP TYPE dbo.IntList;
GO
IF TYPE_ID('dbo.LevelList') IS NOT NULL
    DROP TYPE dbo.LevelList;
GO
IF TYPE_ID('dbo.TimeList') IS NOT NULL
    DROP TYPE dbo.TimeList;
GO
-- Creates list
CREATE TYPE dbo.IntList AS TABLE
(
    Id INT NOT NULL
);
GO
CREATE TYPE dbo.LevelList AS TABLE
(
    Level VARCHAR(3) NOT NULL
);
GO
CREATE TYPE dbo.TimeList AS TABLE
(
    StartTime TIME NOT NULL
);
GO
