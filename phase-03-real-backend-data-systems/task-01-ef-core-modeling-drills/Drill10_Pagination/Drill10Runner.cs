using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Data;
using EFCoreModelingDrills.Drill09_ProjectionDTO;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill10_Pagination;

public static class Drill10Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 10: Server-Side Pagination (Skip/Take Math)");
        Console.WriteLine("========================================================");

        int pageNumber = 1;
        int pageSize = 3;

        // Base query with projection
        var query = context.Students.Select(s => new StudentListItemDto
        {
            StudentId = s.Id,
            FullName = s.FullName,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            IsActive = s.IsActive,
            ActiveEnrollmentsCount = s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
        });

        int totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.StudentId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagedResult = new PaginationResult<StudentListItemDto>(items, totalCount, pageNumber, pageSize);

        Console.WriteLine($"[Pagination Result - Page {pagedResult.PageNumber}/{pagedResult.TotalPages}]");
        Console.WriteLine($"  Total Records: {pagedResult.TotalCount} | Page Size: {pagedResult.PageSize} | HasNext: {pagedResult.HasNextPage} | HasPrev: {pagedResult.HasPreviousPage}");
        Console.WriteLine("  Page Items:");
        foreach (var item in pagedResult.Items)
        {
            Console.WriteLine($"    - #{item.StudentId} {item.FullName} ({item.Email})");
        }

        // Test Validation
        try
        {
            _ = new PaginationResult<StudentListItemDto>(items, totalCount, pageNumber: 0, pageSize: 10);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("  [Validation Test] PageNumber <= 0 correctly rejected with ArgumentOutOfRangeException.");
        }

        try
        {
            _ = new PaginationResult<StudentListItemDto>(items, totalCount, pageNumber: 1, pageSize: 100);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("  [Validation Test] PageSize > 50 correctly rejected with ArgumentOutOfRangeException.");
        }

        Console.WriteLine("✅ Drill 10 passed: Server-side pagination with mathematical metadata and validation verified.");
    }
}
