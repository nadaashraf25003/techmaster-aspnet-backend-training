public class Drill_20_MethodRefactoringChallenge
{
    public static void RefactorMethods()
    {
        Console.WriteLine("=== Method Refactoring Challenge ===\n");

        // Refactored Drill 2: Grade Calculator
        Console.WriteLine("--- Refactored Grade Calculator ---");
        RunRefactoredGradeCalculator();
        Console.WriteLine();

        // Refactored Drill 10: ATM Menu
        Console.WriteLine("--- Refactored ATM Menu ---");
        RunRefactoredAtmMenu();
        Console.WriteLine();

        // Refactored Drill 1: Temperature Converter
        Console.WriteLine("--- Refactored Temperature Converter ---");
        RunRefactoredTemperatureConverter();
    }
    // ------------------------------------- Refactored Drill 2: Grade Calculator -------------------------------------
    #region Refactored Drill 2: Grade Calculator
    private static void RunRefactoredGradeCalculator()
    {
        decimal score = ReadScore();
        if (!ValidateScore(score))
        {
            return; // Validation failed, exit early
        }

        string grade = CalculateGrade(score);
        PrintGrade(grade);
    }
    private static decimal ReadScore()
    {
        Console.Write("Enter your score (0-100): ");
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal score))
        {
            Console.WriteLine("Invalid score value.");
            return -1;
        }

        return score;
    }
    private static bool ValidateScore(decimal score)
    {
        if (score < 0 || score > 100)
        {
            Console.WriteLine("Score must be between 0 and 100");
            return false;
        }
        return true;
    }
    private static string CalculateGrade(decimal score)
    {
        if (score >= 90)
            return "A";
        else if (score >= 80)
            return "B";
        else if (score >= 70)
            return "C";
        else if (score >= 60)
            return "D";
        else
            return "F";
    }
    private static void PrintGrade(string grade)
    {
        Console.WriteLine($"Grade: {grade}");
    }
    #endregion



   // ------------------------------------- Refactored Drill 10: ATM Menu -------------------------------------
    #region Refactored Drill 10: ATM Menu

    private static void RunRefactoredAtmMenu()
    {
        int balance = 1000; // Initial balance

        while (true)
        {
            ShowMenu();
            string? userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                PrintBalance(balance);
            }
            else if (userChoice == "2")
            {
                balance = ProcessDeposit(balance);
            }
            else if (userChoice == "3")
            {
                balance = ProcessWithdrawal(balance);
            }
            else if (userChoice == "4")
            {
                Console.WriteLine("Thank you for using the Simple ATM. Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid option.");
            }

            Console.WriteLine(); // Add spacing between operations
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Welcome to the Simple ATM");
        Console.WriteLine("Please select an option:");
        Console.WriteLine("1. Check Balance");
        Console.WriteLine("2. Deposit");
        Console.WriteLine("3. Withdraw");
        Console.WriteLine("4. Exit");
    }

    private static int ProcessDeposit(int currentBalance)
    {
        Console.Write("Enter amount to deposit: ");
        string? depositInput = Console.ReadLine();

        if (int.TryParse(depositInput, out int depositAmount) && depositAmount > 0)
        {
            int newBalance = currentBalance + depositAmount;
            Console.WriteLine($"Deposit successful. New balance: {newBalance}");
            return newBalance;
        }
        else
        {
            Console.WriteLine("Invalid deposit amount.");
            return currentBalance;
        }
    }
    private static int ProcessWithdrawal(int currentBalance)
    {
        Console.Write("Enter amount to withdraw: ");
        string? withdrawInput = Console.ReadLine();

        if (int.TryParse(withdrawInput, out int withdrawAmount) && withdrawAmount > 0)
        {
            if (withdrawAmount <= currentBalance)
            {
                int newBalance = currentBalance - withdrawAmount;
                Console.WriteLine($"Withdrawal successful. New balance: {newBalance}");
                return newBalance;
            }
            else
            {
                Console.WriteLine("Insufficient balance.");
                return currentBalance;
            }
        }
        else
        {
            Console.WriteLine("Invalid withdrawal amount.");
            return currentBalance;
        }
    }
    private static void PrintBalance(int balance)
    {
        Console.WriteLine($"Your balance is: {balance}");
    }
    #endregion


   // ------------------------------------- Refactored Drill 1: Temperature Converter -------------------------------------
    #region Refactored Drill 1: Temperature Converter
    private static void RunRefactoredTemperatureConverter()
    {
        decimal celsius = ReadCelsiusInput();
        if (celsius == -999m) // Sentinel value indicating invalid input
        {
            return;
        }

        decimal fahrenheit = ConvertCelsiusToFahrenheit(celsius);
        PrintTemperatureConversion(celsius, fahrenheit);
    }

    private static decimal ReadCelsiusInput()
    {
        Console.Write("Enter a Celsius value: ");
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal celsius))
        {
            Console.WriteLine("Invalid temperature value.");
            return -999m;
        }

        return celsius;
    }

    private static decimal ConvertCelsiusToFahrenheit(decimal celsius)
    {
        return celsius * 9 / 5 + 32;
    }
    private static void PrintTemperatureConversion(decimal celsius, decimal fahrenheit)
    {
        Console.WriteLine($"{celsius}°C = {fahrenheit:F2}°F");
    }
    #endregion
}
