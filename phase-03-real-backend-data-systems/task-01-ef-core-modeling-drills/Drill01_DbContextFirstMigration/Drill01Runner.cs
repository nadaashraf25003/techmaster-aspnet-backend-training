using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill01_DbContextFirstMigration;

public static class Drill01Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 01: DbContext, DbSet & First Migration Mapping");
        Console.WriteLine("========================================================");

        var studentsCount = await context.Students.CountAsync();
        Console.WriteLine($"[Schema Check] Students DbSet active. Total Students in DB: {studentsCount}");

        var firstStudent = await context.Students.FirstOrDefaultAsync();
        if (firstStudent != null)
        {
            Console.WriteLine($"[Entity Verified] Id: {firstStudent.Id} | Name: {firstStudent.FullName} | Email: {firstStudent.Email} | CreatedAt: {firstStudent.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
        }
        Console.WriteLine("✅ Drill 01 passed: DbContext and Student entity successfully mapped.");
    }
}
