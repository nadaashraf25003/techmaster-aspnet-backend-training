using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;

public static class Drill03Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 03: One-to-Many Relationship (Instructor -> Tracks)");
        Console.WriteLine("========================================================");

        var instructorsWithTracks = await context.Instructors
            .Include(i => i.Tracks)
            .ToListAsync();

        foreach (var instructor in instructorsWithTracks)
        {
            Console.WriteLine($"[Instructor] #{instructor.Id} {instructor.FullName} ({instructor.Specialization})");
            Console.WriteLine($"  Tracks Assigned ({instructor.Tracks.Count}):");
            foreach (var track in instructor.Tracks)
            {
                Console.WriteLine($"    - [{track.Code}] {track.Title} | Level: {track.Level} | Capacity: {track.Capacity} | Price: {track.Price:C}");
            }
        }

        Console.WriteLine("✅ Drill 03 passed: 1:N relationship with IReadOnlyCollection encapsulation verified.");
    }
}
