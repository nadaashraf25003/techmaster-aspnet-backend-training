using System;
using OrderCalculatorApp.Models;
using OrderCalculatorApp.Services;

namespace OrderCalculatorApp.UI;

public static class ReceiptPrinter
{
    public static void PrintReceipt(Order order, OrderCalculationResult result)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n==============================================");
        Console.WriteLine("               ORDER RECEIPT                  ");
        Console.WriteLine("==============================================");
        Console.ResetColor();

        Console.WriteLine($"Date:           {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Customer:       {order.Customer.Name}");
        Console.WriteLine($"Customer Type:  {order.Customer.Type}");
        Console.WriteLine("----------------------------------------------");

        Console.WriteLine($"Product:        {order.ProductName}");
        Console.WriteLine($"Price:          {order.Price:C}");
        Console.WriteLine($"Quantity:       {order.Quantity}");
        Console.WriteLine("----------------------------------------------");

        Console.WriteLine($"Subtotal:       {result.Subtotal:C}");
        
        if (result.Discount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Discount:      -{result.Discount:C}");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Discount:       {result.Discount:C}");
        }
        
        Console.WriteLine($"After Discount: {result.AfterDiscount:C}");
        Console.WriteLine($"Tax (14%):      {result.Tax:C}");

        if (result.Shipping == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Shipping:       FREE");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Shipping:       {result.Shipping:C}");
        }

        Console.WriteLine("----------------------------------------------");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Final Total:    {result.FinalTotal:C}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==============================================\n");
        Console.ResetColor();
    }
}
