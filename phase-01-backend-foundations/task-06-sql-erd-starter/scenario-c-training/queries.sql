-- ==========================================================
-- Scenario C: Training Center Registration System - Required Queries
-- ==========================================================

-- 1. Select all students
SELECT StudentId, FullName, Email, PhoneNumber, CreatedAt 
FROM Students;

-- 2. Select all tracks
SELECT TrackId, Title, DurationWeeks, StartDate, InstructorId 
FROM Tracks;

-- 3. Select students registered in a specific track (Assuming TrackId = 2)
SELECT s.StudentId, s.FullName, s.Email, r.RegistrationDate, r.Status
FROM Students s
INNER JOIN Registrations r ON s.StudentId = r.StudentId
WHERE r.TrackId = 2;

-- 4. Count students per track
SELECT t.TrackId, t.Title AS TrackTitle, COUNT(r.RegistrationId) AS RegisteredStudentsCount
FROM Tracks t
LEFT JOIN Registrations r ON t.TrackId = r.TrackId
GROUP BY t.TrackId, t.Title;

-- 5. Select unpaid registrations
SELECT r.RegistrationId, s.FullName AS StudentName, t.Title AS TrackTitle, r.RegistrationDate
FROM Registrations r
INNER JOIN Students s ON r.StudentId = s.StudentId
INNER JOIN Tracks t ON r.TrackId = t.TrackId
LEFT JOIN Payments p ON r.RegistrationId = p.RegistrationId
WHERE p.PaymentId IS NULL OR p.PaymentStatus = 'Unpaid';

-- 6. Select tracks by instructor (Assuming InstructorId = 1)
SELECT TrackId, Title, DurationWeeks, StartDate 
FROM Tracks 
WHERE InstructorId = 1;

-- 7. Select registrations with payment status using JOIN
SELECT r.RegistrationId, s.FullName AS StudentName, t.Title AS TrackTitle, 
       r.RegistrationDate, r.Status AS RegistrationStatus,
       COALESCE(p.PaymentStatus, 'Unpaid/No Record') AS PaymentStatus,
       COALESCE(p.Amount, 0.00) AS AmountPaid
FROM Registrations r
INNER JOIN Students s ON r.StudentId = s.StudentId
INNER JOIN Tracks t ON r.TrackId = t.TrackId
LEFT JOIN Payments p ON r.RegistrationId = p.RegistrationId;

-- 8. Select tracks starting after a specific date
SELECT TrackId, Title, StartDate 
FROM Tracks 
WHERE StartDate > '2026-09-01';

-- 9. Count tracks per instructor
SELECT i.InstructorId, i.FullName AS InstructorName, COUNT(t.TrackId) AS TrackCount
FROM Instructors i
LEFT JOIN Tracks t ON i.InstructorId = t.InstructorId
GROUP BY i.InstructorId, i.FullName;

-- 10. Select student registration history (Assuming StudentId = 1)
SELECT r.RegistrationId, t.Title AS TrackTitle, r.RegistrationDate, r.Status AS RegistrationStatus,
       i.FullName AS InstructorName
FROM Registrations r
INNER JOIN Tracks t ON r.TrackId = t.TrackId
INNER JOIN Instructors i ON t.InstructorId = i.InstructorId
WHERE r.StudentId = 1
ORDER BY r.RegistrationDate DESC;
