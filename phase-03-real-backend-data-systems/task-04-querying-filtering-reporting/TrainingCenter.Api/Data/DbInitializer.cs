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
            return; // Database already seeded
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
                FullName = "Eng. Tarek Mahmoud",
                Email = "t.mahmoud@techmaster.edu",
                PhoneNumber = "+201033334444",
                Specialization = "DevOps & Cloud Infrastructure",
                HourlyRate = 700.00m,
                IsActive = true
            },
            new Instructor
            {
                FullName = "Eng. Nour El-Din",
                Email = "n.eldin@techmaster.edu",
                PhoneNumber = "+201044445555",
                Specialization = "Full Stack Web & React Architecture",
                HourlyRate = 550.00m,
                IsActive = true
            },
            new Instructor
            {
                FullName = "Dr. Karim Youssef",
                Email = "k.youssef@techmaster.edu",
                PhoneNumber = "+201055556666",
                Specialization = "Cybersecurity & Ethical Hacking",
                HourlyRate = 680.00m,
                IsActive = true
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
                Capacity = 25,
                Status = TrackStatus.InProgress,
                StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[0].InstructorId
            },
            new TrainingTrack
            {
                Title = "Advanced Applied Machine Learning & Deep Learning",
                Code = "AI-ML-2026",
                Description = "Practical AI engineering with PyTorch, Scikit-Learn, LLM Orchestration, and MLOps pipelines.",
                Price = 10500.00m,
                DurationHours = 140,
                Capacity = 20,
                Status = TrackStatus.InProgress,
                StartDate = new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[1].InstructorId
            },
            new TrainingTrack
            {
                Title = "Enterprise Cloud DevOps & Kubernetes Engineering",
                Code = "DEVOPS-2026",
                Description = "CI/CD pipelines with GitHub Actions, Docker, Kubernetes, Terraform, and AWS/Azure Cloud.",
                Price = 9000.00m,
                DurationHours = 110,
                Capacity = 22,
                Status = TrackStatus.Upcoming,
                StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[2].InstructorId
            },
            new TrainingTrack
            {
                Title = "Full Stack Modern React & Next.js MasterClass",
                Code = "REACT-FS-2026",
                Description = "Full stack modern application development using TypeScript, Next.js, Node.js, and Tailwind CSS.",
                Price = 7500.00m,
                DurationHours = 100,
                Capacity = 30,
                Status = TrackStatus.InProgress,
                StartDate = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 9, 25, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[3].InstructorId
            },
            new TrainingTrack
            {
                Title = "Enterprise Cybersecurity & Penetration Testing",
                Code = "SEC-PEN-2026",
                Description = "Offensive security, network defense, application security, and SOC analytical monitoring.",
                Price = 9500.00m,
                DurationHours = 115,
                Capacity = 18,
                Status = TrackStatus.Upcoming,
                StartDate = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 11, 15, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[4].InstructorId
            },
            new TrainingTrack
            {
                Title = "Legacy Architecture & Monolith Refactoring",
                Code = "ARCH-REF-2026",
                Description = "Strategies for strangler fig pattern, microservice decomposition, and domain-driven design.",
                Price = 6000.00m,
                DurationHours = 60,
                Capacity = 15,
                Status = TrackStatus.Draft,
                StartDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc),
                PrimaryInstructorId = instructors[0].InstructorId
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
                Address = "Sidi Gaber, Alexandria",
                IsActive = true
            },
            new Student
            {
                FullName = "Yara Adel",
                Email = "yara.adel@gmail.com",
                PhoneNumber = "+201100005555",
                DateOfBirth = new DateTime(2003, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                Address = "New Cairo, Cairo",
                IsActive = true
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
            },
            new Student
            {
                FullName = "Hassan Radwan",
                Email = "hassan.radwan@outlook.com",
                PhoneNumber = "+201100008888",
                DateOfBirth = new DateTime(1997, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                Address = "Smouha, Alexandria",
                IsActive = true
            }
        };
        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        // 4. Seed Enrollments
        var enrollments = new List<Enrollment>
        {
            // Enrollment 0: Nada -> NET-BE (Active, Full Payment)
            new Enrollment
            {
                StudentId = students[0].StudentId,
                TrainingTrackId = tracks[0].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 2, 9, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 65.00m,
                Notes = "Excellent performance in Phase 01 and Phase 02 drills."
            },
            // Enrollment 1: Ahmed -> NET-BE (Active, Partial Payment)
            new Enrollment
            {
                StudentId = students[1].StudentId,
                TrainingTrackId = tracks[0].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 3, 10, 30, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 50.00m,
                Notes = "Paid first installment of 5,000 EGP."
            },
            // Enrollment 2: Salma -> NET-BE (Active, ZERO Payment -> Query 18 target!)
            new Enrollment
            {
                StudentId = students[2].StudentId,
                TrainingTrackId = tracks[0].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 4, 11, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 20.00m,
                Notes = "Awaiting financial clearance."
            },
            // Enrollment 3: Omar -> AI-ML (Active, Full Payment)
            new Enrollment
            {
                StudentId = students[3].StudentId,
                TrainingTrackId = tracks[1].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 11, 14, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 75.00m,
                Notes = "Full upfront payment processed."
            },
            // Enrollment 4: Yara -> AI-ML (Pending, ZERO Payment -> Query 18 target!)
            new Enrollment
            {
                StudentId = students[4].StudentId,
                TrainingTrackId = tracks[1].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 12, 16, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Pending,
                ProgressPercentage = 0.00m,
                Notes = "Application received, payment pending."
            },
            // Enrollment 5: Khaled -> DEVOPS (Active, Full Payment)
            new Enrollment
            {
                StudentId = students[5].StudentId,
                TrainingTrackId = tracks[2].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 15, 10, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 30.00m,
                Notes = "Corporate sponsorship payment."
            },
            // Enrollment 6: Mariam -> REACT-FS (Active, Full Payment)
            new Enrollment
            {
                StudentId = students[6].StudentId,
                TrainingTrackId = tracks[3].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 6, 12, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Active,
                ProgressPercentage = 80.00m,
                Notes = "Top frontend performer."
            },
            // Enrollment 7: Hassan -> SEC-PEN (Pending, ZERO Payment -> Query 18 target!)
            new Enrollment
            {
                StudentId = students[7].StudentId,
                TrainingTrackId = tracks[4].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 18, 15, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Pending,
                ProgressPercentage = 0.00m,
                Notes = "Registered for upcoming August batch."
            },
            // Enrollment 8: Ahmed -> AI-ML (Cancelled, ZERO Payment -> must be EXCLUDED in Query 18!)
            new Enrollment
            {
                StudentId = students[1].StudentId,
                TrainingTrackId = tracks[1].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Cancelled,
                ProgressPercentage = 0.00m,
                Notes = "Withdrew before start date."
            },
            // Enrollment 9: Nada -> REACT-FS (Completed, Full Payment)
            new Enrollment
            {
                StudentId = students[0].StudentId,
                TrainingTrackId = tracks[3].TrainingTrackId,
                EnrollmentDate = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Completed,
                ProgressPercentage = 100.00m,
                FinalResult = "Passed with Distinction (98%)",
                Notes = "Completed earlier summer batch."
            }
        };
        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();

        // 5. Seed Payments
        var payments = new List<Payment>
        {
            // Payment 1: Nada for NET-BE (Completed in July 2026)
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
            // Payment 2: Ahmed for NET-BE (Completed in July 2026 - Installment 1)
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
            // Payment 3: Omar for AI-ML (Completed in July 2026)
            new Payment
            {
                EnrollmentId = enrollments[3].EnrollmentId,
                Amount = 10500.00m,
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentDate = new DateTime(2026, 7, 11, 14, 30, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-003",
                Notes = "Direct CIB bank transfer."
            },
            // Payment 4: Khaled for DEVOPS (Completed in July 2026)
            new Payment
            {
                EnrollmentId = enrollments[5].EnrollmentId,
                Amount = 9000.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentDate = new DateTime(2026, 7, 15, 10, 15, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-004",
                Notes = "Mastercard payment."
            },
            // Payment 5: Mariam for REACT-FS (Completed in July 2026)
            new Payment
            {
                EnrollmentId = enrollments[6].EnrollmentId,
                Amount = 7500.00m,
                PaymentMethod = PaymentMethod.Fawry,
                PaymentDate = new DateTime(2026, 7, 6, 12, 45, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-07-005",
                Notes = "Fawry Pay reference #992817."
            },
            // Payment 6: Nada for REACT-FS (Completed in June 2026)
            new Payment
            {
                EnrollmentId = enrollments[9].EnrollmentId,
                Amount = 7500.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentDate = new DateTime(2026, 6, 1, 9, 30, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Completed,
                ReferenceNumber = "PAY-2026-06-001",
                Notes = "Prior batch payment."
            },
            // Payment 7: Pending payment in August 2026
            new Payment
            {
                EnrollmentId = enrollments[1].EnrollmentId,
                Amount = 3500.00m,
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentDate = new DateTime(2026, 8, 5, 10, 0, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Pending,
                ReferenceNumber = "PAY-2026-08-001",
                Notes = "Second installment awaiting bank verification."
            },
            // Payment 8: Failed transaction in July 2026
            new Payment
            {
                EnrollmentId = enrollments[2].EnrollmentId,
                Amount = 8500.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentDate = new DateTime(2026, 7, 4, 11, 15, 0, DateTimeKind.Utc),
                PaymentStatus = PaymentStatus.Failed,
                ReferenceNumber = "PAY-2026-07-FAIL-01",
                Notes = "Card declined - insufficient funds."
            }
        };
        await context.Payments.AddRangeAsync(payments);
        await context.SaveChangesAsync();
    }
}
