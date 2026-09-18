using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;

namespace TrainingCenter.Api.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentResponse>> GetStudentsAsync(StudentFilterParams filters);
    Task<StudentResponse> GetStudentByIdAsync(int id);
    Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request);
    Task<StudentResponse> UpdateStudentAsync(int id, UpdateStudentRequest request);
    Task DeleteStudentAsync(int id);
}

public interface IInstructorService
{
    Task<PagedResult<InstructorResponse>> GetInstructorsAsync(InstructorFilterParams filters);
    Task<InstructorResponse> GetInstructorByIdAsync(int id);
    Task<InstructorResponse> CreateInstructorAsync(CreateInstructorRequest request);
    Task<InstructorResponse> UpdateInstructorAsync(int id, UpdateInstructorRequest request);
    Task DeleteInstructorAsync(int id);
}

public interface ITrackService
{
    Task<PagedResult<TrackResponse>> GetTracksAsync(TrackFilterParams filters);
    Task<TrackResponse> GetTrackByIdAsync(int id);
    Task<TrackResponse> CreateTrackAsync(CreateTrackRequest request);
    Task<TrackResponse> UpdateTrackAsync(int id, UpdateTrackRequest request);
    Task DeleteTrackAsync(int id);
}

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentListItemResponse>> GetEnrollmentsAsync(EnrollmentFilterParams filters);
    Task<EnrollmentDetailsResponse> GetEnrollmentByIdAsync(int id);
    Task<EnrollmentDetailsResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request);
    Task<EnrollmentDetailsResponse> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentStatusRequest request);
    Task DeleteEnrollmentAsync(int id);
}

public interface IPaymentService
{
    Task<PagedResult<PaymentResponse>> GetPaymentsAsync(PaymentFilterParams filters);
    Task<PaymentResponse> GetPaymentByIdAsync(int id);
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request);
}

public interface IReportService
{
    Task<RevenueSummaryReportDto> GetRevenueSummaryAsync();
    Task<List<RevenueByTrackReportDto>> GetRevenueByTrackReportAsync();
    Task<List<TopTrackReportDto>> GetTopTracksByEnrollmentAsync(int count = 5);
    Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadReportAsync();
    Task<List<StudentWithoutPaymentReportDto>> GetStudentsWithoutPaymentsAsync();
    Task<DashboardSummaryReportDto> GetDashboardSummaryAsync();
    Task<List<TrackCapacityReportDto>> GetTrackCapacityReportAsync();
}
