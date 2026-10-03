namespace TrainingCenter.Api.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Instructor = "Instructor";
    public const string Student = "Student";

    public const string AdminOrInstructor = "Admin,Instructor";
    public const string AdminOrStudent = "Admin,Student";
    public const string AnyAuthenticated = "Admin,Instructor,Student";
}
