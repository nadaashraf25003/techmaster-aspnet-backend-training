namespace TrainingCenter.Api.Constants;

public static class AuthConstants
{
    public const string BearerScheme = "Bearer";
    public const string StudentIdClaim = "studentId";
    public const string InstructorIdClaim = "instructorId";
    public const string RoleClaim = "role";
    public const string RefreshTokenCookieName = "refreshToken";
    public const string CorrelationIdHeader = "X-Correlation-ID";
}
