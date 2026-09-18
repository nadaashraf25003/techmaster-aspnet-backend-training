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
    Task<PagedResult<StudentListItemResponse>> GetStudentsAsync(StudentFilterParams filters);
    Task<StudentDetailsResponse> GetStudentByIdAsync(int id);
    Task<StudentDetailsResponse> CreateStudentAsync(CreateStudentRequest request);
    Task<StudentDetailsResponse> UpdateStudentAsync(int id, UpdateStudentRequest request);
    Task<bool> SoftDeleteStudentAsync(int id);
    Task<List<StudentEnrollmentSummaryDto>> GetStudentEnrollmentsAsync(int studentId);
}

public interface IInstructorService
{
    Task<List<InstructorResponse>> GetAllInstructorsAsync(bool? isActive = null);
    Task<InstructorDetailsResponse> GetInstructorByIdAsync(int id);
    Task<InstructorResponse> CreateInstructorAsync(CreateInstructorRequest request);
    Task<InstructorResponse> UpdateInstructorAsync(int id, UpdateInstructorRequest request);
    Task<List<InstructorTrackSummaryDto>> GetInstructorTracksAsync(int instructorId);
}

public interface ITrackService
{
    Task<PagedResult<TrackListItemResponse>> GetTracksAsync(TrackFilterParams filters);
    Task<TrackDetailsResponse> GetTrackByIdAsync(int id);
    Task<TrackDetailsResponse> CreateTrackAsync(CreateTrackRequest request);
    Task<TrackDetailsResponse> UpdateTrackAsync(int id, UpdateTrackRequest request);
    Task<bool> SoftDeleteTrackAsync(int id);
    Task<List<TrackStudentDto>> GetTrackStudentsAsync(int trackId);
}

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentListItemResponse>> GetEnrollmentsAsync(EnrollmentFilterParams filters);
    Task<EnrollmentDetailsResponse> GetEnrollmentByIdAsync(int id);
    Task<EnrollmentDetailsResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request);
    Task<EnrollmentDetailsResponse> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentStatusRequest request);
}

public interface IPaymentService
{
    Task<PagedResult<PaymentResponse>> GetPaymentsAsync(PaymentFilterParams filters);
    Task<PaymentResponse> GetPaymentByIdAsync(int id);
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    Task<List<PaymentResponse>> GetPaymentsByEnrollmentIdAsync(int enrollmentId);
    Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request);
}

public interface IReportService
{
    Task<DashboardSummaryReportDto> GetDashboardSummaryAsync();
    Task<List<UnpaidEnrollmentReportDto>> GetUnpaidEnrollmentsAsync();
    Task<List<TrackCapacityReportDto>> GetTrackCapacityReportAsync();
    Task<RevenueSummaryReportDto> GetRevenueSummaryAsync();
    Task<List<RevenueByTrackReportDto>> GetRevenueByTrackReportAsync();
    Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadReportAsync();
}
