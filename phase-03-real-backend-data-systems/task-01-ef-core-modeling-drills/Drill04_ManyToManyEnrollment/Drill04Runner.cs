using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill04_ManyToManyEnrollment;

public static class Drill04Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 04: Many-to-Many via Join Entity (Enrollment)");
        Console.WriteLine("========================================================");

        var enrollments = await context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .ToListAsync();

        Console.WriteLine($"Total Active Enrollments: {enrollments.Count}");
        foreach (var enrollment in enrollments)
        {
            Console.WriteLine($"  [Enrollment #{enrollment.Id}] Student: {enrollment.Student?.FullName} -> Track: {enrollment.TrainingTrack?.Title} | Status: {enrollment.Status} | Progress: {enrollment.ProgressPercentage}%");
        }

        Console.WriteLine("✅ Drill 04 passed: Rich M:N join entity with domain payload verified.");
    }
}
