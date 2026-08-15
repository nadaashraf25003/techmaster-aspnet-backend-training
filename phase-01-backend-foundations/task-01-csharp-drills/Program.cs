Console.WriteLine("=== C# Drills ===");

Console.WriteLine("1. Temperature Converter");
Console.WriteLine("2. Grade Calculator");
Console.WriteLine("3. Simple Login Validator");
Console.WriteLine("4. Even/Odd Analyzer");

Console.Write("Choose a drill: ");

string? choice = Console.ReadLine();

switch (choice)
{
    case "1":
        Drill01_TemperatureConverter.Run();
        break;

    case "2":
        Drill_02_GradeCalculator.GradeCalculator();
        break;

    case "3":
        Drill_03_SimpleLoginValidator.ValidateLogin();
        break;

    case "4":
        Drill_04_EvenOddAnalyzer.AnalyzeEvenOdd();
        break;

    default:
        Console.WriteLine("Invalid choice.");
        break;
}
