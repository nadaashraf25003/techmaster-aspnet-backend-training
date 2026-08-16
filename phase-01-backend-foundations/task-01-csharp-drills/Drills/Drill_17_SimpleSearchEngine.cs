public class Drill_17_SimpleSearchEngine
{
    public static void Search()
    {
        string[] names = { "Ali Hassan", "Khaled Ali", "Omar", "Sara", "Mohammed Ali", "Fatima" };

        Console.Write("Enter search keyword: ");
        string? keyword = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("No results found");
            return;
        }

        var matches = names
            .Where(name => name != null && name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();

        if (matches.Count == 0)
        {
            Console.WriteLine("No results found");
            return;
        }

        foreach (var name in matches)
        {
            Console.WriteLine(name);
        }
    }
}
