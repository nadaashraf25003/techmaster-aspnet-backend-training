Console.WriteLine("=== C# Drills ===");

Console.WriteLine("1. Temperature Converter");

Console.Write("Choose a drill: ");

string? choice = Console.ReadLine();

switch (choice)
{
    case "1":
        Drill01_TemperatureConverter.Run();
        break;

    case "2":
        // Run Drill 02
        break;

    case "3":
        // Run Drill 03
        break;

    default:
        Console.WriteLine("Invalid choice.");
        break;
}
