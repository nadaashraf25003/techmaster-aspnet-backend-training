using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Validation;

public static class EnrollmentValidator
{
    public static void ValidateEnrollmentEligibility(Student student, TrainingTrack track, int currentActiveOrPendingCount)
    {
        var errors = new List<string>();

        if (!student.IsActive)
        {
            errors.Add($"Cannot enroll student '{student.FullName}' because their account is inactive.");
        }

        if (track.Status is TrackStatus.Closed or TrackStatus.Completed or TrackStatus.Archived or TrackStatus.Draft)
        {
            errors.Add($"Cannot enroll in track '{track.Title}' with status '{track.Status}'. Only Upcoming or InProgress tracks accept enrollments.");
        }

        if (currentActiveOrPendingCount >= track.Capacity)
        {
            errors.Add($"Track '{track.Title}' has reached its maximum capacity of {track.Capacity} students.");
        }

        if (errors.Any())
        {
            throw new ValidationException("Enrollment eligibility validation failed.", errors);
        }
    }
}
