-- ====================================================================================================
-- QUERY 19: Advanced Enrollment Filter
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Combine multiple dynamic query parameters safely (e.g., trackId=1, status=Active, paymentStatus=Paid).
-- Route: GET /api/enrollments?trackId=1&status=Active&paymentStatus=Paid
-- EF Core Concept: Conditional IQueryable composition (only apply filter when parameter has value).
-- Expected Behavior: Safely combine filters, server-side pagination, DTO projection.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 19 - Advanced Enrollment Dynamic Filter (TrackId=1 AND Status=Active AND PaymentStatus=Paid)';
PRINT '==============================================================================================';

DECLARE @FilterTrackId INT = 1;
DECLARE @FilterStatus NVARCHAR(30) = 'Active';
DECLARE @FilterPaymentStatus NVARCHAR(30) = 'Completed';

SELECT 
    e.EnrollmentId,
    s.StudentId,
    s.FullName AS StudentName,
    s.Email AS StudentEmail,
    t.TrainingTrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Price AS TrackPrice,
    e.EnrollmentDate,
    e.Status AS EnrollmentStatus,
    e.ProgressPercentage,
    e.FinalResult,
    CAST(COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) AS DECIMAL(18,2)) AS TotalPaid
FROM dbo.Enrollments e
INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId AND p.IsDeleted = 0
WHERE e.IsDeleted = 0
  AND (@FilterTrackId IS NULL OR e.TrainingTrackId = @FilterTrackId)
  AND (@FilterStatus IS NULL OR e.Status = @FilterStatus)
  AND (
      @FilterPaymentStatus IS NULL 
      OR (@FilterPaymentStatus = 'Completed' AND EXISTS (SELECT 1 FROM dbo.Payments p2 WHERE p2.EnrollmentId = e.EnrollmentId AND p2.PaymentStatus = 'Completed'))
      OR (@FilterPaymentStatus = 'Pending' AND EXISTS (SELECT 1 FROM dbo.Payments p2 WHERE p2.EnrollmentId = e.EnrollmentId AND p2.PaymentStatus = 'Pending'))
      OR (@FilterPaymentStatus = 'Failed' AND EXISTS (SELECT 1 FROM dbo.Payments p2 WHERE p2.EnrollmentId = e.EnrollmentId AND p2.PaymentStatus = 'Failed'))
  )
GROUP BY e.EnrollmentId, s.StudentId, s.FullName, s.Email, t.TrainingTrackId, t.Code, t.Title, t.Price, e.EnrollmentDate, e.Status, e.ProgressPercentage, e.FinalResult
ORDER BY e.EnrollmentDate DESC;
GO
