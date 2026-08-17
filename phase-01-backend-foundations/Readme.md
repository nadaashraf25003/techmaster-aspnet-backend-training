# TechMaster 
## phase-01-backend-foundations

### Task 01: C# Drills

#### Drill 01: Temperature Converter

Converts a validated Celsius value to Fahrenheit and reports invalid input safely.

#### Drill 02: Grade Calculator

Converts a validated score from `0` to `100` into a letter grade.

#### Drill 03: Login Validator

Validates login credentials with a maximum of three attempts before locking the account.

#### Drill 04: Even/Odd Analyzer

Reads multiple validated integers and separates them into even and odd lists with their counts.

#### Drill 05: Maximum and Minimum Finder

Find maximum and minimum values from a list manually before using LINQ, with bonus LINQ comparison.

#### Drill 06: Word Counter

Count words in a sentence while ignoring extra spaces and empty input.

#### Drill 07: Name Formatter

Normalize a messy full name into professional title case.

#### Drill 08: Password Strength Checker

Validate password strength using common rules and report missing requirements.

#### Drill 09: Shopping Cart Total

Calculate a shopping cart total with discount rules (10% off if total exceeds 1000).

#### Drill 10: Simple ATM Menu

Create an ATM simulation with a menu that stays open until exit.

#### Drill 11: Duplicate Number Detector

Detect duplicate numbers in a list and print each duplicate once.

#### Drill 12: Email Validator

Create a simple email validator with basic format checks.

#### Drill 13: Palindrome Checker

Check whether a word or sentence reads the same forward and backward.

#### Drill 14: Simple Expense Tracker

Track named expenses and calculate summary statistics (total, average, highest).

#### Drill 15: Array Rotation

Rotate array elements one step to the right using manual indexing.

#### Drill 16: Frequency Counter

Count element frequencies using Dictionary<int, int>.

#### Drill 17: Simple Search Engine

Search names by partial keyword with case-insensitive matching.

#### Drill 18: Number Statistics

Print count, sum, average, max, min, positives, and negatives.

#### Drill 19: Simple Ticket Price Calculator

Calculate ticket price with the best eligible age/student discount.

#### Drill 20: Method Refactoring Challenge

Refactor 3 drills into small single-responsibility methods.

---

### Task 02: Bank Account System

A console-based bank account management system built with C# (.NET), demonstrating core OOP principles, layered architecture, and business logic encapsulation.

#### Key Features:
- **Create Customer Account**: Register a new customer and open a bank account with an initial balance.
- **Deposit Money**: Deposit a positive amount into an existing account with automated transaction logging.
- **Withdraw Money**: Withdraw funds with overdraft protection and balance validation.
- **Transfer Money**: Securely transfer funds between two accounts with dual-transaction auditing.
- **View Account Details**: Display full account and customer information using DTO patterns.
- **View Transaction History**: View complete transaction audit trail sorted by date (newest first).
- **View All Accounts**: Display a summary view of all registered accounts.

#### Core Concepts:
- **3-Layer Architecture**: Separation of concerns across `UI` (`ConsoleMenu.cs`), `Services` (`BankService.cs`), and `Models` (`BankAccount.cs`, `Customer.cs`, `Transaction.cs`).
- **Encapsulation & Immutability**: Protected balances mutated only through domain methods, with immutable transaction audit logs.
- **Validation & Exception Handling**: Defensive checks across all banking operations with clean UI-level error handling.

---

### Task 03: Employee Management System

A console-based employee management and analytics system built with C# (.NET) adhering to 3-layer architecture, data encapsulation, and comprehensive LINQ querying.

#### Key Features:
- **Add Employee**: Register new employees with unique IDs (e.g., `EMP-1001`), salary validation, and non-future hire date checks.
- **Update Employee**: Selectively update email, department, position, and salary with validation while retaining existing values on empty input.
- **Deactivate Employee**: Soft-delete records (`IsActive = false`) to preserve historical integrity.
- **Search Employees**: Case-insensitive partial name search and exact/partial Employee ID search.
- **Filter by Department**: Filter records by department and active status.
- **Sort Employees**: Sort employee lists by salary (asc/desc), hire date (asc/desc), or full name (alphabetical).
- **Salary & Payroll Reports**: Detailed payroll analytics including average salary, highest/lowest earners, total payroll, and department headcount metrics using LINQ.
- **View All Employees**: Formatted tabular view displaying all registered employees.

#### Core Concepts:
- **Layered Design**: Modular design across `UI` (`ConsoleMenu.cs`), `Services` (`EmployeeService.cs`, `EmployeeReportService.cs`), and `Models` (`Employee.cs`).
- **Domain Invariant Validation**: The `Employee` class encapsulates state changes with internal validation methods.
- **LINQ Operations**: Extensive use of LINQ for sorting, filtering, and statistical aggregations.

---

### Task 04: Product Catalog LINQ System

A high-performance console-based Product Catalog Query and Reporting System built with C# (.NET 10.0) demonstrating 20 comprehensive LINQ query operations, clean layered architecture, and reporting projections.

#### Key Features:
- **Available Products Filtering**: Query active in-stock inventory (`Where(p => p.IsInStock)`).
- **Category & Price Range Filtering**: Case-insensitive category and numeric boundary filtering.
- **Partial Keyword Search**: Case-insensitive substring matching on product titles.
- **Ascending / Descending Sorting**: Dynamic price sorting (`OrderBy` / `OrderByDescending`).
- **Category Grouping & Aggregations**: Group by category and compute counts, min, max, average prices, and stock values (`GroupBy` + `Select`).
- **Low Stock Alerts**: Configurable stock threshold alerts (e.g., items with $\le 5$ units).
- **Supplier Analytics**: Group products by supplier and project `SupplierReport` DTO with total stock valuation.
- **Pagination Engine**: Fast, bounded page navigation (`Skip` + `Take`) with previous/next page indicators.
- **Recent Product Filtering**: Time-window filtering for products created in the last 60 days.
- **Above-Average Pricing Analysis**: Multi-step query finding products priced above catalog average.

#### Core Concepts:
- **Clean 3-Layer Architecture**: Separation of concerns across `UI` (`ConsoleMenu.cs`, `ConsoleFormatter.cs`), `Services` (`ProductQueryService.cs`), and `Models` (`Product.cs`, `SupplierReport.cs`, `CategoryStats.cs`, `PagedResult.cs`).
- **Strongly Typed DTO Projections**: Transforming raw entity groupings into dedicated reporting models.
- **Comprehensive LINQ Showcase**: `Where`, `Select`, `OrderBy`, `OrderByDescending`, `GroupBy`, `Average`, `Sum`, `Min`, `Max`, `Skip`, `Take`, `Any`, and `Distinct`.

