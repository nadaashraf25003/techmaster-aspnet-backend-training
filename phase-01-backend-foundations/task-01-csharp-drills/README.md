# Task 01 - C# Logic Drill Pack

| Drill No. | Drill Name | Topic | Status | Notes |
|---|---|---|---|---|
| 01 | Temperature Converter | Parsing / Calculation | Done | Handles invalid input |
| 02 | Grade Calculator | Conditions | Done | Validates scores from 0 to 100 |
| 03 | Login Validator | Loops / Strings | Done | Max 3 attempts |
| 04 | Even/Odd Analyzer | Conditions / Modulo | Done | Handles invalid input |

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

## Drill 02: Grade Calculator

This console drill reads a score from `0` to `100` and displays its letter grade: `A`, `B`, `C`, `D`, or `F`.

- Non-numeric input displays `Invalid score value.`
- Scores below `0` or above `100` display `Score must be between 0 and 100`.

## Drill 03: Login Validator

This console drill checks a username and password, allowing up to three login attempts.

- Correct credentials display `Login successful!`
- Each failed attempt shows the remaining attempts.
- After three failed attempts, the account is locked.

## Drill 04: Even/Odd Analyzer

This console drill reads a positive count of numbers and separates the entered integers into even and odd groups.

- A count of `0`, a negative count, or non-numeric count displays an error message.
- Invalid number entries are rejected and the user is asked to enter that number again.
- The final output shows each group using `string.Join`, along with its count.
