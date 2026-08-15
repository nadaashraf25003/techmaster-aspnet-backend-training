Console.WriteLine("=== C# Drills ===");

Console.WriteLine("1. Temperature Converter");
Console.WriteLine("2. Grade Calculator");
Console.WriteLine("3. Simple Login Validator");
Console.WriteLine("4. Even/Odd Analyzer");
Console.WriteLine("5. Maximum and Minimum Finder");
Console.WriteLine("6. Word Counter");
Console.WriteLine("7. Name Formatter");
Console.WriteLine("8. Password Strength Checker");
Console.WriteLine("9. Shopping Cart Total");
Console.WriteLine("10. Simple ATM Menu");

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

    case "5":
        Drill_05_MaxMinFinder.FindMaxMin();
        break;

    case "6":
        Drill_06_WordCounter.WordCounter();
        break;

    case "7":
        Drill_07_NameFormatter.FormatName();
        break;

    case "8":
        Drill_08_PasswordStrengthChecker.CheckPasswordStrength();
        break;

    case "9":
        Drill_09_ShoppingCartTotal.CalculateShoppingCartTotal();
        break;

    case "10":
        Drill_10_SimpleAtmMenu.ShowAtmMenu();
        break;

    default:
        Console.WriteLine("Invalid choice.");
        break;
}
