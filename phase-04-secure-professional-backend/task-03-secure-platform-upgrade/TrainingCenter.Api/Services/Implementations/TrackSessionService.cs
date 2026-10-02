using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Sessions;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class TrackSessionService : ITrackSessionService
{
    private readonly TrainingCenterDbContext _context;

    public TrackSessionService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrackSessionResponse>> GetSessionsByTrackAsync(int trackId, int? requestingInstructorId, bool isAdmin)
    {
        var track = await _context.TrainingTracks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TrainingTrackId == trackId && !t.IsDeleted);

        if (track == null)
        {
            throw new NotFoundException($"Training track with ID {trackId} was not found.");
        }

        if (!isAdmin)
        {
            if (!requestingInstructorId.HasValue || track.PrimaryInstructorId != requestingInstructorId.Value)
            {
                throw new ForbiddenException("Access denied. You can only view sessions for tracks assigned to you.");
            }
        }

        var sessions = await _context.TrackSessions
            .AsNoTracking()
            .Include(s => s.CreatedByInstructor)
            .Where(s => s.TrainingTrackId == trackId && !s.IsDeleted)
            .OrderBy(s => s.SessionDate)
            .Select(s => new TrackSessionResponse
            {
                TrackSessionId = s.TrackSessionId,
                TrainingTrackId = s.TrainingTrackId,
                TrackTitle = track.Title,
                SessionDate = s.SessionDate,
                Title = s.Title,
                Description = s.Description,
                MeetingLink = s.MeetingLink,
                IsCompleted = s.IsCompleted,
                Notes = s.Notes,
                CreatedByInstructorId = s.CreatedByInstructorId,
                CreatedByInstructorName = s.CreatedByInstructor != null ? s.CreatedByInstructor.FullName : string.Empty,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return sessions;
    }

    public async Task<TrackSessionResponse> GetSessionByIdAsync(int sessionId, int? requestingInstructorId, bool isAdmin)
    {
        var session = await _context.TrackSessions
            .AsNoTracking()
            .Include(s => s.TrainingTrack)
            .Include(s => s.CreatedByInstructor)
            .FirstOrDefaultAsync(s => s.TrackSessionId == sessionId && !s.IsDeleted);

        if (session == null)
        {
            throw new NotFoundException($"Track session with ID {sessionId} was not found.");
        }

        if (!isAdmin)
        {
            if (!requestingInstructorId.HasValue || 
                (session.TrainingTrack.PrimaryInstructorId != requestingInstructorId.Value && 
                 session.CreatedByInstructorId != requestingInstructorId.Value))
            {
                throw new ForbiddenException("Access denied. You can only view sessions for tracks assigned to you.");
            }
        }

        return new TrackSessionResponse
        {
            TrackSessionId = session.TrackSessionId,
            TrainingTrackId = session.TrainingTrackId,
            TrackTitle = session.TrainingTrack != null ? session.TrainingTrack.Title : string.Empty,
            SessionDate = session.SessionDate,
            Title = session.Title,
            Description = session.Description,
            MeetingLink = session.MeetingLink,
            IsCompleted = session.IsCompleted,
            Notes = session.Notes,
            CreatedByInstructorId = session.CreatedByInstructorId,
            CreatedByInstructorName = session.CreatedByInstructor != null ? session.CreatedByInstructor.FullName : string.Empty,
            CreatedAt = session.CreatedAt
        };
    }

    public async Task<TrackSessionResponse> CreateSessionAsync(int trackId, int instructorId, CreateTrackSessionRequest request)
    {
        var track = await _context.TrainingTracks
            .FirstOrDefaultAsync(t => t.TrainingTrackId == trackId && !t.IsDeleted);

        if (track == null)
        {
            throw new NotFoundException($"Training track with ID {trackId} was not found.");
        }

        if (track.PrimaryInstructorId != instructorId)
        {
            throw new ForbiddenException("Access denied. You can only create sessions for tracks assigned to you.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new BadRequestException("Session title is required.");
        }

        var session = new TrackSession
        {
            TrainingTrackId = trackId,
            SessionDate = request.SessionDate,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            MeetingLink = request.MeetingLink?.Trim() ?? string.Empty,
            Notes = request.Notes?.Trim(),
            IsCompleted = false,
            CreatedByInstructorId = instructorId
        };

        _context.TrackSessions.Add(session);
        await _context.SaveChangesAsync();

        return await GetSessionByIdAsync(session.TrackSessionId, instructorId, isAdmin: true);
    }

    public async Task<TrackSessionResponse> UpdateSessionAsync(int sessionId, int instructorId, UpdateTrackSessionRequest request)
    {
        var session = await _context.TrackSessions
            .Include(s => s.TrainingTrack)
            .FirstOrDefaultAsync(s => s.TrackSessionId == sessionId && !s.IsDeleted);

        if (session == null)
        {
            throw new NotFoundException($"Track session with ID {sessionId} was not found.");
        }

        if (session.CreatedByInstructorId != instructorId && session.TrainingTrack.PrimaryInstructorId != instructorId)
        {
            throw new ForbiddenException("Access denied. You can only update sessions for tracks assigned to you.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new BadRequestException("Session title is required.");
        }

        session.SessionDate = request.SessionDate;
        session.Title = request.Title.Trim();
        session.Description = request.Description?.Trim() ?? string.Empty;
        session.MeetingLink = request.MeetingLink?.Trim() ?? string.Empty;
        session.IsCompleted = request.IsCompleted;
        session.Notes = request.Notes?.Trim();

        await _context.SaveChangesAsync();

        return await GetSessionByIdAsync(sessionId, instructorId, isAdmin: true);
    }

    public async Task DeleteSessionAsync(int sessionId, int instructorId, bool isAdmin)
    {
        var session = await _context.TrackSessions
            .Include(s => s.TrainingTrack)
            .FirstOrDefaultAsync(s => s.TrackSessionId == sessionId && !s.IsDeleted);

        if (session == null)
        {
            throw new NotFoundException($"Track session with ID {sessionId} was not found.");
        }

        if (!isAdmin && session.CreatedByInstructorId != instructorId && session.TrainingTrack.PrimaryInstructorId != instructorId)
        {
            throw new ForbiddenException("Access denied. You can only delete sessions for tracks assigned to you.");
        }

        session.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
