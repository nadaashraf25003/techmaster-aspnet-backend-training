-- ====================================================================================================
-- QUERY 15: Revenue Per Track
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Group paid payments by training track and return track title, total paid amount, and enrollment count.
-- Route: GET /api/reports/revenue-by-track
-- EF Core Concept: GroupBy TrackId / Title or navigation collection projection with SUM.
-- Expected Behavior: Return track title, unit price, total paid, enrollment count, and expected revenue.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 15 - Revenue Per Track (Grouped by Track with Realized Cash)';
PRINT '==============================================================================================';

SELECT 
    t.TrainingTrackId AS TrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Price AS UnitPrice,
    COUNT(DISTINCT CASE WHEN e.Status <> 'Cancelled' THEN e.EnrollmentId END) AS EnrollmentCount,
    CAST(COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) AS DECIMAL(18,2)) AS TotalPaid,
    CAST(COUNT(DISTINCT CASE WHEN e.Status <> 'Cancelled' THEN e.EnrollmentId END) * t.Price AS DECIMAL(18,2)) AS ExpectedRevenue
FROM dbo.TrainingTracks t
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId AND e.IsDeleted = 0
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId AND p.PaymentStatus = 'Completed' AND p.IsDeleted = 0
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Code, t.Title, t.Price
ORDER BY TotalPaid DESC;
GO
