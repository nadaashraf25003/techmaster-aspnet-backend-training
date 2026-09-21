using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Implementations;
using Xunit;

namespace TrainingCenter.Tests;

public class EnrollmentRefactorTests : IDisposable
{
    private readonly TrainingCenterDbContext _context;
    private readonly EnrollmentService _enrollmentService;

    public EnrollmentRefactorTests()
    {
        var options = new DbContextOptionsBuilder<TrainingCenterDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new TrainingCenterDbContext(options);
        _enrollmentService = new EnrollmentService(_context);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var instructor = new Instructor
        {
            Id = 1,
            FullName = "Eng. Ahmed Hassan",
            Email = "ahmed.hassan@test.com",
            Specialization = "Backend",
            HourlyRate = 500m,
            IsActive = true
        };
        _context.Instructors.Add(instructor);

        var tracks = new List<TrainingTrack>
        {
            new()
            {
                Id = 1,
                Code = "NET-101",
                Name = "ASP.NET Core Masterclass",
                Price = 6000m,
                Capacity = 20,
                Status = TrackStatus.Active,
                InstructorId = 1
            },
            new()
            {
                Id = 2,
                Code = "FULL-202",
                Name = "Full Track (Capacity 1)",
                Price = 5000m,
                Capacity = 1, // Cap is 1
                Status = TrackStatus.Active,
                InstructorId = 1
            },
            new()
            {
                Id = 3,
                Code = "CANCELLED-303",
                Name = "Cancelled Track",
                Price = 4000m,
                Capacity = 10,
                Status = TrackStatus.Cancelled,
                InstructorId = 1
            }
        };
        _context.TrainingTracks.AddRange(tracks);

        var students = new List<Student>
        {
            new()
            {
                Id = 1,
                FullName = "Sara Ahmed",
                Email = "sara.ahmed@test.com",
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new()
            {
                Id = 2,
                FullName = "Omar Ali",
                Email = "omar.ali@test.com",
                DateOfBirth = new DateTime(1999, 5, 5, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new()
            {
                Id = 3,
                FullName = "Inactive Student",
                Email = "inactive@test.com",
                DateOfBirth = new DateTime(1995, 3, 3, 0, 0, 0, DateTimeKind.Utc),
                IsActive = false
            }
        };
        _context.Students.AddRange(students);

        var existingEnrollment = new Enrollment
        {
            Id = 1,
            StudentId = 1,
            TrainingTrackId = 2, // Enrolled in Track 2 (which has capacity 1)
            EnrollmentDate = DateTime.UtcNow.AddDays(-5),
            Status = EnrollmentStatus.Active
        };
        _context.Enrollments.Add(existingEnrollment);

        var payment = new Payment
        {
            Id = 1,
            EnrollmentId = 1,
            Amount = 2000m,
            PaymentDate = DateTime.UtcNow.AddDays(-4),
            PaymentMethod = PaymentMethod.Cash,
            Status = PaymentStatus.Paid,
            ReferenceNumber = "PAY-INIT-001"
        };
        _context.Payments.Add(payment);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllPagedAsync_ShouldReturnProjectedDtos_WithoutCircularReferences()
    {
        // Act
        var result = await _enrollmentService.GetAllPagedAsync(new EnrollmentQueryParameters { PageNumber = 1, PageSize = 10 });

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items[0].StudentName.Should().Be("Sara Ahmed");
        result.Items[0].TrackCode.Should().Be("FULL-202");
        result.Items[0].TotalPaidAmount.Should().Be(2000m);
        result.Items[0].RemainingBalance.Should().Be(3000m); // 5000 - 2000
        result.Items[0].IsFullyPaid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldEnforceDuplicateActiveEnrollmentGuard_ThrowsConflictException()
    {
        // Student 1 is already actively enrolled in Track 2
        var request = new CreateEnrollmentRequestDto
        {
            StudentId = 1,
            TrainingTrackId = 2
        };

        // Act & Assert
        var act = () => _enrollmentService.CreateAsync(request);
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already actively enrolled*");
    }

    [Fact]
    public async Task CreateAsync_ShouldEnforceTrackCapacityLimit_ThrowsBusinessRuleException()
    {
        // Track 2 capacity is 1, and Student 1 is already enrolled
        // Attempting to enroll Student 2 in Track 2 should fail due to capacity
        var request = new CreateEnrollmentRequestDto
        {
            StudentId = 2,
            TrainingTrackId = 2
        };

        // Act & Assert
        var act = () => _enrollmentService.CreateAsync(request);
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*reached maximum capacity*");
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectInactiveStudent_ThrowsBusinessRuleException()
    {
        // Student 3 is Inactive
        var request = new CreateEnrollmentRequestDto
        {
            StudentId = 3,
            TrainingTrackId = 1
        };

        // Act & Assert
        var act = () => _enrollmentService.CreateAsync(request);
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*inactive*");
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectNonActiveTrack_ThrowsBusinessRuleException()
    {
        // Track 3 is Cancelled
        var request = new CreateEnrollmentRequestDto
        {
            StudentId = 2,
            TrainingTrackId = 3
        };

        // Act & Assert
        var act = () => _enrollmentService.CreateAsync(request);
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Cannot enroll in track*Cancelled*");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateEnrollment_WhenValid()
    {
        // Student 2 enrolling in Track 1 (active, has capacity)
        var request = new CreateEnrollmentRequestDto
        {
            StudentId = 2,
            TrainingTrackId = 1
        };

        // Act
        var result = await _enrollmentService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.StudentId.Should().Be(2);
        result.TrainingTrackId.Should().Be(1);
        result.Status.Should().Be(EnrollmentStatus.Active);
        result.TotalPaidAmount.Should().Be(0m);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldValidatePositiveAmount_ThrowsValidationException()
    {
        var request = new ProcessPaymentRequestDto
        {
            EnrollmentId = 1,
            Amount = -100m,
            PaymentMethod = PaymentMethod.CreditCard
        };

        // Act & Assert
        var act = () => _enrollmentService.ProcessPaymentAsync(request);
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*greater than zero*");
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldPreventOverpayment_ThrowsBusinessRuleException()
    {
        // Track 2 price = 5000, existing paid = 2000, remaining = 3000
        // Attempting to pay 4000 should throw BusinessRuleException
        var request = new ProcessPaymentRequestDto
        {
            EnrollmentId = 1,
            Amount = 4000m,
            PaymentMethod = PaymentMethod.CreditCard
        };

        // Act & Assert
        var act = () => _enrollmentService.ProcessPaymentAsync(request);
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*exceeds remaining balance*");
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldProcessValidPayment_AndCalculateRemainingBalance()
    {
        // Remaining balance is 3000, paying exactly 3000
        var request = new ProcessPaymentRequestDto
        {
            EnrollmentId = 1,
            Amount = 3000m,
            PaymentMethod = PaymentMethod.InstaPay,
            Notes = "Final payment"
        };

        // Act
        var result = await _enrollmentService.ProcessPaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(3000m);
        result.Status.Should().Be(PaymentStatus.Paid);
        result.ReferenceNumber.Should().StartWith("PAY-");

        // Verify updated detail calculation
        var detail = await _enrollmentService.GetByIdAsync(1);
        detail.TotalPaidAmount.Should().Be(5000m);
        detail.RemainingBalance.Should().Be(0m);
        detail.IsFullyPaid.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteAsync_ShouldMarkIsDeleted_AndPreserveAuditTrail()
    {
        // Act
        await _enrollmentService.SoftDeleteAsync(1);

        // Assert
        var deletedInDb = await _context.Enrollments.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == 1);
        deletedInDb.Should().NotBeNull();
        deletedInDb!.IsDeleted.Should().BeTrue();
        deletedInDb.DeletedAtUtc.Should().NotBeNull();
        deletedInDb.Status.Should().Be(EnrollmentStatus.Cancelled);

        // Assert that normal query filter excludes it
        var queryableCount = await _context.Enrollments.CountAsync();
        queryableCount.Should().Be(0);

        // Attempting to GetById should throw NotFoundException
        var act = () => _enrollmentService.GetByIdAsync(1);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
