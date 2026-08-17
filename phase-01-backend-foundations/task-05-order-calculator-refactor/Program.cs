using OrderCalculatorApp.Services;
using OrderCalculatorApp.UI;

class Program
{
    static void Main(string[] args)
    {
        var calculatorService = new OrderCalculatorService();
        var consoleMenu = new ConsoleMenu(calculatorService);
        consoleMenu.Run();
    }
}
