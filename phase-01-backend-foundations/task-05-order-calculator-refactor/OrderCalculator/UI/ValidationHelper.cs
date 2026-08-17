using System;
using OrderCalculatorApp.Models;

namespace OrderCalculatorApp.UI;

public static class ValidationHelper
{
    public static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Input cannot be empty. Please try again.");
            Console.ResetColor();
        }
    }

    public static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (decimal.TryParse(input, out decimal value) && value > 0)
            {
                return value;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter a positive decimal number greater than 0.");
            Console.ResetColor();
        }
    }

    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value > 0)
            {
                return value;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please enter a positive integer greater than 0.");
            Console.ResetColor();
        }
    }

    public static CustomerType ReadCustomerType(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (Enum.TryParse<CustomerType>(input, true, out var customerType) && Enum.IsDefined(typeof(CustomerType), customerType))
            {
                return customerType;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid customer type. Must be Regular, Silver, Gold, or VIP.");
            Console.ResetColor();
        }
    }
}
