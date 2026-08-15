public class Drill01_TemperatureConverter
{
    public static void Run()
    {
        Console.Write("Enter a Celsius value: ");
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal celsius))
        {
            Console.WriteLine("Invalid temperature value.");
            return;
        }

        decimal fahrenheit = celsius * 9 / 5 + 32;
        Console.WriteLine($"{celsius}°C = {fahrenheit:F2}°F");
    }
}
