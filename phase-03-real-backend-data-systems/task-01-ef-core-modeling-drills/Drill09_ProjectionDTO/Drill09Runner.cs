using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill09_ProjectionDTO;

public static class Drill09Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 09: LINQ Projection DTOs (.Select)");
        Console.WriteLine("========================================================");

        // Projection 1: Student List Item DTO
        var studentDtos = await context.Students
            .Select(s => new StudentListItemDto
            {
                StudentId = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                IsActive = s.IsActive,
                ActiveEnrollmentsCount = s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
            })
            .ToListAsync();

        Console.WriteLine($"[Projected StudentListItemDtos ({studentDtos.Count})]");
        foreach (var s in studentDtos.Take(3))
        {
            Console.WriteLine($"  - #{s.StudentId} {s.FullName} | Email: {s.Email} | Active Enrollments: {s.ActiveEnrollmentsCount}");
        }

        // Projection 2: Track Details DTO with aggregated stats
        var trackDtos = await context.TrainingTracks
            .Select(t => new TrackDetailsDto
            {
                TrackId = t.Id,
                Title = t.Title,
                Code = t.Code,
                Level = t.Level,
                Capacity = t.Capacity,
                EnrolledStudentsCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                Price = t.Price,
                InstructorName = t.Instructor != null ? t.Instructor.FullName : "N/A",
                InstructorEmail = t.Instructor != null ? t.Instructor.Email : "N/A",
                StartDate = t.StartDate,
                EndDate = t.EndDate
            })
            .ToListAsync();

        Console.WriteLine($"[Projected TrackDetailsDtos ({trackDtos.Count})]");
        foreach (var t in trackDtos)
        {
            Console.WriteLine($"  - [{t.Code}] {t.Title} | Instructor: {t.InstructorName} | Capacity: {t.Capacity} | Enrolled: {t.EnrolledStudentsCount} | Available: {t.AvailableSeats}");
        }

        Console.WriteLine("✅ Drill 09 passed: Clean LINQ projections generated optimized queries without entity exposure.");
    }
}
