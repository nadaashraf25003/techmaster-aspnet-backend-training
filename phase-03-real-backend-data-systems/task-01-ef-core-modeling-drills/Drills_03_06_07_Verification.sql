-- =====================================================================
-- TechMaster Academy - Phase 03: Task 01 - EF Core Modeling Drills
-- SQL Verification Scripts for Drill 03, Drill 06, and Drill 07
-- Database: TechMaster_Drills_Db
-- =====================================================================

USE [TechMaster_Drills_Db];
GO

-- =====================================================================
-- 🎯 DRILL 03: One-to-Many Relationship (Instructor -> TrainingTracks)
-- =====================================================================

-- 1. View all Tracks with their assigned Instructor (Proving 1:N Relationship & FK)
SELECT 
    t.Id AS [TrackId],
    t.Code AS [TrackCode],
    t.Title AS [TrackTitle],
    t.Level,
    t.Capacity,
    t.Price,
    i.Id AS [InstructorId],
    i.FullName AS [InstructorName],
    i.Specialization AS [InstructorSpecialization],
    i.Email AS [InstructorEmail]
FROM [dbo].[TrainingTracks] t
INNER JOIN [dbo].[Instructors] i ON t.InstructorId = i.Id
ORDER BY i.Id, t.Id;

-- 2. Aggregate Query: Instructor Track Count & Total Teaching Capacity
SELECT 
    i.Id AS [InstructorId],
    i.FullName AS [InstructorName],
    i.Specialization,
    COUNT(t.Id) AS [TotalAssignedTracks],
    SUM(t.Capacity) AS [TotalStudentCapacity]
FROM [dbo].[Instructors] i
LEFT JOIN [dbo].[TrainingTracks] t ON i.Id = t.InstructorId
GROUP BY i.Id, i.FullName, i.Specialization;

-- 3. Verify Foreign Key Constraint Definition in SQL Server
SELECT 
    fk.name AS [ForeignKeyName],
    OBJECT_NAME(fk.parent_object_id) AS [ChildTable (TrainingTracks)],
    c1.name AS [FK_Column],
    OBJECT_NAME(fk.referenced_object_id) AS [ParentTable (Instructors)],
    c2.name AS [PK_Column],
    fk.delete_referential_action_desc AS [DeleteBehavior]
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id
INNER JOIN sys.columns c2 ON fkc.referenced_object_id = c2.object_id AND fkc.referenced_column_id = c2.column_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'TrainingTracks';
GO


-- =====================================================================
-- 🎯 DRILL 06: Seed Data Verification (HasData Evidence)
-- =====================================================================

-- 1. Seeded Instructors (Expected: >= 2)
SELECT [Id], [FullName], [Email], [Specialization], [CreatedAt]
FROM [dbo].[Instructors];

-- 2. Seeded Training Tracks (Expected: >= 3)
SELECT [Id], [Code], [Title], [Level], [Capacity], [Price], [InstructorId]
FROM [dbo].[TrainingTracks];

-- 3. Seeded Students & Their 1:1 Profiles (Expected: >= 5)
SELECT 
    s.[Id] AS [StudentId],
    s.[FullName],
    s.[Email],
    s.[PhoneNumber],
    p.[NationalId],
    p.[Address],
    p.[EmergencyPhone]
FROM [dbo].[Students] s
LEFT JOIN [dbo].[StudentProfiles] p ON s.[Id] = p.[StudentId];

-- 4. Seeded Enrollments & 1:1 Payment Summaries (Expected: >= 5)
SELECT 
    e.[Id] AS [EnrollmentId],
    s.[FullName] AS [StudentName],
    t.[Code] AS [TrackCode],
    t.[Title] AS [TrackTitle],
    e.[Status] AS [EnrollmentStatus],
    e.[EnrollmentDate],
    e.[FinalGrade],
    ps.[TotalRequired],
    ps.[TotalPaid],
    ps.[RemainingAmount],
    ps.[PaymentStatus]
FROM [dbo].[Enrollments] e
INNER JOIN [dbo].[Students] s ON e.[StudentId] = s.[Id]
INNER JOIN [dbo].[TrainingTracks] t ON e.[TrainingTrackId] = t.[Id]
LEFT JOIN [dbo].[PaymentSummaries] ps ON e.[Id] = ps.[EnrollmentId];

-- 5. Seed Metric Summary Count Check
SELECT 
    (SELECT COUNT(*) FROM [dbo].[Instructors]) AS [TotalInstructors],
    (SELECT COUNT(*) FROM [dbo].[TrainingTracks]) AS [TotalTracks],
    (SELECT COUNT(*) FROM [dbo].[Students]) AS [TotalStudents],
    (SELECT COUNT(*) FROM [dbo].[StudentProfiles]) AS [TotalProfiles],
    (SELECT COUNT(*) FROM [dbo].[Enrollments]) AS [TotalEnrollments],
    (SELECT COUNT(*) FROM [dbo].[PaymentSummaries]) AS [TotalPaymentSummaries];
GO


-- =====================================================================
-- 🎯 DRILL 07: Soft Delete & Global Query Filter Verification
-- =====================================================================

-- 1. Active Students Only (Simulating EF Core default HasQueryFilter: s => !s.IsDeleted)
SELECT [Id], [FullName], [Email], [IsDeleted], [DeletedAt], [CreatedAt], [UpdatedAt]
FROM [dbo].[Students]
WHERE [IsDeleted] = 0;

-- 2. All Students Including Soft-Deleted (Simulating EF Core .IgnoreQueryFilters())
SELECT [Id], [FullName], [Email], [IsDeleted], [DeletedAt], [CreatedAt], [UpdatedAt]
FROM [dbo].[Students];

-- 3. Soft-Deleted Students Only (Audit & Debug View)
SELECT [Id], [FullName], [Email], [IsDeleted], [DeletedAt]
FROM [dbo].[Students]
WHERE [IsDeleted] = 1;
GO


-- =====================================================================
-- 🎯 DRILL 08: Automated UTC Audit Tracking (CreatedAt / UpdatedAt)
-- =====================================================================

-- 1. View Audit Fields on Students (Proving CreatedAt on insert & UpdatedAt on update)
SELECT 
    [Id],
    [FullName],
    [Email],
    [CreatedAt] AS [CreatedAt_UTC],
    [UpdatedAt] AS [UpdatedAt_UTC],
    CASE 
        WHEN [UpdatedAt] IS NOT NULL THEN 'Updated / Modified'
        ELSE 'Original / Created Only'
    END AS [AuditStatus]
FROM [dbo].[Students]
ORDER BY [Id];

-- 2. View Only Modified Records with UpdatedAt Timestamps
SELECT 
    [Id],
    [FullName],
    [Email],
    [CreatedAt] AS [CreatedAt_UTC],
    [UpdatedAt] AS [UpdatedAt_UTC],
    DATEDIFF(MILLISECOND, [CreatedAt], [UpdatedAt]) AS [DiffInMs]
FROM [dbo].[Students]
WHERE [UpdatedAt] IS NOT NULL;

-- 3. Audit Fields across All Core Entities in the System
SELECT 'Students' AS [TableName], [Id], [CreatedAt], [UpdatedAt] FROM [dbo].[Students]
UNION ALL
SELECT 'Instructors' AS [TableName], [Id], [CreatedAt], [UpdatedAt] FROM [dbo].[Instructors]
UNION ALL
SELECT 'TrainingTracks' AS [TableName], [Id], [CreatedAt], [UpdatedAt] FROM [dbo].[TrainingTracks]
UNION ALL
SELECT 'Enrollments' AS [TableName], [Id], [CreatedAt], [UpdatedAt] FROM [dbo].[Enrollments]
ORDER BY [TableName], [Id];
GO

