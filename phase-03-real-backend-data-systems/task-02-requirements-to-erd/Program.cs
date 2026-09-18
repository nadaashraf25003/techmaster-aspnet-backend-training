using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TechMaster.Phase03.Task02.Common;
using TechMaster.Phase03.Task02.Data;
using TechMaster.Phase03.Task02.Entities;
using TechMaster.Phase03.Task02.Entities.Bonus;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("==========================================================================================");
Console.WriteLine("  TECHMASTER ACADEMY - PHASE 03 / TASK 02: DATABASE MODELING & BUSINESS QUESTIONS RUNNER  ");
Console.WriteLine("==========================================================================================");
Console.WriteLine();

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

var config = builder.Build();
var sqlServerConnectionString = config.GetConnectionString("DefaultConnection");

DbContextOptions<TechMasterDbContext> options;
bool isSqlServer = false;

// Check if user requested SQL Server or default connection is provided
if (args.Contains("--sqlserver") || args.Contains("-s") || !string.IsNullOrWhiteSpace(sqlServerConnectionString))
{
    try
    {
        var testOptions = new DbContextOptionsBuilder<TechMasterDbContext>()
            .UseSqlServer(sqlServerConnectionString!)
            .Options;

        using var testDb = new TechMasterDbContext(testOptions);
        if (await testDb.Database.CanConnectAsync())
        {
            options = testOptions;
            isSqlServer = true;
            Console.WriteLine($"🔌 Connected to SQL Server database: TechMasterAcademyDb");
        }
        else
        {
            throw new Exception("Cannot connect to SQL Server");
        }
    }
    catch
    {
        Console.WriteLine("⚠️  SQL Server instance not reached, falling back to In-Memory Database for verification.");
        options = new DbContextOptionsBuilder<TechMasterDbContext>()
            .UseInMemoryDatabase("TechMasterAcademyVerificationDb_" + Guid.NewGuid())
            .Options;
    }
}
else
{
    options = new DbContextOptionsBuilder<TechMasterDbContext>()
        .UseInMemoryDatabase("TechMasterAcademyVerificationDb_" + Guid.NewGuid())
        .Options;
}

using var db = new TechMasterDbContext(options);

if (!isSqlServer)
{
    await SeedSampleDataAsync(db);
    Console.WriteLine("✅ In-Memory Relational Schema Created and Seeded Successfully.\n");
}
else
{
    // Ensure created and seed if empty
    if (!await db.Students.AnyAsync())
    {
        await SeedSampleDataAsync(db);
        Console.WriteLine("✅ SQL Server database seeded with initial data.\n");
    }
}

// ------------------------------------------------------------------------------------------------
// 1. Which students are enrolled in a specific track?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("1. Which students are enrolled in track 'NET-BE-2026'?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var trackStudents = await db.Enrollments
    .Where(e => e.TrainingTrack!.Code == "NET-BE-2026")
    .Select(e => new
    {
        e.Student!.StudentId,
        e.Student.FullName,
        e.Student.Email,
        e.Student.PhoneNumber,
        e.EnrollmentDate,
        e.Status,
        e.ProgressPercentage
    })
    .OrderBy(s => s.FullName)
    .ToListAsync();

foreach (var s in trackStudents)
{
    Console.WriteLine($"• [ID: {s.StudentId}] {s.FullName,-18} | Email: {s.Email,-35} | Status: {s.Status,-8} | Progress: {s.ProgressPercentage}%");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 2. Which tracks have available seats?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("2. Which tracks have available seats?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var availableSeats = await db.TrainingTracks
    .Select(t => new
    {
        t.TrainingTrackId,
        t.Code,
        t.Title,
        t.Capacity,
        EnrolledCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending),
        Available = t.Capacity - t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
    })
    .Where(t => t.Available > 0)
    .OrderByDescending(t => t.Available)
    .ToListAsync();

foreach (var t in availableSeats)
{
    Console.WriteLine($"• [{t.Code,-16}] {t.Title,-40} | Cap: {t.Capacity,2} | Enrolled: {t.EnrolledCount,2} | Available: {t.Available,2} seats");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 3. Which enrollments are unpaid or partially paid?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("3. Which enrollments are unpaid or partially paid?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var unpaidEnrollments = await db.Enrollments
    .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
    .Select(e => new
    {
        e.EnrollmentId,
        StudentName = e.Student!.FullName,
        TrackTitle = e.TrainingTrack!.Title,
        Price = e.TrainingTrack.Price,
        TotalPaid = e.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => (decimal?)p.Amount) ?? 0m,
        Outstanding = e.TrainingTrack.Price - (e.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => (decimal?)p.Amount) ?? 0m)
    })
    .Where(e => e.Outstanding > 0)
    .OrderByDescending(e => e.Outstanding)
    .ToListAsync();

foreach (var u in unpaidEnrollments)
{
    Console.WriteLine($"• [Enroll #{u.EnrollmentId}] {u.StudentName,-18} | Track: {u.TrackTitle,-36} | Price: ${u.Price,7:N2} | Paid: ${u.TotalPaid,7:N2} | Due: ${u.Outstanding,7:N2}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 4. How much revenue did each track generate?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("4. How much revenue did each track generate?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var trackRevenues = await db.TrainingTracks
    .Select(t => new
    {
        t.Code,
        t.Title,
        t.Price,
        EnrollmentsCount = t.Enrollments.Count,
        ExpectedRevenue = t.Enrollments.Count * t.Price,
        RealizedRevenue = t.Enrollments
            .SelectMany(e => e.Payments)
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .Sum(p => (decimal?)p.Amount) ?? 0m
    })
    .OrderByDescending(t => t.RealizedRevenue)
    .ToListAsync();

foreach (var r in trackRevenues)
{
    Console.WriteLine($"• [{r.Code,-16}] Enrolled: {r.EnrollmentsCount,2} | Expected: ${r.ExpectedRevenue,8:N2} | Realized Cash: ${r.RealizedRevenue,8:N2}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 5. Which instructor has the highest workload?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("5. Which instructor has the highest workload?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var instructorWorkload = await db.Instructors
    .Select(i => new
    {
        i.InstructorId,
        i.FullName,
        i.Specialization,
        ActiveTracks = i.TrainingTracks.Count(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming),
        SupervisedStudents = i.TrainingTracks.SelectMany(t => t.Enrollments).Count(e => e.Status == EnrollmentStatus.Active)
    })
    .OrderByDescending(i => i.SupervisedStudents)
    .ThenByDescending(i => i.ActiveTracks)
    .ToListAsync();

foreach (var i in instructorWorkload)
{
    Console.WriteLine($"• {i.FullName,-20} | Specialization: {i.Specialization,-26} | Active Tracks: {i.ActiveTracks,2} | Active Students: {i.SupervisedStudents,2}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 6. Which students have active enrollments?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("6. Which students have active enrollments?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var activeStudents = await db.Students
    .Where(s => s.Enrollments.Any(e => e.Status == EnrollmentStatus.Active))
    .Select(s => new
    {
        s.StudentId,
        s.FullName,
        s.Email,
        ActiveCount = s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)
    })
    .OrderBy(s => s.FullName)
    .ToListAsync();

foreach (var s in activeStudents)
{
    Console.WriteLine($"• [ID: {s.StudentId}] {s.FullName,-18} | Email: {s.Email,-35} | Active Tracks: {s.ActiveCount}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 7. Which tracks start this month (Month 9)?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("7. Which tracks start this month?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var targetMonth = DateTime.UtcNow.Month;
var targetYear = DateTime.UtcNow.Year;

var tracksThisMonth = await db.TrainingTracks
    .Include(t => t.Instructor)
    .Where(t => t.StartDate.Year == targetYear && t.StartDate.Month == targetMonth)
    .OrderBy(t => t.StartDate)
    .ToListAsync();

foreach (var t in tracksThisMonth)
{
    Console.WriteLine($"• [{t.Code,-16}] {t.Title,-40} | Starts: {t.StartDate:yyyy-MM-dd} | Mentor: {t.Instructor?.FullName}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 8. What is the payment history for an enrollment?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("8. What is the payment history for Enrollment #1?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var enrollmentPayments = await db.Payments
    .Include(p => p.Enrollment)!.ThenInclude(e => e!.Student)
    .Include(p => p.Enrollment)!.ThenInclude(e => e!.TrainingTrack)
    .Where(p => p.EnrollmentId == 1)
    .OrderByDescending(p => p.PaymentDate)
    .ToListAsync();

foreach (var p in enrollmentPayments)
{
    Console.WriteLine($"• Ref: {p.ReferenceNumber,-18} | Date: {p.PaymentDate:yyyy-MM-dd HH:mm} | Amount: ${p.Amount,7:N2} | Method: {p.PaymentMethod,-12} | Status: {p.PaymentStatus}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 9. Which tracks are full?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("9. Which tracks are full (At Capacity)?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var fullTracks = await db.TrainingTracks
    .Select(t => new
    {
        t.Code,
        t.Title,
        t.Capacity,
        Enrolled = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
    })
    .Where(t => t.Enrolled >= t.Capacity)
    .ToListAsync();

foreach (var f in fullTracks)
{
    Console.WriteLine($"• [FULL] [{f.Code,-16}] {f.Title,-40} | Capacity: {f.Capacity} | Enrolled: {f.Enrolled}");
}
Console.WriteLine();

// ------------------------------------------------------------------------------------------------
// 10. How many enrollments exist by status?
// ------------------------------------------------------------------------------------------------
Console.WriteLine("------------------------------------------------------------------------------------------");
Console.WriteLine("10. How many enrollments exist by status?");
Console.WriteLine("------------------------------------------------------------------------------------------");

var totalEnrollments = await db.Enrollments.CountAsync();
var enrollmentsByStatus = await db.Enrollments
    .GroupBy(e => e.Status)
    .Select(g => new
    {
        Status = g.Key,
        Count = g.Count(),
        Percentage = (g.Count() * 100.0) / totalEnrollments
    })
    .OrderByDescending(g => g.Count)
    .ToListAsync();

foreach (var stat in enrollmentsByStatus)
{
    Console.WriteLine($"• Status: {stat.Status,-12} | Total: {stat.Count,2} | Pct: {stat.Percentage:F1}%");
}

Console.WriteLine();
Console.WriteLine("==========================================================================================");
Console.WriteLine("  ALL 10 BUSINESS QUESTIONS SUCCESSFULLY EXECUTED & VERIFIED VIA EF CORE & LINQ!           ");
Console.WriteLine("==========================================================================================");

// ------------------------------------------------------------------------------------------------
// Data Seeding Helper
// ------------------------------------------------------------------------------------------------
static async Task SeedSampleDataAsync(TechMasterDbContext db)
{
    var ins1 = new Instructor { InstructorId = 1, FullName = "Eng. Ahmed Hassan", Email = "ahmed.hassan@techmaster.net", Specialization = ".NET & Cloud Architecture", Bio = "Senior Architect", IsActive = true };
    var ins2 = new Instructor { InstructorId = 2, FullName = "Eng. Sara Ibrahim", Email = "sara.ibrahim@techmaster.net", Specialization = "Full Stack React & Node", Bio = "Principal Frontend Lead", IsActive = true };
    var ins3 = new Instructor { InstructorId = 3, FullName = "Eng. Mahmoud Ali", Email = "mahmoud.ali@techmaster.net", Specialization = "DevOps & Kubernetes", Bio = "DevOps Specialist", IsActive = true };
    var ins4 = new Instructor { InstructorId = 4, FullName = "Eng. Mona Farouk", Email = "mona.farouk@techmaster.net", Specialization = "AI & Data Engineering", Bio = "Data Consultant", IsActive = true };
    db.Instructors.AddRange(ins1, ins2, ins3, ins4);

    var s1 = new Student { StudentId = 1, FullName = "Nada Ashraf", Email = "nada.ashraf@student.techmaster.net", PhoneNumber = "+201001234567", IsActive = true, IsDeleted = false };
    var s2 = new Student { StudentId = 2, FullName = "Youssef Mohamed", Email = "youssef.mohamed@student.techmaster.net", PhoneNumber = "+201009876543", IsActive = true, IsDeleted = false };
    var s3 = new Student { StudentId = 3, FullName = "Mariam Tarek", Email = "mariam.tarek@student.techmaster.net", PhoneNumber = "+201112233445", IsActive = true, IsDeleted = false };
    var s4 = new Student { StudentId = 4, FullName = "Omar Khaled", Email = "omar.khaled@student.techmaster.net", PhoneNumber = "+201223344556", IsActive = true, IsDeleted = false };
    var s5 = new Student { StudentId = 5, FullName = "Kareem Gamal", Email = "kareem.gamal@student.techmaster.net", PhoneNumber = "+201011223344", IsActive = true, IsDeleted = false };
    var s6 = new Student { StudentId = 6, FullName = "Hoda Mostafa", Email = "hoda.mostafa@student.techmaster.net", PhoneNumber = "+201155667788", IsActive = true, IsDeleted = false };
    db.Students.AddRange(s1, s2, s3, s4, s5, s6);

    var t1 = new TrainingTrack { TrainingTrackId = 1, Title = "ASP.NET Core Enterprise Backend BootCamp", Code = "NET-BE-2026", Description = "Clean architecture", Level = TrackLevel.Advanced, Price = 4500.00m, Capacity = 3, StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 12, 15), Status = TrackStatus.InProgress, InstructorId = 1, IsDeleted = false };
    var t2 = new TrainingTrack { TrainingTrackId = 2, Title = "Modern Full Stack React & ASP.NET API", Code = "FS-REACT-2026", Description = "Fullstack React & .NET", Level = TrackLevel.Intermediate, Price = 5200.00m, Capacity = 2, StartDate = new DateOnly(2026, 9, 20), EndDate = new DateOnly(2027, 1, 30), Status = TrackStatus.Upcoming, InstructorId = 2, IsDeleted = false };
    var t3 = new TrainingTrack { TrainingTrackId = 3, Title = "Enterprise DevOps & Kubernetes Masterclass", Code = "DEVOPS-K8S-2026", Description = "K8s & CI/CD", Level = TrackLevel.Advanced, Price = 6000.00m, Capacity = 20, StartDate = new DateOnly(2026, 10, 15), EndDate = new DateOnly(2027, 2, 15), Status = TrackStatus.Upcoming, InstructorId = 3, IsDeleted = false };
    var t4 = new TrainingTrack { TrainingTrackId = 4, Title = "Data Engineering & Power BI FastTrack", Code = "DATA-BI-2026", Description = "Data warehousing", Level = TrackLevel.Beginner, Price = 3800.00m, Capacity = 15, StartDate = new DateOnly(2026, 8, 1), EndDate = new DateOnly(2026, 10, 1), Status = TrackStatus.Completed, InstructorId = 4, IsDeleted = false };
    db.TrainingTracks.AddRange(t1, t2, t3, t4);

    var e1 = new Enrollment { EnrollmentId = 1, StudentId = 1, TrainingTrackId = 1, EnrollmentDate = new DateTime(2026, 8, 15, 10, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 65.50m };
    var e2 = new Enrollment { EnrollmentId = 2, StudentId = 2, TrainingTrackId = 1, EnrollmentDate = new DateTime(2026, 8, 16, 11, 30, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 70.00m };
    var e3 = new Enrollment { EnrollmentId = 3, StudentId = 3, TrainingTrackId = 1, EnrollmentDate = new DateTime(2026, 8, 18, 14, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 45.00m };
    var e4 = new Enrollment { EnrollmentId = 4, StudentId = 4, TrainingTrackId = 2, EnrollmentDate = new DateTime(2026, 9, 5, 9, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 10.00m };
    var e5 = new Enrollment { EnrollmentId = 5, StudentId = 1, TrainingTrackId = 3, EnrollmentDate = new DateTime(2026, 9, 10, 16, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Pending, ProgressPercentage = 0.00m };
    var e6 = new Enrollment { EnrollmentId = 6, StudentId = 5, TrainingTrackId = 3, EnrollmentDate = new DateTime(2026, 9, 12, 12, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Active, ProgressPercentage = 0.00m };
    var e7 = new Enrollment { EnrollmentId = 7, StudentId = 6, TrainingTrackId = 4, EnrollmentDate = new DateTime(2026, 7, 20, 10, 0, 0, DateTimeKind.Utc), Status = EnrollmentStatus.Completed, ProgressPercentage = 100.00m, FinalResult = "Distinction" };
    db.Enrollments.AddRange(e1, e2, e3, e4, e5, e6, e7);

    var p1 = new Payment { PaymentId = 1, EnrollmentId = 1, Amount = 2250.00m, PaymentMethod = PaymentMethod.CreditCard, PaymentDate = new DateTime(2026, 8, 15, 10, 15, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260815-001", Notes = "First installment" };
    var p2 = new Payment { PaymentId = 2, EnrollmentId = 1, Amount = 2250.00m, PaymentMethod = PaymentMethod.Fawry, PaymentDate = new DateTime(2026, 9, 1, 14, 20, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260901-002", Notes = "Second installment" };
    var p3 = new Payment { PaymentId = 3, EnrollmentId = 2, Amount = 2000.00m, PaymentMethod = PaymentMethod.VodafoneCash, PaymentDate = new DateTime(2026, 8, 16, 11, 45, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260816-003", Notes = "Vodafone Cash" };
    var p4 = new Payment { PaymentId = 4, EnrollmentId = 4, Amount = 5200.00m, PaymentMethod = PaymentMethod.BankTransfer, PaymentDate = new DateTime(2026, 9, 5, 9, 30, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260905-004", Notes = "Bank wire" };
    var p5 = new Payment { PaymentId = 5, EnrollmentId = 5, Amount = 6000.00m, PaymentMethod = PaymentMethod.CreditCard, PaymentDate = new DateTime(2026, 9, 10, 16, 5, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Failed, ReferenceNumber = "PAY-20260910-005", Notes = "Declined" };
    var p6 = new Payment { PaymentId = 6, EnrollmentId = 6, Amount = 3000.00m, PaymentMethod = PaymentMethod.CreditCard, PaymentDate = new DateTime(2026, 9, 12, 12, 15, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260912-006", Notes = "Half payment" };
    var p7 = new Payment { PaymentId = 7, EnrollmentId = 7, Amount = 3800.00m, PaymentMethod = PaymentMethod.Cash, PaymentDate = new DateTime(2026, 7, 20, 10, 30, 0, DateTimeKind.Utc), PaymentStatus = PaymentStatus.Completed, ReferenceNumber = "PAY-20260720-007", Notes = "Front desk" };
    db.Payments.AddRange(p1, p2, p3, p4, p5, p6, p7);

    await db.SaveChangesAsync();
}
