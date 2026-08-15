public class Drill_03_SimpleLoginValidator
{
    public static void ValidateLogin()
    {
        var user = new { name = "admin", password = "1234" };
        var attempts = 3;
        while (attempts > 0)
        {
            Console.Write("Enter username: ");
            string? username = Console.ReadLine()?.ToLower();

            Console.Write("Enter password: ");
            string? password = Console.ReadLine();


            if (username == user.name && password == user.password)
            {
                Console.WriteLine("Login successful!");
                return;
            }
            else
            {
                attempts--;
                Console.WriteLine($"Invalid username or password. Attempts left: {attempts}");
            }
        }
        Console.WriteLine("Account locked. Too many failed attempts.");
    }
}