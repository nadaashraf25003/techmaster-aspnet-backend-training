using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;

namespace TrainingCenter.Api.Services.Interfaces;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentResponseDto>> GetAllPagedAsync(EnrollmentQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<EnrollmentDetailResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentRequestDto request, CancellationToken cancellationToken = default);
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentRequestDto request, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface IPaymentService
{
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken = default);
}

public interface IStudentService
{
    Task<PagedResult<StudentResponseDto>> GetAllPagedAsync(PaginationParams pagination, CancellationToken cancellationToken = default);
    Task<StudentResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentResponseDto> CreateAsync(CreateStudentRequestDto request, CancellationToken cancellationToken = default);
    Task<StudentResponseDto> UpdateAsync(int id, UpdateStudentRequestDto request, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface ITrackService
{
    Task<IReadOnlyList<TrackResponseDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<TrackResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TrackResponseDto> CreateAsync(CreateTrackRequestDto request, CancellationToken cancellationToken = default);
}

public interface IInstructorService
{
    Task<IReadOnlyList<InstructorResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InstructorResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InstructorResponseDto> CreateAsync(CreateInstructorRequestDto request, CancellationToken cancellationToken = default);
}

public interface IReportService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrackOccupancyDto>> GetTrackOccupancyAsync(CancellationToken cancellationToken = default);
}
