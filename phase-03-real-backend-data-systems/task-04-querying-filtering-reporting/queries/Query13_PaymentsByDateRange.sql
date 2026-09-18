-- ====================================================================================================
-- QUERY 13: Payments By Date Range
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Return all payments inside a specified date range (from <= to) with student and track details.
-- Route: GET /api/payments?from=2026-07-01&to=2026-07-31
-- EF Core Concept: Where PaymentDate between from/to (query.Where(p => p.PaymentDate >= from && p.PaymentDate <= to))
-- Expected Behavior: Validate from <= to (return 400 Bad Request on invalid input).
-- Index Optimization: Nonclustered index on Payments(PaymentDate) INCLUDE (Amount, PaymentStatus, EnrollmentId)
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 13 - Payments By Date Range (e.g., 2026-07-01 to 2026-07-31)';
PRINT '==============================================================================================';

DECLARE @FromDate DATETIME2 = '2026-07-01 00:00:00';
DECLARE @ToDate DATETIME2   = '2026-07-31 23:59:59';

-- Validation: Validate from <= to
IF @FromDate > @ToDate
BEGIN
    RAISERROR('Validation Error: @FromDate must be less than or equal to @ToDate.', 16, 1);
END
ELSE
BEGIN
    SELECT 
        p.PaymentId,
        p.EnrollmentId,
        s.FullName AS StudentName,
        s.Email AS StudentEmail,
        t.Title AS TrackTitle,
        p.Amount,
        p.PaymentMethod,
        p.PaymentDate,
        p.PaymentStatus,
        p.ReferenceNumber,
        p.Notes,
        p.CreatedAt
    FROM dbo.Payments p
    INNER JOIN dbo.Enrollments e ON p.EnrollmentId = e.EnrollmentId
    INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
    INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
    WHERE p.IsDeleted = 0
      AND e.IsDeleted = 0
      AND p.PaymentDate >= @FromDate 
      AND p.PaymentDate <= @ToDate
    ORDER BY p.PaymentDate DESC;
END
GO
