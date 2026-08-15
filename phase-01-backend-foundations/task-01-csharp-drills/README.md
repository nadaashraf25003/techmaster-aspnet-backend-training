# Task 01 - C# Logic Drill Pack

## Drill Overview

| Drill No. | Drill Name | Topic | Status | Notes | Expected Skill Evidence |
|---:|---|---|---|---|---|
| 01 | Temperature Converter | Parsing / Calculation | Done | Handles invalid input | Parsing / Calculation / Formatting |
| 02 | Grade Calculator | Conditions | Done | Validates scores from 0 to 100 | Conditions / Boundaries |
| 03 | Login Validator | Loops / Strings | Done | Max 3 attempts | Loops / String Comparison |
| 04 | Even/Odd Analyzer | Conditions / Modulo | Done | Handles invalid input | Loops / Lists / Modulo |
| 05 | Maximum and Minimum Finder | Manual Looping / Comparison | Done | Manual max/min then LINQ bonus | Manual Looping / Comparison |
| 06 | Word Counter | String Manipulation | Done | Ignores extra spaces | String Manipulation |
| 07 | Name Formatter | Strings / Loops / Formatting | Done | Title case normalization | Strings / Loops / Formatting |
| 08 | Password Strength Checker | Validation / Boolean Flags | Done | Reports missing rules | Validation / Boolean Flags |
| 09 | Shopping Cart Total | Loops / Decimal Business Rules | Done | 10% discount over 1000 | Loops / Decimal Business Rules |
| 10 | Simple ATM Menu | Menu Loop / Switch / Validation | Done | Menu stays open until exit | Menu Loop / Switch / Validation |
| 11 | Duplicate Number Detector | HashSet / Duplicates | Done | Detects and prints duplicates once | HashSet / Duplicates |
| 12 | Email Validator | String Validation | Done | Validates email format | String Validation |
| 13 | Palindrome Checker | String Reverse / Comparison | Done | Checks palindrome with/without spaces | String Reverse / Comparison |
| 14 | Simple Expense Tracker | Lists / Objects / Calculations | Done | Tracks expenses with statistics | Lists / Objects / Calculations |
| 15 | Array Rotation | Arrays / Indexing | Done | Rotates array right by one position | Arrays / Indexing |
| 16 | Frequency Counter | Dictionary / Counting | Done | Counts element frequencies | Dictionary / Counting |
| 17 | Simple Search Engine | String Search / Lists | Done | Searches words in string list | String Search / Lists |
| 18 | Number Statistics | Statistics / Calculations | Done | Calculates min/max/sum/average | Statistics / Calculations |
| 19 | Simple Ticket Price Calculator | Conditions / Calculations | Done | Calculates price by age category | Conditions / Calculations |
| 20 | Method Refactoring Challenge | Refactoring / Design | Done | Splits complex method into smaller ones | Refactoring / Design |


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

## Drill 05: Maximum and Minimum Finder

Find maximum and minimum values from a list manually before using LINQ.

- Read a list of numbers.
- Find max manually without `Max()`.
- Find min manually without `Min()`.
- Print both values.
- Bonus: solve again with LINQ and compare results.

Edge cases covered:
- Single value
- Negative values
- Duplicate values
- Empty list is rejected

Examples:
- Input: `5,1,9,-2` → Output: `Max: 9 | Min: -2`
- Input: `7` → Output: `Max: 7 | Min: 7`
- Input: `-5,-2,-10` → Output: `Max: -2 | Min: -10`

## Drill 06: Word Counter

Count the real number of words in a sentence while ignoring extra spaces.

- Ask for a sentence.
- Reject empty input.
- Trim leading/trailing spaces.
- Split by spaces while removing empty entries.
- Print word count.

Examples:
- Input: `I am learning backend development` → Output: `Word count: 5`
- Input: `Hello world` → Output: `Word count: 2`
- Input: `   ` → Output: `Sentence cannot be empty.`

## Drill 07: Name Formatter

Normalize a messy full name into professional title case.

- Ask for full name.
- Remove extra spaces.
- Convert each name part to first-letter uppercase and remaining lowercase.
- Print formatted name.

Examples:
- Input: `mOhAmEd aYmAn aDeL` → Output: `Mohamed Ayman Adel`
- Input: `sara` → Output: `Sara`
- Input: `  ahmed   mohamed  ` → Output: `Ahmed Mohamed`

## Drill 08: Password Strength Checker

Validate password strength using common rules and report missing requirements.

- Check length >= 8.
- Check uppercase letter.
- Check lowercase letter.
- Check digit.
- Check special character.
- Print Strong or Weak with missing rules.

Examples:
- Input: `P@ssword123` → Output: `Strong`
- Input: `password` → Output: `Weak - missing uppercase, digit, special character`
- Input: `Pass1234` → Output: `Weak - missing special character`

## Drill 09: Shopping Cart Total

Calculate a simple shopping cart total with discount rules.

- Ask how many items.
- For each item read price and quantity.
- Reject negative or zero price/quantity.
- Calculate subtotal per item.
- Calculate grand total.
- Apply 10% discount if total exceeds 1000.

Examples:
- Total: `1200` → Output: `Discount: 120, Final: 1080`
- Total: `900` → Output: `No discount`
- Input: `-5` → Output: `Invalid price`

## Drill 10: Simple ATM Menu

Create an ATM simulation with a menu that stays open until exit.

- Initial balance = 1000.
- Menu: Check Balance, Deposit, Withdraw, Exit.
- Deposit must be positive.
- Withdraw cannot exceed balance.
- Invalid option prints clear message.

Examples:
- Deposit `500` → Balance: `1500`
- Withdraw `2000` → Output: `Insufficient balance`
- Option `9` → Output: `Invalid option`

## Drill 11: Duplicate Number Detector

Detect duplicate numbers in a list and print each duplicate once.

- Accept a list of integers.
- Detect duplicated values.
- Print duplicate values once.
- Print clear message if no duplicates.

Examples:
- Input: `1,2,3,2,4,1` → Output: `Duplicates: 2, 1`
- Input: `1,2,3` → Output: `No duplicates found`

## Drill 12: Email Validator

Create a simple email validator for training purposes.

- Email cannot be empty.
- Must contain @.
- Must contain dot.
- Cannot start or end with @.
- Cannot contain spaces.

Examples:
- Input: `test@mail.com` → Output: `Valid email`
- Input: `@mail.com` → Output: `Invalid`
- Input: `test mail.com` → Output: `Invalid`
- Input: `test@mail` → Output: `Invalid`

## Drill 13: Palindrome Checker

Check whether a word or sentence reads the same forward and backward.

- Ask for text.
- Ignore case.
- Reverse the cleaned text.
- Compare original cleaned text with reversed text.
- Bonus: ignore spaces.

Examples:
- Input: `madam` → Output: `Palindrome`
- Input: `hello` → Output: `Not Palindrome`
- Input: `nurses run` → Output: `Palindrome`

## Drill 14: Simple Expense Tracker

Track named expenses and calculate summary statistics.

- Allow user to enter multiple expenses.
- Each expense has name and amount.
- Amount must be positive.
- Print total, average and highest expense.

Examples:
- Input: `Food 100, Transport 50` → Output: `Total: 150, Average: 75, Highest: Food`
- Input: `-50` → Output: `Invalid amount`

## Drill 15: Array Rotation

Rotate array elements one step to the right.

- Use an array.
- Move last element to first position.
- Shift all other elements right.
- Do not use built-in tricks only.

Examples:
- Input: `[1,2,3,4,5]` → Output: `[5,1,2,3,4]`
- Input: `[7]` → Output: `[7]`
- Input: `[1,1,2]` → Output: `[2,1,1]`
