# Task 05 - Order Calculator Debug & Refactoring

A comprehensive refactoring of a monolithic, error-prone console order calculator into a clean, modular, and maintainable C# Object-Oriented design.

---

## Technical Highlights & Improvements

Here are the **10 key improvements** implemented during this refactoring:

1. **Cryptic to Descriptive Variable Naming**
   * *Before*: Cryptic names like `c`, `p`, `pr`, `q`, and `t` were used.
   * *After*: Clean, descriptive properties and variables such as `CustomerName`, `ProductName`, `Price`, `Quantity`, and `CustomerType` were introduced.

2. **Separation of Concerns (Layered Design)**
   * *Before*: All logic, input reading, processing, math, and output printing were crammed inside a single `Program.Main` method.
   * *After*: Extracted concerns into separate logical layers: Models (`Customer`, `Order`), Services (`OrderCalculatorService`), and UI (`ConsoleMenu`, `ReceiptPrinter`, `ValidationHelper`).

3. **Domain Encapsulation**
   * *Before*: Free-floating variables in `Main` with no boundary logic or models.
   * *After*: Built custom `Customer` and `Order` classes. State can only be updated through controlled property setters or constructor parameters.

4. **Strongly-Typed Enum for Loyalty Levels**
   * *Before*: Unsafe, case-sensitive string matching for `"Regular"`, `"Silver"`, `"Gold"`, and `"VIP"`. Any typo led to silently bypassing discounts.
   * *After*: Introduced a strongly-typed `CustomerType` enum. The application parses and validates the enum using case-insensitive checks.

5. **Financial Arithmetic Precision**
   * *Before*: Used `double` for price and currency math. Double-precision numbers are prone to binary floating-point representation rounding errors, which is unsafe for financial applications.
   * *After*: Upgraded all price and money-related calculations to use `decimal`, guaranteeing exact base-10 precision.

6. **Input Validation & Crash Prevention**
   * *Before*: Direct `double.Parse(...)` and `int.Parse(...)` on raw console lines. Typing a letter or a blank string instantly crashed the program.
   * *After*: Implemented robust parsing loops using `decimal.TryParse` and `int.TryParse` within `ValidationHelper` to catch errors gracefully and prompt again.

7. **Magic Numbers Replaced with Constants**
   * *Before*: Magic inline constants like `0.14`, `50`, `1000`, `0.05`, etc.
   * *After*: Declared descriptive, self-documenting constants inside `OrderCalculatorService` (e.g. `TaxRate = 0.14m`, `StandardShippingCost = 50m`, `FreeShippingThreshold = 1000m`).

8. **Don't Repeat Yourself (DRY) Principle**
   * *Before*: Manual, repetitive prompts and validation logic inline.
   * *After*: Consolidated inputs and error responses into a reusable static `ValidationHelper` class.

9. **Premium Styled Receipt Printing**
   * *Before*: Raw `Console.WriteLine` listings without alignment, casing consistency, or currency formatting.
   * *After*: Developed a custom `ReceiptPrinter` that formats money fields as currency (`:C`), prints standard date timestamps, highlights discounts/free shipping in green, and prints totals in bold yellow.

10. **Application Lifecycle Loop**
    * *Before*: Run once and exit.
    * *After*: Implemented a clean, user-friendly loop in `ConsoleMenu` asking if the user wants to run another calculation or exit.

---

## Architectural Layout

*   **Models/**
    *   [CustomerType.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/Models/CustomerType.cs): An enum detailing regular/loyalty categories.
    *   [Customer.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/Models/Customer.cs): Domain entity representing customer name and type, validating name presence.
    *   [Order.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/Models/Order.cs): Domain entity encapsulating order lines, validating quantity and price are positive.
*   **Services/**
    *   [OrderCalculatorService.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/Services/OrderCalculatorService.cs): Holds business rules and calculates tax, discounts, subtotal, shipping, and totals.
*   **UI/**
    *   [ConsoleMenu.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/UI/ConsoleMenu.cs): Runs the continuous application lifecycle.
    *   [ReceiptPrinter.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/UI/ReceiptPrinter.cs): Renders clean, colorized output.
    *   [ValidationHelper.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/OrderCalculator/UI/ValidationHelper.cs): Consolidates console prompting, parsing, and constraints.
*   [Program.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/Program.cs): Executable entry point.
*   [original-bad-code/Program.cs](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/original-bad-code/Program.cs): Original monolithic code retained for reference.
