public class Drill_15_ArrayRotation
{
    public static void RotateArray()
    {
        Console.WriteLine("Enter array elements separated by comma (,):");
        string? input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input.");
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

        // Edge case: single element or empty array
        if (array.Length <= 1)
        {
            Console.WriteLine("Rotated array:");
            Console.WriteLine(string.Join(", ", array));
            return;
        }

        // Approach: Store last element, shift all elements right, place last at first
        int temp = array[array.Length - 1];
        
        // Loop from last index down to 1
        for (int i = array.Length - 1; i > 0; i--)
        {
            array[i] = array[i - 1];
        }
        
        array[0] = temp;

        // Print result
        Console.WriteLine("Rotated array:");
        Console.WriteLine(string.Join(", ", array));
    }
}
