public class Drill_19_SimpleTicketPriceCalculator
{
    public static void CalculateTicketPrice()
    {
        Console.WriteLine("Welcome to the Ticket Price Calculator!");
        int basePrice = 100;
        Console.Write("Enter your age: ");
        string? ageInput = Console.ReadLine();

        Console.Write("Are you a student? (yes/no): ");
        string? studentInput = Console.ReadLine();

        int age;

        if (string.IsNullOrWhiteSpace(ageInput) || !int.TryParse(ageInput, out age))
        {
            Console.WriteLine("Invalid age input.");
            return;
        }
        else
        {
            age = int.Parse(ageInput);
        }

        decimal discount = 0; // Initialize discount to 0 by default

        if(age < 12) discount = Math.Max(discount, 0.5m); // 50% discount for children under 12
        if(age >= 65) discount = Math.Max(discount, 0.3m); // 30% discount for seniors 65 and older
        if(studentInput != null && studentInput.Trim().ToLower() == "yes") discount = Math.Max(discount, 0.2m); // 20% discount for students

        decimal finalPrice = basePrice * (1 - discount);
        Console.WriteLine($"The final price is: {finalPrice:C}");




    }
}
