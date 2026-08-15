public class Drill_11_DuplicateNumDetector
{
    public static void HasDuplicate()
    {
        Console.WriteLine("Enter a list of numbers separated by commas:");
        string? input = Console.ReadLine();
        int[] numbersDuplicate = new int[0];
        if (input != null)
        {
            string[] numbers = input.Split(',');
            if (numbers.Length > 0)
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    for (int j = i + 1; j < numbers.Length; j++)
                    {
                        if (numbers[i] == numbers[j])
                        {
                            numbersDuplicate = numbersDuplicate.Append(int.Parse(numbers[i])).ToArray();
                        }
                    }

                }
            }
            if (numbersDuplicate.Length > 0)
            {
                Console.WriteLine($"Duplicates: {string.Join(", ", numbersDuplicate)}");
            }
            else
            {
                Console.WriteLine("No duplicate numbers found.");
            }
        }
        else
        {
            Console.WriteLine("Input cannot be empty.");
        }



    }
}