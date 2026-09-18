-- ====================================================================================================
-- QUERY 16: Top Tracks By Enrollment
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Return tracks ordered by active enrollment count (top 5 by default), including capacity and available seats.
-- Route: GET /api/reports/top-tracks?count=5
-- EF Core Concept: GroupBy + OrderByDescending (.OrderByDescending(t => t.Enrollments.Count(e => e.Status == Active)).Take(5))
-- Expected Behavior: Return top 5 by default with capacity, available seats, and utilization percentage.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 16 - Top Tracks By Enrollment (Top 5 Active Tracks)';
PRINT '==============================================================================================';

SELECT TOP (5)
    t.TrainingTrackId AS TrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Capacity,
    COUNT(CASE WHEN e.Status = 'Active' THEN 1 END) AS ActiveEnrollmentCount,
    COUNT(CASE WHEN e.Status <> 'Cancelled' THEN 1 END) AS TotalEnrollmentCount,
    (t.Capacity - COUNT(CASE WHEN e.Status IN ('Active', 'Pending') THEN 1 END)) AS AvailableSeats,
    CAST(ROUND(CASE WHEN t.Capacity > 0 
               THEN (CAST(COUNT(CASE WHEN e.Status = 'Active' THEN 1 END) AS DECIMAL(18,2)) / t.Capacity) * 100.00 
               ELSE 0.00 END, 2) AS DECIMAL(5,2)) AS UtilizationPercentage,
    COALESCE(i.FullName, 'Unassigned') AS InstructorName
FROM dbo.TrainingTracks t
LEFT JOIN dbo.Instructors i ON t.PrimaryInstructorId = i.InstructorId AND i.IsDeleted = 0
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId AND e.IsDeleted = 0
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Code, t.Title, t.Capacity, i.FullName
ORDER BY ActiveEnrollmentCount DESC, TotalEnrollmentCount DESC;
GO
