using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(TrainingCenterDbContext context)
    {
        if (await context.Students.AnyAsync())
        {
            return;
        }

        // 1. Seed Instructors
        var instructors = new List<Instructor>
        {
            new Instructor
            {
                FullName = "Eng. Mohamed Ali",
                Email = "m.ali@techmaster.edu",
                PhoneNumber = "+201011112222",
                Specialization = ".NET Cloud & Enterprise Architecture",
                HourlyRate = 650.00m,
                IsActive = true
            },
            new Instructor
            {
                FullName = "Dr. Sara Hassan",
                Email = "s.hassan@techmaster.edu",
                PhoneNumber = "+201022223333",
                Specialization = "AI & Machine Learning Engineering",
                HourlyRate = 750.00m,
                IsActive = true
            },
            new Instructor
            {
                FullName = "Eng. Inactive Instructor",
                Email = "inactive.instructor@techmaster.edu",
                PhoneNumber = "+201099998888",
                Specialization = "Archived Subjects",
                HourlyRate = 400.00m,
                IsActive = false
            }
        };
        await context.Instructors.AddRangeAsync(instructors);
        await context.SaveChangesAsync();

        // 2. Seed Training Tracks
        var tracks = new List<TrainingTrack>
        {
            new TrainingTrack
            {
                Title = "ASP.NET Core Enterprise Backend BootCamp",
                Code = "NET-BE-2026",
                Description = "Comprehensive enterprise mastery of C#, EF Core, Clean Architecture, CQRS, and Microservices.",
                Price = 8500.00m,
                DurationHours = 120,
                Capacity = 5,
                Status = TrackStatus.InProgress,
                StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[0].InstructorId
            },
            new TrainingTrack
            {
                Title = "Full Stack Modern React & Next.js MasterClass",
                Code = "REACT-FULL-2026",
                Description = "Full stack modern application development using TypeScript, Next.js, and Node.js.",
                Price = 5000.00m,
                DurationHours = 100,
                Capacity = 2,
                Status = TrackStatus.InProgress,
                StartDate = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 9, 25, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[0].InstructorId
            },
            new TrainingTrack
            {
                Title = "Legacy Architecture & Monolith Refactoring",
                Code = "ARCH-CLOSED-2026",
                Description = "Strategies for strangler fig pattern and monolith decomposition.",
                Price = 6000.00m,
                DurationHours = 60,
                Capacity = 15,
                Status = TrackStatus.Closed,
                StartDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[1].InstructorId
            },
            new TrainingTrack
            {
                Title = "Advanced Applied Machine Learning & Deep Learning",
                Code = "AI-ML-2026",
                Description = "Practical AI engineering with PyTorch, Scikit-Learn, and MLOps pipelines.",
                Price = 10500.00m,
                DurationHours = 140,
                Capacity = 20,
                Status = TrackStatus.Upcoming,
                StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[1].InstructorId
            }
        };
        await context.TrainingTracks.AddRangeAsync(tracks);
        await context.SaveChangesAsync();

        // 3. Seed Students
        var students = new List<Student>
        {
            new Student
            {
                FullName = "Nada Ashraf",
                Email = "nada.ashraf@gmail.com",
                PhoneNumber = "+201100001111",
                DateOfBirth = new DateTime(2002, 5, 14, 0, 0, 0, DateTimeKind.Utc),
                Address = "Nasr City, Cairo",
                IsActive = true
            },
            new Student
            {
                FullName = "Ahmed Mansour",
                Email = "ahmed.mansour@yahoo.com",
                PhoneNumber = "+201100002222",
                DateOfBirth = new DateTime(1999, 11, 23, 0, 0, 0, DateTimeKind.Utc),
                Address = "Dokki, Giza",
                IsActive = true
            },
            new Student
            {
                FullName = "Salma Ibrahim",
                Email = "salma.ibrahim@outlook.com",
                PhoneNumber = "+201100003333",
                DateOfBirth = new DateTime(2001, 8, 19, 0, 0, 0, DateTimeKind.Utc),
                Address = "Maadi, Cairo",
                IsActive = true
            },
            new Student
            {
                FullName = "Omar Farouk",
                Email = "omar.farouk@techmail.com",
                PhoneNumber = "+201100004444",
                DateOfBirth = new DateTime(2000, 3, 30, 0, 0, 0, DateTimeKind.Utc),
                Address = "Alexandria",
                IsActive = true
            },
            new Student
            {
                FullName = "Tarek Inactive Student",
                Email = "tarek.inactive@techmail.com",
                PhoneNumber = "+201100005555",
                DateOfBirth = new DateTime(1998, 4, 12, 0, 0, 0, DateTimeKind.Utc),
                Address = "Giza",
                IsActive = false
            },
            new Student
            {
                FullName = "Khaled Mostafa",
                Email = "khaled.mostafa@techmail.com",
                PhoneNumber = "+201100006666",
                DateOfBirth = new DateTime(1998, 7, 8, 0, 0, 0, DateTimeKind.Utc),
                Address = "Heliopolis, Cairo",
                IsActive = true
            },
            new Student
            {
                FullName = "Mariam Samir",
                Email = "mariam.samir@gmail.com",
                PhoneNumber = "+201100007777",
                DateOfBirth = new DateTime(2001, 4, 25, 0, 0, 0, DateTimeKind.Utc),
                Address = "Mohandessin, Giza",
                IsActive = true
            }
        };
        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        // 4. Seed Enrollments
        var enrollments = new List<Enrollment>
        {
            new Enrollment
            {
                StudentId = students[0].StudentId,
                TrainingTrackId = tracks[0].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 2, 9, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 65.00m,
                Notes = "Full tuition payment completed."
            },
            new Enrollment
            {
                StudentId = students[1].StudentId,
                TrainingTrackId = tracks[0].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 3, 10, 30, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 50.00m,
                Notes = "First installment paid 5,000 EGP. Remaining = 3,500 EGP."
            },
            new Enrollment
            {
                StudentId = students[2].StudentId,
                TrainingTrackId = tracks[1].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 6, 12, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 40.00m
            },
            new Enrollment
            {
                StudentId = students[3].StudentId,
                TrainingTrackId = tracks[1].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 7, 14, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 35.00m
            },
            new Enrollment
            {
                StudentId = students[6].StudentId,
                TrainingTrackId = tracks[3].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Completed,
                ProgressPercentage = 100.00m,
                FinalResult = "Passed with Honors (96%)",
                Notes = "Completed prior batch."
            },
            new Enrollment
            {
                StudentId = students[5].StudentId,
                TrainingTrackId = tracks[0].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Cancelled,
                ProgressPercentage = 0.00m,
                Notes = "Withdrew before track started."
            }
        };
        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();

        // 5. Seed Payments
        var payments = new List<Payment>
        {
            new Payment
            {
                EnrollmentId = enrollments[0].EnrollmentId,
                Amount = 8500.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentDate = new DateTime(2026, 7, 2, 9, 15, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-001",
                Notes = "Full tuition payment via Visa."
            },
            new Payment
            {
                EnrollmentId = enrollments[1].EnrollmentId,
                Amount = 5000.00m,
                PaymentMethod = PaymentMethod.VodafoneCash,
                PaymentDate = new DateTime(2026, 7, 3, 11, 0, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-002",
                Notes = "First installment paid via Vodafone Cash."
            },
            new Payment
            {
                EnrollmentId = enrollments[2].EnrollmentId,
                Amount = 5000.00m,
                PaymentMethod = PaymentMethod.Fawry,
                PaymentDate = new DateTime(2026, 7, 6, 12, 30, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-003",
                Notes = "Fawry payment."
            },
            new Payment
            {
                EnrollmentId = enrollments[3].EnrollmentId,
                Amount = 5000.00m,
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentDate = new DateTime(2026, 7, 7, 14, 30, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-004",
                Notes = "Bank transfer."
            },
            new Payment
            {
                EnrollmentId = enrollments[4].EnrollmentId,
                Amount = 10500.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentDate = new DateTime(2026, 6, 1, 9, 30, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-06-001",
                Notes = "Completed payment for completed track."
            }
        };
        await context.Payments.AddRangeAsync(payments);
        await context.SaveChangesAsync();
    }
}
