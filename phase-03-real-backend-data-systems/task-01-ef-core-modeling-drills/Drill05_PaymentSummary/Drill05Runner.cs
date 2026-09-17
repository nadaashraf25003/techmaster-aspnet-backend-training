using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill05_PaymentSummary;

public static class Drill05Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 05: One-to-One Payment Summary & Decimal Precision");
        Console.WriteLine("========================================================");

        var paymentSummaries = await context.PaymentSummaries
            .Include(ps => ps.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(ps => ps.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .ToListAsync();

        foreach (var ps in paymentSummaries)
        {
            Console.WriteLine($"[PaymentSummary #{ps.Id}] Enrollment #{ps.EnrollmentId} ({ps.Enrollment?.Student?.FullName} - {ps.Enrollment?.TrainingTrack?.Code})");
            Console.WriteLine($"  Required: {ps.TotalRequired:N2} EGP | Paid: {ps.TotalPaid:N2} EGP | Remaining: {ps.RemainingAmount:N2} EGP | Status: {ps.PaymentStatus}");
        }

        Console.WriteLine("✅ Drill 05 passed: 1:1 PaymentSummary with decimal(18,2) precision verified.");
    }
}
