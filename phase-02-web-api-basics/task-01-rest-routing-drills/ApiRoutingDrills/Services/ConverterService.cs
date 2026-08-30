namespace ApiRoutingDrills.Services;

public class ConverterService : IConverterService
{
    public (decimal Fahrenheit, string Formula) ConvertCelsiusToFahrenheit(decimal celsius)
    {
        decimal fahrenheit = (celsius * 9m / 5m) + 32m;
        return (Math.Round(fahrenheit, 2), "F = (C * 9/5) + 32");
    }
}
