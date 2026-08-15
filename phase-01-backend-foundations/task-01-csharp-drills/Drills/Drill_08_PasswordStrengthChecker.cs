public class Drill_08_PasswordStrengthChecker
{
    public static void CheckPasswordStrength()
    {
        Console.Write("Enter a password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        // Approach: boolean flags + single pass through characters
        bool hasLength = password.Length >= 8;
        bool hasUpper = false;
        bool hasLower = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecial = true;
        }

        // Collect missing rules
        List<string> missingRules = new List<string>();
        if (!hasLength) missingRules.Add("length >= 8");
        if (!hasUpper) missingRules.Add("uppercase letter");
        if (!hasLower) missingRules.Add("lowercase letter");
        if (!hasDigit) missingRules.Add("digit");
        if (!hasSpecial) missingRules.Add("special character");

        if (missingRules.Count == 0)
        {
            Console.WriteLine("Strong");
        }
        else
        {
            Console.WriteLine($"Weak - missing {string.Join(", ", missingRules)}");
        }
    }
}
