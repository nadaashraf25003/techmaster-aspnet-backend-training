public class Drill_05_MaxMinFinder
{
    public static void FindMaxMin()
    {
        Console.WriteLine("Enter a list of integers separated by comma (,):");
        string? input = Console.ReadLine();
        if (input == null) return;

        string[] numbersAsStrings = input.Split(',');
        int[] numbers = Array.ConvertAll(numbersAsStrings, int.Parse);

        // Approach 1: Manual loop
        int min = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] < min)
            {
                min = numbers[i];
            }
        }

        int max = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
            {
                max = numbers[i];
            }
        }

        Console.WriteLine($"Approach 1 (Manual loop) -> Max: {max} | Min: {min}");

        // Approach 2: LINQ
        var linqMin = numbers.Min();
        var linqMax = numbers.Max();

        Console.WriteLine($"Approach 2 (LINQ)        -> Max: {linqMax} | Min: {linqMin}");

        // Comparison
        bool sameResults = (max == linqMax) && (min == linqMin);
        Console.WriteLine($"Results match: {sameResults}");
    }
}