using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(TrainingCenterDbContext context)
    {
        // Check if database already seeded
        if (await context.Students.AnyAsync())
        {
            return;
        }

        // 1. Seed Instructors
        var instructors = new List<Instructor>
        {
            new() { InstructorId = 1, FullName = "Eng. Ahmed Hassan", Email = "ahmed.hassan@techmaster.net", Specialization = ".NET & Cloud Architecture", Bio = "Senior Solutions Architect with 14+ years experience in Enterprise Systems.", IsActive = true },
            new() { InstructorId = 2, FullName = "Eng. Sara Ibrahim", Email = "sara.ibrahim@techmaster.net", Specialization = "Full Stack React & Node", Bio = "Principal Frontend Engineer and Technical Speaker.", IsActive = true },
            new() { InstructorId = 3, FullName = "Eng. Mahmoud Ali", Email = "mahmoud.ali@techmaster.net", Specialization = "DevOps & Kubernetes", Bio = "DevOps Lead specializing in Azure CI/CD pipelines.", IsActive = true },
            new() { InstructorId = 4, FullName = "Eng. Mona Farouk", Email = "mona.farouk@techmaster.net", Specialization = "AI & Data Engineering", Bio = "Data Science Consultant with PhD in Applied ML.", IsActive = true }
        };
        await context.Instructors.AddRangeAsync(instructors);
        await context.SaveChangesAsync();

        // 2. Seed Students
        var students = new List<Student>
        {
            new() { StudentId = 1, FullName = "Nada Ashraf", Email = "nada.ashraf@student.techmaster.net", PhoneNumber = "+201001234567", IsActive = true, IsDeleted = false },
            new() { StudentId = 2, FullName = "Youssef Mohamed", Email = "youssef.mohamed@student.techmaster.net", PhoneNumber = "+201009876543", IsActive = true, IsDeleted = false },
            new() { StudentId = 3, FullName = "Mariam Tarek", Email = "mariam.tarek@student.techmaster.net", PhoneNumber = "+201112233445", IsActive = true, IsDeleted = false },
            new() { StudentId = 4, FullName = "Omar Khaled", Email = "omar.khaled@student.techmaster.net", PhoneNumber = "+201223344556", IsActive = true, IsDeleted = false },
            new() { StudentId = 5, FullName = "Kareem Gamal", Email = "kareem.gamal@student.techmaster.net", PhoneNumber = "+201011223344", IsActive = true, IsDeleted = false },
            new() { StudentId = 6, FullName = "Hoda Mostafa", Email = "hoda.mostafa@student.techmaster.net", PhoneNumber = "+201155667788", IsActive = true, IsDeleted = false }
        };
        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        // 3. Seed Training Tracks
        var tracks = new List<TrainingTrack>
        {
            new() { TrainingTrackId = 1, Title = "ASP.NET Core Enterprise Backend BootCamp", Code = "NET-BE-2026", Description = "Deep dive into EF Core, SQL Server, Clean Architecture, and Azure deployment.", Level = TrackLevel.Advanced, Price = 4500.00m, Capacity = 3, StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 12, 15), Status = TrackStatus.InProgress, InstructorId = 1, IsDeleted = false },
            new() { TrainingTrackId = 2, Title = "Modern Full Stack React & ASP.NET API", Code = "FS-REACT-2026", Description = "Comprehensive fullstack journey building cloud-native apps.", Level = TrackLevel.Intermediate, Price = 5200.00m, Capacity = 2, StartDate = new DateOnly(2026, 9, 20), EndDate = new DateOnly(2027, 1, 30), Status = TrackStatus.Upcoming, InstructorId = 2, IsDeleted = false },
            new() { TrainingTrackId = 3, Title = "Enterprise DevOps & Kubernetes Masterclass", Code = "DEVOPS-K8S-2026", Description = "Docker, Kubernetes, Terraform, and CI/CD pipelines.", Level = TrackLevel.Advanced, Price = 6000.00m, Capacity = 20, StartDate = new DateOnly(2026, 10, 15), EndDate = new DateOnly(2027, 2, 15), Status = TrackStatus.Upcoming, InstructorId = 3, IsDeleted = false },
            new() { TrainingTrackId = 4, Title = "Data Engineering & Power BI FastTrack", Code = "DATA-BI-2026", Description = "Data warehousing, ETL pipelines, and BI reporting.", Level = TrackLevel.Beginner, Price = 3800.00m, Capacity = 15, StartDate = new DateOnly(2026, 8, 1), EndDate = new DateOnly(2026, 10, 1), Status = TrackStatus.Completed, InstructorId = 4, IsDeleted = false }
        };
        await context.TrainingTracks.AddRangeAsync(tracks);
        await context.SaveChangesAsync();

        // 4. Seed Enrollments
        var enrollments = new List<Enrollment>
        {
            new() { EnrollmentId = 1, StudentId = 1, TrainingTrackId = 1, EnrollmentDate = new DateTime(2026, 8, 15, 10, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 65.50m },
            new() { EnrollmentId = 2, StudentId = 2, TrainingTrackId = 1, EnrollmentDate = new DateTime(2026, 8, 16, 11, 30, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 70.00m },
            new() { EnrollmentId = 3, StudentId = 3, TrainingTrackId = 1, EnrollmentDate = new DateTime(2026, 8, 18, 14, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 45.00m },
            new() { EnrollmentId = 4, StudentId = 4, TrainingTrackId = 2, EnrollmentDate = new DateTime(2026, 9, 5, 9, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 10.00m },
            new() { EnrollmentId = 5, StudentId = 1, TrainingTrackId = 3, EnrollmentDate = new DateTime(2026, 9, 10, 16, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Pending, ProgressPercentage = 0.00m },
            new() { EnrollmentId = 6, StudentId = 5, TrainingTrackId = 3, EnrollmentDate = new DateTime(2026, 9, 12, 12, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 0.00m },
            new() { EnrollmentId = 7, StudentId = 6, TrainingTrackId = 4, EnrollmentDate = new DateTime(2026, 7, 20, 10, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Completed, ProgressPercentage = 100.00m, FinalResult = "Distinction" }
        };
        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();

        // 5. Seed Payments
        var payments = new List<Payment>
        {
            new() { PaymentId = 1, EnrollmentId = 1, Amount = 2250.00m, PaymentMethod = PaymentMethod.CreditCard, PaymentDate = new DateTime(2026, 8, 15, 10, 15, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260815-001", Notes = "First installment paid via Visa." },
            new() { PaymentId = 2, EnrollmentId = 1, Amount = 2250.00m, PaymentMethod = PaymentMethod.Fawry, PaymentDate = new DateTime(2026, 9, 1, 14, 20, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260901-002", Notes = "Second installment paid via Fawry." },
            new() { PaymentId = 3, EnrollmentId = 2, Amount = 2000.00m, PaymentMethod = PaymentMethod.VodafoneCash, PaymentDate = new DateTime(2026, 8, 16, 11, 45, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260816-003", Notes = "Vodafone Cash transfer." },
            new() { PaymentId = 4, EnrollmentId = 4, Amount = 5200.00m, PaymentMethod = PaymentMethod.BankTransfer, PaymentDate = new DateTime(2026, 9, 5, 9, 30, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260905-004", Notes = "Bank transfer." },
            new() { PaymentId = 5, EnrollmentId = 5, Amount = 6000.00m, PaymentMethod = PaymentMethod.CreditCard, PaymentDate = new DateTime(2026, 9, 10, 16, 5, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Failed, ReferenceNumber = "PAY-20260910-005", Notes = "Card declined." },
            new() { PaymentId = 6, EnrollmentId = 6, Amount = 3000.00m, PaymentMethod = PaymentMethod.CreditCard, PaymentDate = new DateTime(2026, 9, 12, 12, 15, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260912-006", Notes = "First installment." },
            new() { PaymentId = 7, EnrollmentId = 7, Amount = 3800.00m, PaymentMethod = PaymentMethod.Cash, PaymentDate = new DateTime(2026, 7, 20, 10, 30, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260720-007", Notes = "Cash at front desk." }
        };
        await context.Payments.AddRangeAsync(payments);
        await context.SaveChangesAsync();
    }
}
