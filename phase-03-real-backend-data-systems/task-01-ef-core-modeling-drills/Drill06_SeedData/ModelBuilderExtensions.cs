using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using EFCoreModelingDrills.Drill02_OneToOneStudentProfile;
using EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;
using EFCoreModelingDrills.Drill04_ManyToManyEnrollment;
using EFCoreModelingDrills.Drill05_PaymentSummary;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill06_SeedData;

/// <summary>
/// Drill 06: ModelBuilder extension for deterministic seed data using EF Core HasData.
/// Seeds 5 students, 2 instructors, 3 tracks, and 5 enrollments.
/// </summary>
public static class ModelBuilderExtensions
{
    public static void SeedDrillData(this ModelBuilder modelBuilder)
    {
        var fixedDate = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // 1. Instructors (2 seeded)
        modelBuilder.Entity<Instructor>().HasData(
            new Instructor
            {
                Id = 1,
                FullName = "Eng. Ahmed Hassan",
                Email = "ahmed.hassan@techmaster.com",
                Specialization = "ASP.NET Core & Cloud Architecture",
                Bio = "Principal Architect with 12+ years in enterprise .NET",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Instructor
            {
                Id = 2,
                FullName = "Eng. Sara Mahmoud",
                Email = "sara.mahmoud@techmaster.com",
                Specialization = "SQL Server & Database Engineering",
                Bio = "Senior Database Consultant and Performance Tuning Expert",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            }
        );

        // 2. Training Tracks (3 seeded)
        modelBuilder.Entity<TrainingTrack>().HasData(
            new TrainingTrack
            {
                Id = 1,
                Title = "ASP.NET Core Backend Mastery",
                Code = "NET-BE-01",
                Description = "From C# foundations to production microservices and cloud deployment.",
                Level = TrackLevel.Advanced,
                Capacity = 30,
                Price = 6500m,
                InstructorId = 1,
                StartDate = fixedDate.AddDays(15),
                EndDate = fixedDate.AddDays(105),
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new TrainingTrack
            {
                Id = 2,
                Title = "Database Design & SQL Server Performance",
                Code = "SQL-DB-02",
                Description = "Deep dive into relational modeling, indexing, and execution plans.",
                Level = TrackLevel.Intermediate,
                Capacity = 25,
                Price = 4500m,
                InstructorId = 2,
                StartDate = fixedDate.AddDays(20),
                EndDate = fixedDate.AddDays(80),
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new TrainingTrack
            {
                Id = 3,
                Title = "C# OOP & Clean Architecture Basics",
                Code = "CS-FND-03",
                Description = "Object-oriented design patterns, SOLID principles, and clean code.",
                Level = TrackLevel.Beginner,
                Capacity = 20,
                Price = 3500m,
                InstructorId = 1,
                StartDate = fixedDate.AddDays(10),
                EndDate = fixedDate.AddDays(60),
                CreatedAt = fixedDate,
                IsDeleted = false
            }
        );

        // 3. Students (5 seeded)
        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                FullName = "Nada Ashraf",
                Email = "nada.ashraf@example.com",
                PhoneNumber = "+201001112233",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Student
            {
                Id = 2,
                FullName = "Mohamed Ayman",
                Email = "mohamed.ayman@example.com",
                PhoneNumber = "+201002223344",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Student
            {
                Id = 3,
                FullName = "Youssef Ibrahim",
                Email = "youssef.ibrahim@example.com",
                PhoneNumber = "+201003334455",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Student
            {
                Id = 4,
                FullName = "Mariam Ali",
                Email = "mariam.ali@example.com",
                PhoneNumber = "+201004445566",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Student
            {
                Id = 5,
                FullName = "Omar Khaled",
                Email = "omar.khaled@example.com",
                PhoneNumber = "+201005556677",
                IsActive = true,
                CreatedAt = fixedDate,
                IsDeleted = false
            }
        );

        // 4. Student Profiles (1:1 with Students)
        modelBuilder.Entity<StudentProfile>().HasData(
            new StudentProfile
            {
                Id = 1,
                StudentId = 1,
                NationalId = "29901011234567",
                Address = "Cairo, Nasr City",
                EmergencyPhone = "+201099998888",
                DateOfBirth = new DateTime(2001, 5, 20)
            },
            new StudentProfile
            {
                Id = 2,
                StudentId = 2,
                NationalId = "29802022345678",
                Address = "Giza, Dokki",
                EmergencyPhone = "+201088887777",
                DateOfBirth = new DateTime(2000, 8, 14)
            }
        );

        // 5. Enrollments (5 seeded)
        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                Id = 1,
                StudentId = 1,
                TrainingTrackId = 1,
                EnrollmentDate = fixedDate.AddDays(1),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 45,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Enrollment
            {
                Id = 2,
                StudentId = 1,
                TrainingTrackId = 2,
                EnrollmentDate = fixedDate.AddDays(2),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 20,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Enrollment
            {
                Id = 3,
                StudentId = 2,
                TrainingTrackId = 1,
                EnrollmentDate = fixedDate.AddDays(1),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 40,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Enrollment
            {
                Id = 4,
                StudentId = 3,
                TrainingTrackId = 3,
                EnrollmentDate = fixedDate.AddDays(3),
                Status = EnrollmentStatus.Pending,
                ProgressPercentage = 0,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new Enrollment
            {
                Id = 5,
                StudentId = 4,
                TrainingTrackId = 2,
                EnrollmentDate = fixedDate.AddDays(4),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 10,
                CreatedAt = fixedDate,
                IsDeleted = false
            }
        );

        // 6. Payment Summaries (1:1 with Enrollments)
        modelBuilder.Entity<PaymentSummary>().HasData(
            new PaymentSummary
            {
                Id = 1,
                EnrollmentId = 1,
                TotalRequired = 6500m,
                TotalPaid = 6500m,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new PaymentSummary
            {
                Id = 2,
                EnrollmentId = 2,
                TotalRequired = 4500m,
                TotalPaid = 2250m,
                PaymentStatus = PaymentStatus.PartiallyPaid,
                CreatedAt = fixedDate,
                IsDeleted = false
            },
            new PaymentSummary
            {
                Id = 3,
                EnrollmentId = 3,
                TotalRequired = 6500m,
                TotalPaid = 6500m,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = fixedDate,
                IsDeleted = false
            }
        );
    }
}
