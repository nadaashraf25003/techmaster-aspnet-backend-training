public class Drill_12_EmailValidator
{
    public static void ValidateEmail()
    {

        Console.WriteLine("Enter your email address:");
        string? email = Console.ReadLine();

        string missingRules = "";
        if (!email.Contains("@"))
        {
            missingRules += "It must contain '@' ";
        }
        if (email.StartsWith("@"))
        {
            missingRules += "It cannot start with '@'. ";
        }
        if (email.EndsWith("@"))
        {
            missingRules += "It cannot end with '@'. ";
        }
        if (!email.Contains("."))
        {
            missingRules += "It must contain a '.' ";
        }
        if (string.IsNullOrEmpty(missingRules))
        {
            Console.WriteLine("Email is valid");
        }
        else
        {
            Console.WriteLine("Email is invalid because: " + missingRules);
        }
    
    }
}
