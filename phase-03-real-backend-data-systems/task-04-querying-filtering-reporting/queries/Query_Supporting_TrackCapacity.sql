-- ====================================================================================================
-- SUPPORTING REPORT: Track Capacity and Available Seats
-- Phase 03: Task 04 • TechMaster Academy
-- Route: GET /api/reports/track-capacity
-- ====================================================================================================

USE [TechMaster_Phase03_Task04_Db];
GO

PRINT '==============================================================================================';
PRINT 'Supporting Report: Track Capacity & Available Seats';
PRINT '==============================================================================================';

SELECT 
    t.TrainingTrackId AS TrackId,
    t.Code AS TrackCode,
    t.Title AS TrackTitle,
    t.Capacity,
    COUNT(CASE WHEN e.Status IN ('Active', 'Pending') THEN 1 END) AS EnrolledCount,
    CASE 
        WHEN (t.Capacity - COUNT(CASE WHEN e.Status IN ('Active', 'Pending') THEN 1 END)) < 0 THEN 0
        ELSE (t.Capacity - COUNT(CASE WHEN e.Status IN ('Active', 'Pending') THEN 1 END))
    END AS AvailableSeats,
    t.Status AS TrackStatus
FROM dbo.TrainingTracks t
LEFT JOIN dbo.Enrollments e ON t.TrainingTrackId = e.TrainingTrackId AND e.IsDeleted = 0
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Code, t.Title, t.Capacity, t.Status
ORDER BY EnrolledCount DESC;
GO
