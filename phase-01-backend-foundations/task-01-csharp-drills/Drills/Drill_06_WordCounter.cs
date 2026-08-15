public class Drill_06_WordCounter
{
    public static void WordCounter()
    {
        Console.Write("Enter a sentence: ");
        string? sentence = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(sentence))
        {
            Console.WriteLine("Sentence cannot be empty.");
            return;
        }

        string[] words = sentence.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;

        Console.WriteLine($"Word count: {wordCount}");
    }

    
}