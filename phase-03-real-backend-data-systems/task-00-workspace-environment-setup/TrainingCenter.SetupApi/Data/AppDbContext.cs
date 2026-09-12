using Microsoft.EntityFrameworkCore;
using TrainingCenter.SetupApi.Entities;

namespace TrainingCenter.SetupApi.Data;

/// <summary>
/// Core EF Core DbContext for Training Center Database System.
/// Configured for SQL Server and extensible for relational entity configurations.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API Entity Configurations
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.FullName).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
