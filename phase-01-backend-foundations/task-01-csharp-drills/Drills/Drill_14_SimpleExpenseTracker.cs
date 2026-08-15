public class Drill_14_SimpleExpenseTracker
{
    public static void TrackExpenses()
    {
        Console.WriteLine("Enter the number of expenses:");
        if (!int.TryParse(Console.ReadLine(), out int numExpenses) || numExpenses <= 0)
        {
            Console.WriteLine("Invalid number of expenses.");
            return;
        }

        List<(string Name, decimal Amount)> expenses = new List<(string, decimal)>();

        for (int i = 0; i < numExpenses; i++)
        {
            Console.Write($"Enter expense {i + 1} name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Invalid expense name.");
                return;
            }

            Console.Write($"Enter amount for {name}: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid expense amount.");
                return;
            }

            expenses.Add((name, amount));
        }

        // Calculate statistics
        decimal total = expenses.Sum(e => e.Amount);
        decimal average = total / expenses.Count;
        var highestExpense = expenses.OrderByDescending(e => e.Amount).First();

        // Print summary
        Console.WriteLine("\n=== Expense Summary ===");
        Console.WriteLine($"Total: {total}");
        Console.WriteLine($"Average: {average:F2}");
        Console.WriteLine($"Highest Expense: {highestExpense.Name} ({highestExpense.Amount})");
    }
}

