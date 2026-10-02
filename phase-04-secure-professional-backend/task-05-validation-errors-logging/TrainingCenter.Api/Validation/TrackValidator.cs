using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Validation;

public static class TrackValidator
{
    public static void ValidateCreateTrack(CreateTrackRequest request, Instructor instructor)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors.Add("Track title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            errors.Add("Track code is required.");
        }

        if (request.Price < 0)
        {
            errors.Add("Track price cannot be negative.");
        }

        if (request.Capacity <= 0)
        {
            errors.Add("Track capacity must be strictly greater than 0.");
        }

        if (request.DurationHours <= 0)
        {
            errors.Add("Duration hours must be greater than 0.");
        }

        if (request.StartDate >= request.EndDate)
        {
            errors.Add($"Invalid track dates: StartDate ({request.StartDate:yyyy-MM-dd}) must be before EndDate ({request.EndDate:yyyy-MM-dd}).");
        }

        if (!instructor.IsActive)
        {
            errors.Add($"Cannot assign instructor '{instructor.FullName}' because their account is inactive.");
        }

        if (errors.Any())
        {
            throw new ValidationException("Track validation failed.", errors);
        }
    }
}
