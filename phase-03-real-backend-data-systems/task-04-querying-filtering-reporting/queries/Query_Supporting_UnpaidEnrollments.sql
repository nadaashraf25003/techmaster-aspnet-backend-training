-- ====================================================================================================
-- SUPPORTING REPORT: Unpaid or Partially Paid Enrollments
-- Phase 03: Task 04 • TechMaster Academy
-- Route: GET /api/reports/unpaid-enrollments
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Supporting Report: Unpaid or Partially Paid Enrollments';
PRINT '==============================================================================================';

SELECT 
    e.EnrollmentId,
    s.StudentId,
    s.FullName AS StudentName,
    s.Email AS StudentEmail,
    s.PhoneNumber,
    t.TrainingTrackId AS TrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Price AS TrackPrice,
    COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) AS TotalPaid,
    (t.Price - COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00)) AS OutstandingBalance,
    CASE 
        WHEN COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) = 0.00 THEN 'Completely Unpaid'
        ELSE 'Partially Paid'
    END AS PaymentStatusSummary
FROM dbo.Enrollments e
INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId AND p.IsDeleted = 0
WHERE e.IsDeleted = 0
  AND e.Status IN ('Active', 'Pending')
GROUP BY e.EnrollmentId, s.StudentId, s.FullName, s.Email, s.PhoneNumber, t.TrainingTrackId, t.Code, t.Title, t.Price
HAVING COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0.00 END), 0.00) < t.Price
ORDER BY OutstandingBalance DESC;
GO
