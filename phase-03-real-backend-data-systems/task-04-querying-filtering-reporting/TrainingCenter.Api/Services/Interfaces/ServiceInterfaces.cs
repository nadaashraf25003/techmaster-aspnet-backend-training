using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;

namespace TrainingCenter.Api.Services.Interfaces;

public interface IReportService
{
    /// <summary>
    /// Query 14: Return total revenue, paid count, pending count, and failed count.
    /// </summary>
    Task<RevenueSummaryReportDto> GetRevenueSummaryAsync();

    /// <summary>
    /// Query 15: Group paid payments by track and return track title, total paid, enrollment count.
    /// </summary>
    Task<List<RevenueByTrackReportDto>> GetRevenueByTrackReportAsync();

    /// <summary>
    /// Query 16: Return tracks ordered by active enrollment count (top N, default 5).
    /// </summary>
    Task<List<TopTrackReportDto>> GetTopTracksByEnrollmentAsync(int count = 5);

    /// <summary>
    /// Query 17: Return each instructor with number of tracks and active students.
    /// </summary>
    Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadReportAsync();

    /// <summary>
    /// Query 18: Return students with active/pending enrollments but no payment (excluding cancelled).
    /// </summary>
    Task<List<StudentWithoutPaymentReportDto>> GetStudentsWithoutPaymentsAsync();

    /// <summary>
    /// Query 20: Return high-level dashboard numbers in one response.
    /// </summary>
    Task<DashboardSummaryReportDto> GetDashboardSummaryAsync();

    /// <summary>
    /// Supporting report: Unpaid / partially paid enrollments.
    /// </summary>
    Task<List<UnpaidEnrollmentReportDto>> GetUnpaidEnrollmentsAsync();

    /// <summary>
    /// Supporting report: Track capacity and available seats.
    /// </summary>
    Task<List<TrackCapacityReportDto>> GetTrackCapacityReportAsync();
}

public interface IPaymentService
{
    /// <summary>
    /// Query 13: Return payments inside date range (validates from &lt;= to) with status/method filters.
    /// </summary>
    Task<PagedResult<PaymentResponse>> GetPaymentsAsync(PaymentFilterParams filters);

    Task<PaymentResponse> GetPaymentByIdAsync(int id);
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request);
}

public interface IEnrollmentService
{
    /// <summary>
    /// Query 19: Dynamic conditional IQueryable filter for enrollments.
    /// </summary>
    Task<PagedResult<EnrollmentListItemResponse>> GetEnrollmentsAsync(EnrollmentFilterParams filters);

    Task<EnrollmentDetailsResponse> GetEnrollmentByIdAsync(int id);
    Task<EnrollmentDetailsResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request);
    Task<EnrollmentDetailsResponse> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentStatusRequest request);
    Task DeleteEnrollmentAsync(int id);
}

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
