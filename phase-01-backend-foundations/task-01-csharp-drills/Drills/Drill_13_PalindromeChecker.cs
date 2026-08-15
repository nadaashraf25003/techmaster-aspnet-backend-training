public class Drill_13_PalindromeChecker
{
    public static void CheckPalindrome()
    {
        Console.WriteLine("Enter a string to check if it's a palindrome:");
        string? input = Console.ReadLine();
        input = input?.Replace(" ", ""); // Remove spaces 

        if (input != null && input == ReverseString(input))
        {
            Console.WriteLine($"Palindrome");
        }
        else
        {
            Console.WriteLine($"Not Palindrome");
        }
    }

    public static string ReverseString(string input)
    {
        char[] charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
