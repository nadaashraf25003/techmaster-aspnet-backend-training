public class Drill_07_NameFormatter
{
    public static void FormatName()
    {
        Console.Write("Enter your full name: ");
        string? fullName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        string[] nameParts = fullName.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < nameParts.Length; i++)
        {
            nameParts[i] = char.ToUpper(nameParts[i][0]) + nameParts[i].Substring(1).ToLower();
        }

        string formattedName = string.Join(" ", nameParts);
        Console.WriteLine(formattedName);
    }
}
