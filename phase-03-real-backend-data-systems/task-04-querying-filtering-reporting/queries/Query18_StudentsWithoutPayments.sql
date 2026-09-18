-- ====================================================================================================
-- QUERY 18: Students Without Payments
-- Phase 03: Task 04 • TechMaster Academy
-- Scenario: Return students with Active or Pending enrollments who have ZERO completed payments.
--           STRICT REQUIREMENT: Must NOT include cancelled enrollments.
-- Route: GET /api/reports/students-without-payments
-- EF Core Concept: Any/All on related payments (.Where(e => (e.Status == Active || e.Status == Pending) && !e.Payments.Any(p => p.PaymentStatus == Completed)))
-- Expected Behavior: Must not include cancelled enrollments.
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Query 18 - Students with Active/Pending Enrollments but Zero Payments';
PRINT '==============================================================================================';

SELECT 
    s.StudentId,
    s.FullName AS StudentName,
    s.Email,
    s.PhoneNumber,
    e.EnrollmentId,
    e.Status AS EnrollmentStatus,
    e.EnrollmentDate,
    t.TrainingTrackId AS TrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Price AS TrackPrice,
    0.00 AS TotalPaid,
    t.Price AS OutstandingBalance
FROM dbo.Enrollments e
INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
WHERE e.IsDeleted = 0 
  AND s.IsDeleted = 0
  AND t.IsDeleted = 0
  AND e.Status IN ('Active', 'Pending') -- Strictly Active or Pending, never Cancelled
  AND NOT EXISTS (
      SELECT 1 
      FROM dbo.Payments p 
      WHERE p.EnrollmentId = e.EnrollmentId 
        AND p.PaymentStatus = 'Completed'
        AND p.IsDeleted = 0
  )
ORDER BY s.FullName ASC;
GO
