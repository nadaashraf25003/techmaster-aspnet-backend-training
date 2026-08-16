public class Drill_16_FrequencyCounter
{
    public static void CountFrequency()
    {
        Console.WriteLine("Enter elements separated by comma (,):");
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

        Dictionary<int, int> frequency = new Dictionary<int, int>();

        foreach (int num in array)
        {
            if (frequency.ContainsKey(num))
            {
                frequency[num]++;
            }
            else
            {
                frequency[num] = 1;
            }
        }

        foreach (var kvp in frequency)
        {
            Console.WriteLine($"{kvp.Key}=> {kvp.Value},");
        }
    }
}
