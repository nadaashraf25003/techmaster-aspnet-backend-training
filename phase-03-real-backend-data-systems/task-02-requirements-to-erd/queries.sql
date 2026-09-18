-- ====================================================================================================
-- Phase 03: Task 02 - Requirements to ERD: Business Intelligence & Reporting Query Pack
-- Database: TechMasterAcademyDb (Microsoft SQL Server)
-- ====================================================================================================

USE [TechMasterAcademyDb];
GO

PRINT '==============================================================================================';
PRINT 'TECHMASTER ACADEMY - 10 CORE BUSINESS REPORTING QUERIES';
PRINT '==============================================================================================';
GO

-- ----------------------------------------------------------------------------------------------------
-- 1. Which students are enrolled in a specific track?
-- Target: 'NET-BE-2026' (ASP.NET Core Enterprise Backend BootCamp)
-- ----------------------------------------------------------------------------------------------------
PRINT '1. Students Enrolled in Track (NET-BE-2026):';

SELECT 
    s.StudentId,
    s.FullName AS StudentName,
    s.Email AS StudentEmail,
    s.PhoneNumber,
    e.EnrollmentId,
    e.EnrollmentDate,
    e.Status AS EnrollmentStatus,
    e.ProgressPercentage,
    t.Code AS TrackCode,
    t.Title AS TrackTitle
FROM dbo.Enrollments e
INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
WHERE t.Code = 'NET-BE-2026' 
  AND s.IsDeleted = 0
ORDER BY s.FullName ASC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 2. Which tracks have available seats?
-- (Calculates total active/pending enrollments vs track capacity)
-- ----------------------------------------------------------------------------------------------------
PRINT '2. Tracks with Available Seats:';

SELECT 
    t.TrainingTrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Capacity AS MaxCapacity,
    COUNT(e.EnrollmentId) AS CurrentEnrolled,
    (t.Capacity - COUNT(e.EnrollmentId)) AS AvailableSeats,
    CASE 
        WHEN COUNT(e.EnrollmentId) = 0 THEN 'Empty'
        WHEN (t.Capacity - COUNT(e.EnrollmentId)) > 0 THEN 'Open Seats'
        ELSE 'Full'
    END AS SeatStatus
FROM dbo.TrainingTracks t
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId 
    AND e.Status IN ('Active', 'Pending')
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Code, t.Title, t.Capacity
HAVING COUNT(e.EnrollmentId) < t.Capacity
ORDER BY AvailableSeats DESC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 3. Which enrollments are unpaid or partially paid?
-- (Compares Track Price against sum of completed payments for that enrollment)
-- ----------------------------------------------------------------------------------------------------
PRINT '3. Unpaid or Partially Paid Enrollments:';

SELECT 
    e.EnrollmentId,
    s.StudentId,
    s.FullName AS StudentName,
    s.Email AS StudentEmail,
    s.PhoneNumber,
    t.Title AS TrackTitle,
    t.Price AS TrackPrice,
    COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00) AS TotalPaid,
    (t.Price - COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00)) AS OutstandingBalance,
    CASE 
        WHEN COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00) = 0.00 THEN 'Completely Unpaid'
        ELSE 'Partially Paid'
    END AS FinancialStatus
FROM dbo.Enrollments e
INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId
WHERE e.Status IN ('Active', 'Pending')
GROUP BY e.EnrollmentId, s.StudentId, s.FullName, s.Email, s.PhoneNumber, t.Title, t.Price
HAVING COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00) < t.Price
ORDER BY OutstandingBalance DESC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 4. How much revenue did each track generate?
-- (Calculates total realized cash collected vs expected book value)
-- ----------------------------------------------------------------------------------------------------
PRINT '4. Revenue by Training Track:';

SELECT 
    t.TrainingTrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Price AS UnitPrice,
    COUNT(DISTINCT e.EnrollmentId) AS TotalEnrolledCount,
    (COUNT(DISTINCT e.EnrollmentId) * t.Price) AS TotalExpectedTuition,
    COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00) AS RealizedRevenue,
    ((COUNT(DISTINCT e.EnrollmentId) * t.Price) - COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00)) AS PendingRevenue
FROM dbo.TrainingTracks t
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Code, t.Title, t.Price
ORDER BY RealizedRevenue DESC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 5. Which instructor has the highest workload?
-- (Analyzes active/upcoming tracks and active student enrollments per mentor)
-- ----------------------------------------------------------------------------------------------------
PRINT '5. Instructor Workload & Mentorship Metric:';

SELECT 
    i.InstructorId,
    i.FullName AS InstructorName,
    i.Email AS InstructorEmail,
    i.Specialization,
    COUNT(DISTINCT CASE WHEN t.Status IN ('Upcoming', 'InProgress') AND t.IsDeleted = 0 THEN t.TrainingTrackId END) AS ActiveTracksAssigned,
    COUNT(CASE WHEN e.Status = 'Active' THEN e.EnrollmentId END) AS TotalActiveStudentsSupervised,
    COALESCE(SUM(CASE WHEN p.PaymentStatus = 'Completed' THEN p.Amount ELSE 0 END), 0.00) AS TotalRevenueGenerated
FROM dbo.Instructors i
LEFT JOIN dbo.TrainingTracks t ON i.InstructorId = t.InstructorId AND t.IsDeleted = 0
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId
LEFT JOIN dbo.Payments p ON e.EnrollmentId = p.EnrollmentId
WHERE i.IsActive = 1
GROUP BY i.InstructorId, i.FullName, i.Email, i.Specialization
ORDER BY TotalActiveStudentsSupervised DESC, ActiveTracksAssigned DESC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 6. Which students have active enrollments?
-- ----------------------------------------------------------------------------------------------------
PRINT '6. Active Students with Ongoing Tracks:';

SELECT 
    s.StudentId,
    s.FullName AS StudentName,
    s.Email,
    s.PhoneNumber,
    COUNT(e.EnrollmentId) AS ActiveEnrollmentsCount,
    STRING_AGG(t.Code, ', ') AS EnrolledTrackCodes
FROM dbo.Students s
INNER JOIN dbo.Enrollments e ON s.StudentId = e.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
WHERE s.IsActive = 1 
  AND s.IsDeleted = 0 
  AND e.Status = 'Active'
GROUP BY s.StudentId, s.FullName, s.Email, s.PhoneNumber
ORDER BY ActiveEnrollmentsCount DESC, s.FullName ASC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 7. Which tracks start this month (e.g. September 2026)?
-- ----------------------------------------------------------------------------------------------------
PRINT '7. Tracks Launching in the Current Month:';

SELECT 
    t.TrainingTrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Level,
    t.Capacity,
    t.StartDate,
    t.EndDate,
    t.Status,
    i.FullName AS InstructorName
FROM dbo.TrainingTracks t
INNER JOIN dbo.Instructors i ON t.InstructorId = i.InstructorId
WHERE t.IsDeleted = 0
  AND YEAR(t.StartDate) = YEAR(GETUTCDATE())
  AND MONTH(t.StartDate) = MONTH(GETUTCDATE())
ORDER BY t.StartDate ASC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 8. What is the payment history for an enrollment?
-- (Parameter: @TargetEnrollmentId = 1)
-- ----------------------------------------------------------------------------------------------------
PRINT '8. Payment History Ledger for Enrollment #1:';

DECLARE @TargetEnrollmentId INT = 1;

SELECT 
    p.PaymentId,
    p.ReferenceNumber,
    p.Amount,
    p.PaymentMethod,
    p.PaymentDate,
    p.PaymentStatus,
    p.Notes,
    s.FullName AS StudentName,
    t.Title AS TrackTitle
FROM dbo.Payments p
INNER JOIN dbo.Enrollments e ON p.EnrollmentId = e.EnrollmentId
INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
INNER JOIN dbo.TrainingTracks t ON e.TrainingTrackId = t.TrainingTrackId
WHERE p.EnrollmentId = @TargetEnrollmentId
ORDER BY p.PaymentDate DESC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 9. Which tracks are full (Capacity reached)?
-- ----------------------------------------------------------------------------------------------------
PRINT '9. Fully Booked Tracks (At Capacity):';

SELECT 
    t.TrainingTrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Capacity AS MaxCapacity,
    COUNT(e.EnrollmentId) AS TotalEnrolledStudents,
    t.Status AS TrackStatus
FROM dbo.TrainingTracks t
INNER JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId
WHERE t.IsDeleted = 0 
  AND e.Status IN ('Active', 'Pending')
GROUP BY t.TrainingTrackId, t.Code, t.Title, t.Capacity, t.Status
HAVING COUNT(e.EnrollmentId) >= t.Capacity
ORDER BY TotalEnrolledStudents DESC;
GO

-- ----------------------------------------------------------------------------------------------------
-- 10. How many enrollments exist by status?
-- ----------------------------------------------------------------------------------------------------
PRINT '10. Enrollment Distribution by Status:';

SELECT 
    e.Status AS EnrollmentStatus,
    COUNT(*) AS TotalCount,
    CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS PctOfTotal
FROM dbo.Enrollments e
GROUP BY e.Status
ORDER BY TotalCount DESC;
GO

-- ====================================================================================================
-- BONUS QUERIES: Attendance Rates & Assignment Performance
-- ====================================================================================================

PRINT 'BONUS A: Student Attendance Rate per Session:';
SELECT 
    ts.Title AS SessionTitle,
    ts.SessionDate,
    COUNT(a.AttendanceId) AS TotalMarked,
    SUM(CASE WHEN a.Status = 'Present' THEN 1 ELSE 0 END) AS PresentCount,
    SUM(CASE WHEN a.Status = 'Absent' THEN 1 ELSE 0 END) AS AbsentCount,
    SUM(CASE WHEN a.Status = 'Late' THEN 1 ELSE 0 END) AS LateCount
FROM dbo.TrackSessions ts
LEFT JOIN dbo.Attendances a ON ts.SessionId = a.TrackSessionId
GROUP BY ts.SessionId, ts.Title, ts.SessionDate
ORDER BY ts.SessionDate ASC;
GO

PRINT 'BONUS B: Assignment Grade Distribution:';
SELECT 
    a.Title AS AssignmentTitle,
    a.DueDate,
    COUNT(sub.SubmissionId) AS SubmissionsCount,
    AVG(sub.Score) AS AverageScore,
    MIN(sub.Score) AS MinScore,
    MAX(sub.Score) AS MaxScore
FROM dbo.Assignments a
LEFT JOIN dbo.AssignmentSubmissions sub ON a.AssignmentId = sub.AssignmentId
GROUP BY a.AssignmentId, a.Title, a.DueDate
ORDER BY a.DueDate ASC;
GO
