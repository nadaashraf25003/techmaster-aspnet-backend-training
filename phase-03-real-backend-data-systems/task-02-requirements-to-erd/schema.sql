-- ====================================================================================================
-- Phase 03: Task 02 - Requirements to ERD: Database DDL Schema Script
-- Database: TechMasterAcademyDb (Microsoft SQL Server)
-- ====================================================================================================

-- 1. Create Database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TechMasterAcademyDb')
BEGIN
    CREATE DATABASE [TechMasterAcademyDb];
END
GO

USE [TechMasterAcademyDb];
GO

-- Clean up existing tables if running idempotent re-creation
IF OBJECT_ID(N'dbo.AssignmentSubmissions', N'U') IS NOT NULL DROP TABLE dbo.AssignmentSubmissions;
IF OBJECT_ID(N'dbo.Assignments', N'U') IS NOT NULL DROP TABLE dbo.Assignments;
IF OBJECT_ID(N'dbo.Attendances', N'U') IS NOT NULL DROP TABLE dbo.Attendances;
IF OBJECT_ID(N'dbo.TrackSessions', N'U') IS NOT NULL DROP TABLE dbo.TrackSessions;
IF OBJECT_ID(N'dbo.Payments', N'U') IS NOT NULL DROP TABLE dbo.Payments;
IF OBJECT_ID(N'dbo.Enrollments', N'U') IS NOT NULL DROP TABLE dbo.Enrollments;
IF OBJECT_ID(N'dbo.TrainingTracks', N'U') IS NOT NULL DROP TABLE dbo.TrainingTracks;
IF OBJECT_ID(N'dbo.Instructors', N'U') IS NOT NULL DROP TABLE dbo.Instructors;
IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL DROP TABLE dbo.Students;
GO

-- ====================================================================================================
-- 2. Core Entities DDL
-- ====================================================================================================

-- ----------------------------------------------------------------------------------------------------
-- TABLE: Students
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.Students (
    StudentId INT IDENTITY(1,1) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    PhoneNumber VARCHAR(25) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Students_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Students_IsDeleted DEFAULT (0),
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(7) NULL,
    DeletedAt DATETIME2(7) NULL,

    CONSTRAINT PK_Students PRIMARY KEY CLUSTERED (StudentId)
);
GO

-- Filtered Unique Index on Email to allow soft delete support
CREATE UNIQUE NONCLUSTERED INDEX UQ_Students_Email ON dbo.Students (Email) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Students_IsActive ON dbo.Students (IsActive) WHERE IsDeleted = 0;
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: Instructors
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.Instructors (
    InstructorId INT IDENTITY(1,1) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    Bio NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Instructors_IsActive DEFAULT (1),
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Instructors_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(7) NULL,

    CONSTRAINT PK_Instructors PRIMARY KEY CLUSTERED (InstructorId)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Instructors_Email ON dbo.Instructors (Email);
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: TrainingTracks
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.TrainingTracks (
    TrainingTrackId INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Code VARCHAR(30) NOT NULL,
    Description NVARCHAR(2000) NULL,
    Level VARCHAR(20) NOT NULL,
    Price DECIMAL(18,2) NOT NULL CONSTRAINT DF_TrainingTracks_Price DEFAULT (0.00),
    Capacity INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL,
    InstructorId INT NOT NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_TrainingTracks_IsDeleted DEFAULT (0),
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_TrainingTracks_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(7) NULL,
    DeletedAt DATETIME2(7) NULL,

    CONSTRAINT PK_TrainingTracks PRIMARY KEY CLUSTERED (TrainingTrackId),
    CONSTRAINT FK_TrainingTracks_Instructors FOREIGN KEY (InstructorId) REFERENCES dbo.Instructors(InstructorId) ON DELETE NO ACTION,
    CONSTRAINT CK_TrainingTracks_Level CHECK (Level IN ('Beginner', 'Intermediate', 'Advanced')),
    CONSTRAINT CK_TrainingTracks_Status CHECK (Status IN ('Draft', 'Upcoming', 'InProgress', 'Completed', 'Cancelled')),
    CONSTRAINT CK_TrainingTracks_Capacity CHECK (Capacity > 0),
    CONSTRAINT CK_TrainingTracks_Price CHECK (Price >= 0.00),
    CONSTRAINT CK_TrainingTracks_Dates CHECK (EndDate >= StartDate)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_TrainingTracks_Code ON dbo.TrainingTracks (Code) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_TrainingTracks_InstructorId ON dbo.TrainingTracks (InstructorId);
CREATE NONCLUSTERED INDEX IX_TrainingTracks_Status ON dbo.TrainingTracks (Status) WHERE IsDeleted = 0;
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: Enrollments
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.Enrollments (
    EnrollmentId INT IDENTITY(1,1) NOT NULL,
    StudentId INT NOT NULL,
    TrainingTrackId INT NOT NULL,
    EnrollmentDate DATETIME2(7) NOT NULL CONSTRAINT DF_Enrollments_EnrollmentDate DEFAULT (SYSUTCDATETIME()),
    Status VARCHAR(20) NOT NULL,
    ProgressPercentage DECIMAL(5,2) NOT NULL CONSTRAINT DF_Enrollments_Progress DEFAULT (0.00),
    FinalResult VARCHAR(20) NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Enrollments_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(7) NULL,

    CONSTRAINT PK_Enrollments PRIMARY KEY CLUSTERED (EnrollmentId),
    CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_Enrollments_TrainingTracks FOREIGN KEY (TrainingTrackId) REFERENCES dbo.TrainingTracks(TrainingTrackId) ON DELETE NO ACTION,
    CONSTRAINT CK_Enrollments_Status CHECK (Status IN ('Pending', 'Active', 'Completed', 'Dropped', 'Suspended')),
    CONSTRAINT CK_Enrollments_Progress CHECK (ProgressPercentage >= 0.00 AND ProgressPercentage <= 100.00),
    CONSTRAINT CK_Enrollments_FinalResult CHECK (FinalResult IN ('Pass', 'Fail', 'Distinction', 'Incomplete') OR FinalResult IS NULL),
    CONSTRAINT UQ_Enrollments_Student_Track UNIQUE (StudentId, TrainingTrackId)
);
GO

CREATE NONCLUSTERED INDEX IX_Enrollments_TrackId ON dbo.Enrollments (TrainingTrackId);
CREATE NONCLUSTERED INDEX IX_Enrollments_Status ON dbo.Enrollments (Status);
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: Payments
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.Payments (
    PaymentId INT IDENTITY(1,1) NOT NULL,
    EnrollmentId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod VARCHAR(30) NOT NULL,
    PaymentDate DATETIME2(7) NOT NULL CONSTRAINT DF_Payments_PaymentDate DEFAULT (SYSUTCDATETIME()),
    PaymentStatus VARCHAR(20) NOT NULL,
    ReferenceNumber VARCHAR(100) NOT NULL,
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Payments_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Payments PRIMARY KEY CLUSTERED (PaymentId),
    CONSTRAINT FK_Payments_Enrollments FOREIGN KEY (EnrollmentId) REFERENCES dbo.Enrollments(EnrollmentId) ON DELETE CASCADE,
    CONSTRAINT CK_Payments_Amount CHECK (Amount > 0.00),
    CONSTRAINT CK_Payments_Method CHECK (PaymentMethod IN ('CreditCard', 'BankTransfer', 'Cash', 'VodafoneCash', 'Fawry', 'Stripe')),
    CONSTRAINT CK_Payments_Status CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed', 'Refunded'))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Payments_ReferenceNumber ON dbo.Payments (ReferenceNumber);
CREATE NONCLUSTERED INDEX IX_Payments_EnrollmentId ON dbo.Payments (EnrollmentId);
CREATE NONCLUSTERED INDEX IX_Payments_PaymentStatus ON dbo.Payments (PaymentStatus);
GO

-- ====================================================================================================
-- 3. Bonus Entities DDL (TrackSession, Attendance, Assignment, AssignmentSubmission)
-- ====================================================================================================

-- ----------------------------------------------------------------------------------------------------
-- TABLE: TrackSessions
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.TrackSessions (
    SessionId INT IDENTITY(1,1) NOT NULL,
    TrainingTrackId INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    SessionDate DATE NOT NULL,
    StartTime TIME(0) NOT NULL,
    EndTime TIME(0) NOT NULL,
    RoomOrLink NVARCHAR(255) NULL,
    IsCompleted BIT NOT NULL CONSTRAINT DF_TrackSessions_IsCompleted DEFAULT (0),
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_TrackSessions_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_TrackSessions PRIMARY KEY CLUSTERED (SessionId),
    CONSTRAINT FK_TrackSessions_TrainingTracks FOREIGN KEY (TrainingTrackId) REFERENCES dbo.TrainingTracks(TrainingTrackId) ON DELETE CASCADE,
    CONSTRAINT CK_TrackSessions_Time CHECK (EndTime > StartTime)
);
GO

CREATE NONCLUSTERED INDEX IX_TrackSessions_TrackDate ON dbo.TrackSessions (TrainingTrackId, SessionDate);
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: Attendances
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.Attendances (
    AttendanceId INT IDENTITY(1,1) NOT NULL,
    TrackSessionId INT NOT NULL,
    StudentId INT NOT NULL,
    Status VARCHAR(20) NOT NULL,
    MarkedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Attendances_MarkedAt DEFAULT (SYSUTCDATETIME()),
    Remarks NVARCHAR(255) NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Attendances_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Attendances PRIMARY KEY CLUSTERED (AttendanceId),
    CONSTRAINT FK_Attendances_TrackSessions FOREIGN KEY (TrackSessionId) REFERENCES dbo.TrackSessions(SessionId) ON DELETE CASCADE,
    CONSTRAINT FK_Attendances_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId) ON DELETE NO ACTION,
    CONSTRAINT CK_Attendances_Status CHECK (Status IN ('Present', 'Absent', 'Late', 'Excused')),
    CONSTRAINT UQ_Attendances_Session_Student UNIQUE (TrackSessionId, StudentId)
);
GO

CREATE NONCLUSTERED INDEX IX_Attendances_StudentId ON dbo.Attendances (StudentId);
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: Assignments
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.Assignments (
    AssignmentId INT IDENTITY(1,1) NOT NULL,
    TrainingTrackId INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NULL,
    DueDate DATETIME2(7) NOT NULL,
    MaxScore DECIMAL(5,2) NOT NULL CONSTRAINT DF_Assignments_MaxScore DEFAULT (100.00),
    WeightPercentage DECIMAL(5,2) NOT NULL CONSTRAINT DF_Assignments_Weight DEFAULT (10.00),
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Assignments_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Assignments PRIMARY KEY CLUSTERED (AssignmentId),
    CONSTRAINT FK_Assignments_TrainingTracks FOREIGN KEY (TrainingTrackId) REFERENCES dbo.TrainingTracks(TrainingTrackId) ON DELETE CASCADE,
    CONSTRAINT CK_Assignments_MaxScore CHECK (MaxScore > 0.00),
    CONSTRAINT CK_Assignments_Weight CHECK (WeightPercentage >= 0.00 AND WeightPercentage <= 100.00)
);
GO

CREATE NONCLUSTERED INDEX IX_Assignments_TrackId ON dbo.Assignments (TrainingTrackId);
GO

-- ----------------------------------------------------------------------------------------------------
-- TABLE: AssignmentSubmissions
-- ----------------------------------------------------------------------------------------------------
CREATE TABLE dbo.AssignmentSubmissions (
    SubmissionId INT IDENTITY(1,1) NOT NULL,
    AssignmentId INT NOT NULL,
    StudentId INT NOT NULL,
    SubmittedAt DATETIME2(7) NOT NULL CONSTRAINT DF_AssignmentSubmissions_SubmittedAt DEFAULT (SYSUTCDATETIME()),
    ContentUrl NVARCHAR(500) NOT NULL,
    Score DECIMAL(5,2) NULL,
    Feedback NVARCHAR(1000) NULL,
    GradedAt DATETIME2(7) NULL,

    CONSTRAINT PK_AssignmentSubmissions PRIMARY KEY CLUSTERED (SubmissionId),
    CONSTRAINT FK_AssignmentSubmissions_Assignments FOREIGN KEY (AssignmentId) REFERENCES dbo.Assignments(AssignmentId) ON DELETE CASCADE,
    CONSTRAINT FK_AssignmentSubmissions_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId) ON DELETE NO ACTION,
    CONSTRAINT CK_AssignmentSubmissions_Score CHECK (Score IS NULL OR (Score >= 0.00 AND Score <= 100.00)),
    CONSTRAINT UQ_Submissions_Assignment_Student UNIQUE (AssignmentId, StudentId)
);
GO

CREATE NONCLUSTERED INDEX IX_AssignmentSubmissions_StudentId ON dbo.AssignmentSubmissions (StudentId);
GO

-- ====================================================================================================
-- 4. High-Quality Seed Data for Query Verification & Testing
-- ====================================================================================================

-- 4.1 Instructors
INSERT INTO dbo.Instructors (FullName, Email, Specialization, Bio, IsActive) VALUES
(N'Eng. Ahmed Hassan', 'ahmed.hassan@techmaster.net', N'.NET & Cloud Architecture', N'Senior Solutions Architect with 14+ years experience in Enterprise Systems.', 1),
(N'Eng. Sara Ibrahim', 'sara.ibrahim@techmaster.net', N'Full Stack React & Node', N'Principal Frontend Engineer and Technical Speaker.', 1),
(N'Eng. Mahmoud Ali', 'mahmoud.ali@techmaster.net', N'DevOps & Kubernetes', N'DevOps Lead specializing in Azure CI/CD pipelines.', 1),
(N'Eng. Mona Farouk', 'mona.farouk@techmaster.net', N'AI & Data Engineering', N'Data Science Consultant with PhD in Applied ML.', 1);

-- 4.2 Students
INSERT INTO dbo.Students (FullName, Email, PhoneNumber, IsActive, IsDeleted) VALUES
(N'Nada Ashraf', 'nada.ashraf@student.techmaster.net', '+201001234567', 1, 0),
(N'Youssef Mohamed', 'youssef.mohamed@student.techmaster.net', '+201009876543', 1, 0),
(N'Mariam Tarek', 'mariam.tarek@student.techmaster.net', '+201112233445', 1, 0),
(N'Omar Khaled', 'omar.khaled@student.techmaster.net', '+201223344556', 1, 0),
(N'Kareem Gamal', 'kareem.gamal@student.techmaster.net', '+201011223344', 1, 0),
(N'Hoda Mostafa', 'hoda.mostafa@student.techmaster.net', '+201155667788', 1, 0),
(N'Tamer Samir', 'tamer.samir@student.techmaster.net', '+201299887766', 0, 0),
(N'Deactivated OldStudent', 'deleted.student@student.techmaster.net', '+201000000000', 0, 1);

-- 4.3 Training Tracks
INSERT INTO dbo.TrainingTracks (Title, Code, Description, Level, Price, Capacity, StartDate, EndDate, Status, InstructorId, IsDeleted) VALUES
(N'ASP.NET Core Enterprise Backend BootCamp', 'NET-BE-2026', N'Deep dive into EF Core, SQL Server, Clean Architecture, and Azure deployment.', 'Advanced', 4500.00, 3, '2026-09-01', '2026-12-15', 'InProgress', 1, 0),
(N'Modern Full Stack React & ASP.NET API', 'FS-REACT-2026', N'Comprehensive fullstack journey building cloud-native apps.', 'Intermediate', 5200.00, 2, '2026-09-20', '2027-01-30', 'Upcoming', 2, 0),
(N'Enterprise DevOps & Kubernetes Masterclass', 'DEVOPS-K8S-2026', N'Docker, Kubernetes, Terraform, and CI/CD pipelines.', 'Advanced', 6000.00, 20, '2026-10-15', '2027-02-15', 'Upcoming', 3, 0),
(N'Data Engineering & Power BI FastTrack', 'DATA-BI-2026', N'Data warehousing, ETL pipelines, and BI reporting.', 'Beginner', 3800.00, 15, '2026-08-01', '2026-10-01', 'Completed', 4, 0);

-- 4.4 Enrollments
INSERT INTO dbo.Enrollments (StudentId, TrainingTrackId, EnrollmentDate, Status, ProgressPercentage, FinalResult) VALUES
-- Track 1 (NET-BE-2026): Capacity 3, 3 enrolled -> FULL!
(1, 1, '2026-08-15 10:00:00', 'Active', 65.50, NULL),
(2, 1, '2026-08-16 11:30:00', 'Active', 70.00, NULL),
(3, 1, '2026-08-18 14:00:00', 'Active', 45.00, NULL),

-- Track 2 (FS-REACT-2026): Capacity 2, 1 enrolled -> 1 seat available
(4, 2, '2026-09-05 09:00:00', 'Active', 10.00, NULL),

-- Track 3 (DEVOPS-K8S-2026): Capacity 20, 2 enrolled -> 18 seats available
(1, 3, '2026-09-10 16:00:00', 'Pending', 0.00, NULL),
(5, 3, '2026-09-12 12:00:00', 'Active', 0.00, NULL),

-- Track 4 (DATA-BI-2026): Completed track
(6, 4, '2026-07-20 10:00:00', 'Completed', 100.00, 'Distinction');

-- 4.5 Payments (Installment & Lump-sum testing)
INSERT INTO dbo.Payments (EnrollmentId, Amount, PaymentMethod, PaymentDate, PaymentStatus, ReferenceNumber, Notes) VALUES
-- Enrollment 1 (Nada in NET-BE-2026, Price 4500): Paid in 2 installments (Total 4500) -> Fully Paid
(1, 2250.00, 'CreditCard', '2026-08-15 10:15:00', 'Completed', 'PAY-20260815-001', N'First installment paid via Visa.'),
(1, 2250.00, 'Fawry', '2026-09-01 14:20:00', 'Completed', 'PAY-20260901-002', N'Second installment paid via Fawry ref 981273.'),

-- Enrollment 2 (Youssef in NET-BE-2026, Price 4500): Paid 2000 -> Partially Paid (Unpaid remaining: 2500)
(2, 2000.00, 'VodafoneCash', '2026-08-16 11:45:00', 'Completed', 'PAY-20260816-003', N'Vodafone cash transfer from wallet 01009876543.'),

-- Enrollment 3 (Mariam in NET-BE-2026, Price 4500): 0 payments -> Unpaid!

-- Enrollment 4 (Omar in FS-REACT-2026, Price 5200): Paid in full via Bank Transfer
(4, 5200.00, 'BankTransfer', '2026-09-05 09:30:00', 'Completed', 'PAY-20260905-004', N'CIB Bank transfer wire receipt #88129.'),

-- Enrollment 5 (Nada in DEVOPS-K8S-2026, Price 6000): Pending payment attempt that failed
(5, 6000.00, 'CreditCard', '2026-09-10 16:05:00', 'Failed', 'PAY-20260910-005', N'Card declined insufficient funds.'),

-- Enrollment 6 (Kareem in DEVOPS-K8S-2026, Price 6000): Paid 3000 installment
(6, 3000.00, 'CreditCard', '2026-09-12 12:15:00', 'Completed', 'PAY-20260912-006', N'First half tuition fee.'),

-- Enrollment 7 (Hoda in DATA-BI-2026, Price 3800): Fully Paid Cash
(7, 3800.00, 'Cash', '2026-07-20 10:30:00', 'Completed', 'PAY-20260720-007', N'Paid at Academy front desk.');

-- 4.6 Bonus: Track Sessions
INSERT INTO dbo.TrackSessions (TrainingTrackId, Title, SessionDate, StartTime, EndTime, RoomOrLink, IsCompleted) VALUES
(1, N'Session 01: C# Advanced & Memory Management', '2026-09-02', '18:00:00', '21:00:00', 'Lab 101 / Teams Link A', 1),
(1, N'Session 02: EF Core In-Depth & Migrations', '2026-09-05', '18:00:00', '21:00:00', 'Lab 101 / Teams Link A', 1),
(1, N'Session 03: REST APIs & Repository Pattern', '2026-09-09', '18:00:00', '21:00:00', 'Lab 101 / Teams Link A', 0);

-- 4.7 Bonus: Attendances
INSERT INTO dbo.Attendances (TrackSessionId, StudentId, Status, MarkedAt, Remarks) VALUES
(1, 1, 'Present', '2026-09-02 18:05:00', N'On-time'),
(1, 2, 'Present', '2026-09-02 18:02:00', N'On-time'),
(1, 3, 'Late', '2026-09-02 18:40:00', N'Traffic delay'),
(2, 1, 'Present', '2026-09-05 18:01:00', N'On-time'),
(2, 2, 'Absent', '2026-09-05 18:30:00', N'Sick leave notified');

-- 4.8 Bonus: Assignments & Submissions
INSERT INTO dbo.Assignments (TrainingTrackId, Title, Description, DueDate, MaxScore, WeightPercentage) VALUES
(1, N'Assignment 01: EF Core Relational Drill', N'Design entities, fluent API configurations, and migrations.', '2026-09-15 23:59:59', 100.00, 20.00),
(1, N'Assignment 02: RESTful API Layering', N'Build controller, service, repository, and DTO mappings.', '2026-09-30 23:59:59', 100.00, 25.00);

INSERT INTO dbo.AssignmentSubmissions (AssignmentId, StudentId, SubmittedAt, ContentUrl, Score, Feedback, GradedAt) VALUES
(1, 1, '2026-09-14 20:15:00', 'https://github.com/nadaashraf/ef-core-drill', 98.50, N'Exceptional architecture, clean Fluent API mapping.', '2026-09-16 10:00:00'),
(1, 2, '2026-09-15 22:30:00', 'https://github.com/youssef/ef-core-drill', 88.00, N'Good work. Missing unique composite index on join entity.', '2026-09-16 11:30:00');
GO
