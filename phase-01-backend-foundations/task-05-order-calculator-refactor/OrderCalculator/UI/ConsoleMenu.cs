using System;
using OrderCalculatorApp.Models;
using OrderCalculatorApp.Services;

namespace OrderCalculatorApp.UI;

public class ConsoleMenu
{
    private readonly OrderCalculatorService _calculatorService;

    public ConsoleMenu(OrderCalculatorService calculatorService)
    {
        _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
    }

    public void Run()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==============================================");
        Console.WriteLine("        Welcome to Order Calculator           ");
        Console.WriteLine("==============================================");
        Console.ResetColor();

        bool keepRunning = true;
        while (keepRunning)
        {
            try
            {
                string customerName = ValidationHelper.ReadString("Enter customer name: ");
                string productName = ValidationHelper.ReadString("Enter product name: ");
                decimal price = ValidationHelper.ReadPositiveDecimal("Enter product price: ");
                int quantity = ValidationHelper.ReadPositiveInt("Enter quantity: ");
                CustomerType customerType = ValidationHelper.ReadCustomerType("Enter customer type (Regular/Silver/Gold/VIP): ");

                Customer customer = new Customer(customerName, customerType);
                Order order = new Order(customer, productName, price, quantity);

                OrderCalculationResult result = _calculatorService.Calculate(order);

                ReceiptPrinter.PrintReceipt(order, result);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.ResetColor();
            }

            Console.Write("Calculate another order? (y/n): ");
            string? choice = Console.ReadLine()?.Trim().ToLower();
            keepRunning = choice == "y" || choice == "yes";
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Thank you for using Order Calculator. Goodbye!");
        Console.ResetColor();
    }
}
