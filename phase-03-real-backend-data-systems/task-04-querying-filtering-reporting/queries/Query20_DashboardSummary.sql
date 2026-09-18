-- ====================================================================================================
-- QUERY 20: Dashboard Summary
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Return high-level system numbers in one unified response.
-- Route: GET /api/reports/dashboard-summary
-- EF Core Concept: Multiple aggregate queries consolidated into one result DTO.
-- Expected Behavior: Include studentsCount, tracksCount, activeEnrollments, revenue, unpaidCount.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 20 - Dashboard High-Level Metrics Summary';
PRINT '==============================================================================================';

SELECT 
    (SELECT COUNT(*) FROM dbo.Students WHERE IsDeleted = 0) AS StudentsCount,
    (SELECT COUNT(*) FROM dbo.Students WHERE IsDeleted = 0 AND IsActive = 1) AS ActiveStudentsCount,
    (SELECT COUNT(*) FROM dbo.Instructors WHERE IsDeleted = 0) AS InstructorsCount,
    (SELECT COUNT(*) FROM dbo.TrainingTracks WHERE IsDeleted = 0) AS TracksCount,
    (SELECT COUNT(*) FROM dbo.TrainingTracks WHERE IsDeleted = 0 AND Status IN ('InProgress', 'Upcoming')) AS ActiveTracksCount,
    (SELECT COUNT(*) FROM dbo.Enrollments WHERE IsDeleted = 0) AS TotalEnrollmentsCount,
    (SELECT COUNT(*) FROM dbo.Enrollments WHERE IsDeleted = 0 AND Status = 'Active') AS ActiveEnrollments,
    (SELECT CAST(COALESCE(SUM(Amount), 0.00) AS DECIMAL(18,2)) FROM dbo.Payments WHERE IsDeleted = 0 AND PaymentStatus = 'Completed') AS RealizedRevenue,
    (SELECT CAST(COALESCE(SUM(t.Price), 0.00) AS DECIMAL(18,2)) FROM dbo.Enrollments e INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId WHERE e.IsDeleted = 0 AND e.Status <> 'Cancelled') AS ExpectedRevenue,
    (SELECT COUNT(*) FROM dbo.Payments WHERE IsDeleted = 0 AND PaymentStatus = 'Completed') AS PaidCount,
    (SELECT COUNT(*) FROM dbo.Enrollments e 
     INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId 
     WHERE e.IsDeleted = 0 
       AND e.Status IN ('Active', 'Pending') 
       AND (SELECT COALESCE(SUM(p.Amount), 0.00) FROM dbo.Payments p WHERE p.EnrollmentId = e.EnrollmentId AND p.PaymentStatus = 'Completed' AND p.IsDeleted = 0) < t.Price) AS UnpaidCount;
GO
