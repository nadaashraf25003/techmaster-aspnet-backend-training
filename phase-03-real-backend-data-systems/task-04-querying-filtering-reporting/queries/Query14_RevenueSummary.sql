-- ====================================================================================================
-- QUERY 14: Revenue Summary
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Return total realized revenue, expected book revenue, completed/paid count, pending count,
--           failed count, and average payment amount. Avoid integer money values.
-- Route: GET /api/reports/revenue-summary
-- EF Core Concept: Sum + Count + aggregations (.SumAsync(), .CountAsync()) using decimal precision.
-- Expected Behavior: Use decimal(18,2) and avoid integer money values.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 14 - Revenue Summary (Realized, Expected, Paid/Pending/Failed Counts)';
PRINT '==============================================================================================';

SELECT 
    CAST(COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) AS DECIMAL(18,2)) AS TotalRealizedRevenue,
    CAST(COALESCE((SELECT SUM(t.Price) 
                   FROM dbo.Enrollments e 
                   INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId 
                   WHERE e.Status <> 'Cancelled' AND e.IsDeleted = 0), 0.00) AS DECIMAL(18,2)) AS TotalExpectedRevenue,
    COUNT(CASE WHEN p.PaymentStatus = 'Completed' THEN 1 END) AS PaidCount,
    COUNT(CASE WHEN p.PaymentStatus = 'Pending' THEN 1 END) AS PendingCount,
    COUNT(CASE WHEN p.PaymentStatus = 'Failed' THEN 1 END) AS FailedCount,
    COUNT(p.PaymentId) AS TotalTransactionsCount,
    CAST(COALESCE(AVG(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount END), 0.00) AS DECIMAL(18,2)) AS AveragePaymentAmount
FROM dbo.Payments p
WHERE p.IsDeleted = 0;
GO
