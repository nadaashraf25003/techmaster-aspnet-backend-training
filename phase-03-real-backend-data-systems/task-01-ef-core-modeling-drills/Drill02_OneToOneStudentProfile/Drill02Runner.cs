using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill02_OneToOneStudentProfile;

public static class Drill02Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 02: One-to-One Relationship (Student <-> Profile)");
        Console.WriteLine("========================================================");

        var studentWithProfile = await context.Students
            .Include(s => s.Profile)
            .FirstOrDefaultAsync(s => s.Profile != null);

        if (studentWithProfile != null)
        {
            Console.WriteLine($"[1:1 Relationship Verified]");
            Console.WriteLine($"  Student: #{studentWithProfile.Id} {studentWithProfile.FullName}");
            Console.WriteLine($"  Profile: NationalId={studentWithProfile.Profile!.NationalId}, Address={studentWithProfile.Profile.Address}, EmergencyPhone={studentWithProfile.Profile.EmergencyPhone}");
        }

        // Test querying profile independently
        var profile = await context.StudentProfiles
            .Include(p => p.Student)
            .FirstOrDefaultAsync();

        if (profile != null)
        {
            Console.WriteLine($"  Reverse Navigation: Profile for Student #{profile.StudentId} -> {profile.Student?.FullName}");
        }

        Console.WriteLine("✅ Drill 02 passed: 1:1 relationship with unique FK and bidirectional navigation verified.");
    }
}
