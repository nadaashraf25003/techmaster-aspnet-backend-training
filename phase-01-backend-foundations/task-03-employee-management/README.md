# Task 03 - Employee Management System

A console-based Employee Management System built with C# (.NET 10.0) following clean architecture design patterns, encapsulating state, and providing robust input validation.

---

## Features

The system fully supports the following features as required:

1. **Add Employee**: Registers a new active employee. Validates all fields (non-empty fields, positive salary, non-future hire date). Generates a unique Employee ID (e.g. `EMP-1001`) automatically or validates manually-entered IDs to guarantee uniqueness.
2. **Update Employee**: Prompts for Employee ID and allows selective updates of Email, Department, Position, and Salary. Invalid values (such as empty strings or negative salaries) are rejected, while pressing enter retains the current value.
3. **Deactivate Employee**: Marks an employee as inactive (`IsActive = false`) without deleting the record from memory to preserve data integrity.
4. **Search Employees**: Case-insensitive partial name search and exact/partial Employee ID search.
5. **Filter by Department**: Case-insensitive department search with option to filter by active employees only or all records.
6. **Sort Employees**: Sort lists by:
   - Salary (Ascending)
   - Salary (Descending)
   - Hire Date (Ascending)
   - Hire Date (Descending)
   - Full Name (Alphabetical)
7. **Show Salary Reports**: Generates a detailed payroll report containing:
   - Average salary of active employees
   - Employee with highest salary
   - Employee with lowest salary
   - Total payroll of active employees
   - Department employee counts (active and inactive)
   - Active and inactive count metrics
8. **View All Employees**: Displays a beautifully padded table containing all employee records.
9. **Exit**: Exits the console application gracefully.

---

## Project Structure

```
task-03-employee-management/
│
├── Program.cs                        ← Entry point
├── README.md                         ← Features & running instructions
├── task-03-employee-management.csproj ← .NET project configuration
│
└── EmployeeManagement/
    ├── Models/
    │   └── Employee.cs                ← Encapsulated data model with validations
    │
    ├── Services/
    │   ├── EmployeeService.cs         ← Handles memory storage & core business logic
    │   └── EmployeeReportService.cs   ← Computes salary, payroll, and department metrics
    │
    └── UI/
        └── ConsoleMenu.cs             ← UI menu loop, input validations, and presentation
```

---

## Architecture

Following the 3-Layer separation pattern:
```
[ UI Layer (ConsoleMenu.cs) ]
            ↓ calls
[ Service Layer (EmployeeService.cs & EmployeeReportService.cs) ]
            ↓ manipulates
[ Model Layer (Employee.cs) ]
```

- **Model Layer**: The `Employee` class encapsulates state. Setting values requires using validation-backed methods (`UpdateEmail`, `UpdateSalary`, etc.) to prevent corrupt state.
- **Service Layer**: Orchestrates database simulation using lists, validates constraints across multiple employees (e.g., uniqueness of Employee IDs), and calculates analytics.
- **UI Layer**: Manages menu loops, prompts the user field-by-field, performs initial format parsing, catches errors, and prints results in tables.
