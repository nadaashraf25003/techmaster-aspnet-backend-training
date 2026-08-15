# Task 01: C# Drills

## Drill 01: Temperature Converter

This console drill converts a temperature from Celsius to Fahrenheit.

1. The program asks the user to enter a Celsius value.
2. It uses `decimal.TryParse` to safely validate the input, so empty or non-numeric values do not crash the program.
3. When the input is valid, it calculates Fahrenheit with the formula:

   `Fahrenheit = Celsius * 9 / 5 + 32`

4. The result is displayed in a clear format, with the Fahrenheit value shown to two decimal places.

Examples:

- `25` becomes `25°C = 77.00°F`
- `0` becomes `0°C = 32.00°F`
- `-10` becomes `-10°C = 14.00°F`
- `abc` displays `Invalid temperature value.`
