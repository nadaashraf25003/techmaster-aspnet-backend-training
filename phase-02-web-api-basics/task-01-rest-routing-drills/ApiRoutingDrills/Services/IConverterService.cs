namespace ApiRoutingDrills.Services;

public interface IConverterService
{
    (decimal Fahrenheit, string Formula) ConvertCelsiusToFahrenheit(decimal celsius);
}
