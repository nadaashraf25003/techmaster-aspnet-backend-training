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
Console.WriteLine("11. Duplicate Number Detector");
Console.WriteLine("12. Email Validator");
Console.WriteLine("13. Palindrome Checker");
Console.WriteLine("14. Simple Expense Tracker");
Console.WriteLine("15. Array Rotation");
Console.WriteLine("16. Frequency Counter");
Console.WriteLine("17. Simple Search Engine");
Console.WriteLine("18. Number Statistics");
Console.WriteLine("19. Simple Ticket Price Calculator");
Console.WriteLine("20. Method Refactoring Challenge");

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

    case "11":
        Drill_11_DuplicateNumDetector.HasDuplicate();
        break;

    case "12":
        Drill_12_EmailValidator.ValidateEmail();
        break;

    case "13":
        Drill_13_PalindromeChecker.CheckPalindrome();
        break;

    case "14":
        Drill_14_SimpleExpenseTracker.TrackExpenses();
        break;

    case "15":
        Drill_15_ArrayRotation.RotateArray();
        break;

    case "16":
        Drill_16_FrequencyCounter.CountFrequency();
        break;

    case "17":
        Drill_17_SimpleSearchEngine.Search();
        break;

    case "18":
        Drill_18_NumberStatistics.CalculateStatistics();
        break;

    case "19":
        Drill_19_SimpleTicketPriceCalculator.CalculateTicketPrice();
        break;

    case "20":
        Drill_20_MethodRefactoringChallenge.RefactorMethods();
        break;

    default:
        Console.WriteLine("Invalid choice.");
        break;
}
