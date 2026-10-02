namespace TrainingCenter.Api.Configurations;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TechMasterTrainingCenter";
    public string Audience { get; set; } = "TechMasterClients";
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
