public class Drill_04_EvenOddAnalyzer
{
    public static void AnalyzeEvenOdd()
    {
        Console.Write("How many numbers will you enter? ");
        string? countInput = Console.ReadLine();

        if (!int.TryParse(countInput, out int count) || count <= 0)
        {
            Console.WriteLine("Count must be a positive whole number.");
            return;
        }

        List<int> evenNumbers = new();
        List<int> oddNumbers = new();

        for (int i = 1; i <= count; i++)
        {
            Console.Write($"Enter number {i} of {count}: ");
            string? numberInput = Console.ReadLine();

            if (!int.TryParse(numberInput, out int number))
            {
                Console.WriteLine("Invalid number. Please enter this number again.");
                i--;
                continue;
            }

            if (number % 2 == 0)
            {
                evenNumbers.Add(number);
            }
            else
            {
                oddNumbers.Add(number);
            }
        }
        if (evenNumbers.Count == 0)
        {
            Console.WriteLine("Even list should be empty");
            
            return;
        }
        if (oddNumbers.Count == 0)
        {
            Console.WriteLine("Odd list should be empty");
            return;
        }
        if (evenNumbers.Count != 0 && oddNumbers.Count != 0)
        {
             Console.WriteLine($"Even ({evenNumbers.Count}): {string.Join(",", evenNumbers)} | Odd ({oddNumbers.Count}): {string.Join(",", oddNumbers)}");
            return;
        }
       
    }
}
