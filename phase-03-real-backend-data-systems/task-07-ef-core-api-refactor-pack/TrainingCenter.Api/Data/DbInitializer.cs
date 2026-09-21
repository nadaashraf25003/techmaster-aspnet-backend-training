using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(TrainingCenterDbContext context)
    {
        if (await context.Instructors.AnyAsync())
        {
            return; // DB already seeded
        }

        var instructors = new List<Instructor>
        {
            new()
            {
                FullName = "Eng. Ahmed Hassan",
                Email = "ahmed.hassan@techmaster.com",
                PhoneNumber = "+201001112233",
                Specialization = "ASP.NET Core & Cloud Architecture",
                HourlyRate = 500.00m,
                IsActive = true
            },
            new()
            {
                FullName = "Dr. Mona Farouk",
                Email = "mona.farouk@techmaster.com",
                PhoneNumber = "+201002223344",
                Specialization = "EF Core & Database Optimization",
                HourlyRate = 600.00m,
                IsActive = true
            },
            new()
            {
                FullName = "Eng. Kareem Tarek",
                Email = "kareem.tarek@techmaster.com",
                PhoneNumber = "+201003334455",
                Specialization = "Full Stack .NET & React",
                HourlyRate = 450.00m,
                IsActive = true
            }
        };

        await context.Instructors.AddRangeAsync(instructors);
        await context.SaveChangesAsync();

        var tracks = new List<TrainingTrack>
        {
            new()
            {
                Code = "NET-CORE-101",
                Name = "ASP.NET Core Backend Masterclass",
                Description = "Master RESTful APIs, Clean Architecture, and EF Core 10",
                Price = 7500.00m,
                Capacity = 20,
                Status = TrackStatus.Active,
                InstructorId = instructors[0].Id
            },
            new()
            {
                Code = "DB-EF-201",
                Name = "Advanced EF Core & SQL Performance",
                Description = "Deep dive into query optimization, migrations, and concurrency",
                Price = 8500.00m,
                Capacity = 15,
                Status = TrackStatus.Active,
                InstructorId = instructors[1].Id
            },
            new()
            {
                Code = "FULLSTACK-301",
                Name = "Full Stack .NET & Angular Enterprise",
                Description = "Enterprise full stack development from database to frontend",
                Price = 9500.00m,
                Capacity = 25,
                Status = TrackStatus.Planned,
                InstructorId = instructors[2].Id
            },
            new()
            {
                Code = "MICROSERVICES-401",
                Name = "Cloud Native Microservices with Docker",
                Description = "Distributed architecture, messaging, and container deployment",
                Price = 12000.00m,
                Capacity = 2, // Low capacity for testing capacity guard!
                Status = TrackStatus.Active,
                InstructorId = instructors[0].Id
            }
        };

        await context.TrainingTracks.AddRangeAsync(tracks);
        await context.SaveChangesAsync();

        var students = new List<Student>
        {
            new()
            {
                FullName = "Tamer Hosny",
                Email = "tamer.hosny@student.com",
                PhoneNumber = "+201111223344",
                DateOfBirth = new DateTime(1998, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Address = "Nasr City, Cairo",
                IsActive = true
            },
            new()
            {
                FullName = "Sara Mostafa",
                Email = "sara.mostafa@student.com",
                PhoneNumber = "+201122334455",
                DateOfBirth = new DateTime(2000, 8, 14, 0, 0, 0, DateTimeKind.Utc),
                Address = "Dokki, Giza",
                IsActive = true
            },
            new()
            {
                FullName = "Omar Abdelaziz",
                Email = "omar.abdelaziz@student.com",
                PhoneNumber = "+201133445566",
                DateOfBirth = new DateTime(1999, 11, 3, 0, 0, 0, DateTimeKind.Utc),
                Address = "Maadi, Cairo",
                IsActive = true
            },
            new()
            {
                FullName = "Nourhan Ezzat",
                Email = "nourhan.ezzat@student.com",
                PhoneNumber = "+201144556677",
                DateOfBirth = new DateTime(2002, 2, 18, 0, 0, 0, DateTimeKind.Utc),
                Address = "Heliopolis, Cairo",
                IsActive = true
            }
        };

        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        var enrollments = new List<Enrollment>
        {
            new()
            {
                StudentId = students[0].Id,
                TrainingTrackId = tracks[0].Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-10),
                Status = EnrollmentStatus.Active
            },
            new()
            {
                StudentId = students[1].Id,
                TrainingTrackId = tracks[0].Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-8),
                Status = EnrollmentStatus.Active
            },
            new()
            {
                StudentId = students[2].Id,
                TrainingTrackId = tracks[3].Id, // Track 3 has capacity 2
                EnrollmentDate = DateTime.UtcNow.AddDays(-5),
                Status = EnrollmentStatus.Active
            },
            new()
            {
                StudentId = students[3].Id,
                TrainingTrackId = tracks[3].Id, // Track 3 is now FULL (2/2)
                EnrollmentDate = DateTime.UtcNow.AddDays(-2),
                Status = EnrollmentStatus.Active
            }
        };

        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();

        var payments = new List<Payment>
        {
            new()
            {
                EnrollmentId = enrollments[0].Id,
                Amount = 7500.00m,
                PaymentDate = DateTime.UtcNow.AddDays(-9),
                PaymentMethod = PaymentMethod.CreditCard,
                Status = PaymentStatus.Paid,
                ReferenceNumber = "PAY-2026-INIT-001",
                Notes = "Full payment received via CreditCard"
            },
            new()
            {
                EnrollmentId = enrollments[1].Id,
                Amount = 3000.00m,
                PaymentDate = DateTime.UtcNow.AddDays(-7),
                PaymentMethod = PaymentMethod.InstaPay,
                Status = PaymentStatus.Paid,
                ReferenceNumber = "PAY-2026-INIT-002",
                Notes = "Deposit installment paid"
            },
            new()
            {
                EnrollmentId = enrollments[2].Id,
                Amount = 12000.00m,
                PaymentDate = DateTime.UtcNow.AddDays(-4),
                PaymentMethod = PaymentMethod.BankTransfer,
                Status = PaymentStatus.Paid,
                ReferenceNumber = "PAY-2026-INIT-003",
                Notes = "Full tuition transfer"
            }
        };

        await context.Payments.AddRangeAsync(payments);
        await context.SaveChangesAsync();
    }
}
