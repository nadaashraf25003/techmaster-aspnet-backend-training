-- ==========================================================
-- Scenario C: Training Center Registration System - Schema Definitions
-- ==========================================================

-- Drop existing tables to ensure clean run
IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL DROP TABLE dbo.Payments;
IF OBJECT_ID('dbo.Registrations', 'U') IS NOT NULL DROP TABLE dbo.Registrations;
IF OBJECT_ID('dbo.Tracks', 'U') IS NOT NULL DROP TABLE dbo.Tracks;
IF OBJECT_ID('dbo.Instructors', 'U') IS NOT NULL DROP TABLE dbo.Instructors;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;

CREATE TABLE Students (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Instructors (
    InstructorId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Specialization NVARCHAR(100) NULL
);

CREATE TABLE Tracks (
    TrackId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    DurationWeeks INT NOT NULL CHECK (DurationWeeks > 0),
    StartDate DATETIME NOT NULL,
    InstructorId INT NOT NULL FOREIGN KEY REFERENCES Instructors(InstructorId)
);

CREATE TABLE Registrations (
    RegistrationId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId),
    TrackId INT NOT NULL FOREIGN KEY REFERENCES Tracks(TrackId),
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active'
);

CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    RegistrationId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Registrations(RegistrationId) ON DELETE CASCADE,
    Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0),
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Completed'
);
