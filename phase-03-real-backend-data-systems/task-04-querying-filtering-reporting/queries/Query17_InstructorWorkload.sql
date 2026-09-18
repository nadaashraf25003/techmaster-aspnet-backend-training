-- ====================================================================================================
-- QUERY 17: Instructor Workload
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Return each instructor with number of tracks, active tracks, active students supervised,
--           and total revenue generated. Useful for management workload balancing and reporting.
-- Route: GET /api/reports/instructor-workload
-- EF Core Concept: Group by instructor / projection over navigation collections.
-- Expected Behavior: Useful for management reporting.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 17 - Instructor Workload & Student Distribution';
PRINT '==============================================================================================';

SELECT 
    i.InstructorId,
    i.FullName AS InstructorName,
    i.Email,
    i.Specialization,
    COUNT(DISTINCT t.TrainingTrackId) AS NumberOfTracks,
    COUNT(DISTINCT CASE WHEN t.Status IN ('InProgress', 'Upcoming') THEN t.TrainingTrackId END) AS ActiveTracksCount,
    COUNT(DISTINCT CASE WHEN e.Status = 'Active' THEN e.EnrollmentId END) AS ActiveStudentsCount,
    COUNT(DISTINCT CASE WHEN e.Status <> 'Cancelled' THEN e.EnrollmentId END) AS TotalStudentsSupervised,
    CAST(COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) AS DECIMAL(18,2)) AS TotalRevenueGenerated
FROM dbo.Instructors i
LEFT JOIN dbo.TrainingTracks t ON i.InstructorId = t.PrimaryInstructorId AND t.IsDeleted = 0
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId AND e.IsDeleted = 0
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId AND p.PaymentStatus = 'Completed' AND p.IsDeleted = 0
WHERE i.IsDeleted = 0 
  AND i.IsActive = 1
GROUP BY i.InstructorId, i.FullName, i.Email, i.Specialization
ORDER BY ActiveStudentsCount DESC, ActiveTracksCount DESC;
GO
