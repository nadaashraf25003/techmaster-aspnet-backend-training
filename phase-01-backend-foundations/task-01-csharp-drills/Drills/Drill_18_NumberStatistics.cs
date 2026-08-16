public class Drill_18_NumberStatistics
{
    public static void CalculateStatistics()
    {
        Console.WriteLine("Enter numbers separated by comma (,):");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Empty List");
            return;
        }

        string[] elements = input.Split(',');
        int[] array = new int[elements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            if (!int.TryParse(elements[i].Trim(), out array[i]))
            {
                Console.WriteLine("Invalid array element.");
                return;
            }
        }

        int sum = array.Sum();
        double average = array.Average();
        int min = array.Min();
        int max = array.Max();

        if (array.Length == 1)
        {
            Console.WriteLine($"Average: {average}");
            return;
        }
        else
        {
            Console.WriteLine($"Count: {array.Length}, Sum: {sum} ,Average: {average} , Minimum: {min}, Maximum: {max}");
        }

    }
}
