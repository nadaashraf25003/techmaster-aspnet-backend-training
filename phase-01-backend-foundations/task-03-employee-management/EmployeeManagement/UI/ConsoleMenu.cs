using EmployeeManagement.Models;
using EmployeeManagement.Services;

namespace EmployeeManagement.UI;

public class ConsoleMenu
{
    private readonly EmployeeService _employeeService;
    private readonly EmployeeReportService _reportService;

    public ConsoleMenu()
    {
        _employeeService = new EmployeeService();
        _reportService = new EmployeeReportService(_employeeService);
    }

    public void Run()
    {
        while (true)
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // Safe ignore if console is redirected
            }
            ShowMainMenu();
            var choice = Console.ReadLine()?.Trim();

            if (choice == "9")
            {
                Exit();
                break;
            }

            try
            {
                switch (choice)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        UpdateEmployee();
                        break;
                    case "3":
                        DeactivateEmployee();
                        break;
                    case "4":
                        SearchEmployee();
                        break;
                    case "5":
                        FilterByDepartment();
                        break;
                    case "6":
                        SortEmployees();
                        break;
                    case "7":
                        ShowSalaryReports();
                        break;
                    case "8":
                        ViewAllEmployees();
                        break;
                    default:
                        PrintError("Invalid option. Please choose a number from 1 to 9.");
                        break;
                }
            }
            catch (Exception ex)
            {
                PrintError($"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress Enter to return to the menu...");
            Console.ReadLine();
        }
    }

    private void ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====== Employee Management System ======");
        Console.ResetColor();
        Console.WriteLine("1. Add Employee");
        Console.WriteLine("2. Update Employee");
        Console.WriteLine("3. Deactivate Employee");
        Console.WriteLine("4. Search Employee");
        Console.WriteLine("5. Filter by Department");
        Console.WriteLine("6. Sort Employees");
        Console.WriteLine("7. Show Salary Reports");
        Console.WriteLine("8. View All Employees");
        Console.WriteLine("9. Exit");
        Console.Write("Choose an option: ");
    }

    private void AddEmployee()
    {
        Console.WriteLine("\n=== Add Employee ===");

        Console.Write("Enter Employee ID (leave empty to auto-generate): ");
        string? employeeId = Console.ReadLine()?.Trim();
        if (employeeId == "") employeeId = null;

        // Validation for uniqueness if ID entered
        if (employeeId != null && _employeeService.GetEmployee(employeeId) != null)
        {
            PrintError($"Error: Employee ID '{employeeId}' already exists.");
            return;
        }

        string firstName = GetRequiredInput("Enter First Name: ");
        string lastName = GetRequiredInput("Enter Last Name: ");
        string email = GetRequiredInput("Enter Email: ");
        string department = GetRequiredInput("Enter Department: ");
        string position = GetRequiredInput("Enter Position: ");
        decimal salary = GetPositiveDecimalInput("Enter Salary: ");
        DateTime hireDate = GetPastOrPresentDateInput("Enter Hire Date (yyyy-MM-dd): ");

        try
        {
            var emp = _employeeService.AddEmployee(employeeId, firstName, lastName, email, department, position, salary, hireDate);
            PrintSuccess($"Employee '{emp.FullName}' added successfully with ID: {emp.EmployeeId}");
        }
        catch (Exception ex)
        {
            PrintError($"Failed to add employee: {ex.Message}");
        }
    }

    private void UpdateEmployee()
    {
        Console.WriteLine("\n=== Update Employee ===");
        Console.Write("Enter Employee ID to update: ");
        string? employeeId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(employeeId))
        {
            PrintError("Employee ID cannot be empty.");
            return;
        }

        var employee = _employeeService.GetEmployee(employeeId);
        if (employee == null)
        {
            PrintError($"Employee with ID '{employeeId}' not found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Found Employee: {employee.FullName} | Dept: {employee.Department} | Position: {employee.Position} | Salary: {employee.Salary:C}");
        Console.ResetColor();
        Console.WriteLine("Leave field empty to keep current value.");

        string? email = GetOptionalInput($"Enter new Email (current: {employee.Email}): ");
        string? department = GetOptionalInput($"Enter new Department (current: {employee.Department}): ");
        string? position = GetOptionalInput($"Enter new Position (current: {employee.Position}): ");
        
        decimal? salary = null;
        while (true)
        {
            Console.Write($"Enter new Salary (current: {employee.Salary:C}): ");
            string? salaryInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(salaryInput))
            {
                break; // Keep current
            }
            if (decimal.TryParse(salaryInput, out decimal parsedSalary) && parsedSalary > 0)
            {
                salary = parsedSalary;
                break;
            }
            PrintError("Error: Salary must be a positive number.");
        }

        try
        {
            _employeeService.UpdateEmployee(employee.EmployeeId, email, department, position, salary);
            PrintSuccess("Employee updated successfully!");
        }
        catch (Exception ex)
        {
            PrintError($"Failed to update employee: {ex.Message}");
        }
    }

    private void DeactivateEmployee()
    {
        Console.WriteLine("\n=== Deactivate Employee ===");
        Console.Write("Enter Employee ID to deactivate: ");
        string? employeeId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(employeeId))
        {
            PrintError("Employee ID cannot be empty.");
            return;
        }

        var employee = _employeeService.GetEmployee(employeeId);
        if (employee == null)
        {
            PrintError($"Employee with ID '{employeeId}' not found.");
            return;
        }

        if (!employee.IsActive)
        {
            PrintError($"Employee '{employee.FullName}' is already inactive/deactivated.");
            return;
        }

        Console.Write($"Are you sure you want to deactivate {employee.FullName}? (y/n): ");
        string? confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm == "y" || confirm == "yes")
        {
            try
            {
                _employeeService.DeactivateEmployee(employee.EmployeeId);
                PrintSuccess($"Employee '{employee.FullName}' was deactivated successfully.");
            }
            catch (Exception ex)
            {
                PrintError($"Failed to deactivate: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Deactivation cancelled.");
        }
    }

    private void SearchEmployee()
    {
        Console.WriteLine("\n=== Search Employees ===");
        Console.Write("Enter search term (ID or Name): ");
        string query = Console.ReadLine()?.Trim() ?? "";

        var results = _employeeService.SearchEmployees(query);
        PrintEmployeeTable(results, $"Search Results for '{query}'");
    }

    private void FilterByDepartment()
    {
        Console.WriteLine("\n=== Filter by Department ===");
        Console.Write("Enter Department Name: ");
        string? dept = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(dept))
        {
            PrintError("Department name cannot be empty.");
            return;
        }

        Console.Write("Show active employees only? (y/n) [default: y]: ");
        string? activeInput = Console.ReadLine()?.Trim().ToLower();
        bool activeOnly = activeInput != "n" && activeInput != "no";

        var results = _employeeService.FilterByDepartment(dept, activeOnly);
        string mode = activeOnly ? "Active Only" : "All Records";
        PrintEmployeeTable(results, $"Department: {dept} ({mode})");
    }

    private void SortEmployees()
    {
        Console.WriteLine("\n=== Sort Employees ===");
        Console.WriteLine("Choose sort option:");
        Console.WriteLine("1. Salary (Ascending)");
        Console.WriteLine("2. Salary (Descending)");
        Console.WriteLine("3. Hire Date (Ascending)");
        Console.WriteLine("4. Hire Date (Descending)");
        Console.WriteLine("5. Name (Alphabetical)");
        Console.Write("Enter choice (1-5): ");
        string? choice = Console.ReadLine()?.Trim();

        string criteria = choice switch
        {
            "1" => "salary_asc",
            "2" => "salary_desc",
            "3" => "hire_asc",
            "4" => "hire_desc",
            "5" => "name",
            _ => ""
        };

        if (criteria == "")
        {
            PrintError("Invalid sorting choice.");
            return;
        }

        var results = _employeeService.SortEmployees(criteria);
        PrintEmployeeTable(results, "Sorted Employees");
    }

    private void ShowSalaryReports()
    {
        Console.WriteLine("\n=== Salary & HR Metrics Report ===");
        var report = _reportService.GetSalaryReport();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine($"Total Active Employees  : {report.ActiveCount}");
        Console.WriteLine($"Total Inactive Employees: {report.InactiveCount}");
        Console.WriteLine($"Total Payroll (Active)  : {report.TotalPayroll:C}");
        Console.WriteLine($"Average Salary (Active) : {report.AverageSalary:C}");
        
        if (report.HighestSalaryEmployee != null)
        {
            Console.WriteLine($"Highest Paid Employee   : {report.HighestSalaryEmployee.FullName} ({report.HighestSalaryEmployee.Salary:C}) in {report.HighestSalaryEmployee.Department}");
        }
        else
        {
            Console.WriteLine("Highest Paid Employee   : N/A");
        }

        if (report.LowestSalaryEmployee != null)
        {
            Console.WriteLine($"Lowest Paid Employee    : {report.LowestSalaryEmployee.FullName} ({report.LowestSalaryEmployee.Salary:C}) in {report.LowestSalaryEmployee.Department}");
        }
        else
        {
            Console.WriteLine("Lowest Paid Employee    : N/A");
        }

        Console.WriteLine("\nEmployee Count by Department:");
        foreach (var pair in report.CountByDepartment)
        {
            Console.WriteLine($"  - {pair.Key,-20}: {pair.Value} employee(s)");
        }
        Console.WriteLine("---------------------------------------------");
        Console.ResetColor();
    }

    private void ViewAllEmployees()
    {
        var all = _employeeService.GetAllEmployees();
        PrintEmployeeTable(all, "All Employees");
    }

    private void Exit()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nThank you for using the TechMaster Employee Management System. Goodbye!");
        Console.ResetColor();
    }

    // --- Helper Methods ---

    private void PrintEmployeeTable(List<Employee> list, string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n=== {title} ({list.Count} found) ===");
        Console.ResetColor();

        if (list.Count == 0)
        {
            Console.WriteLine("No records to display.");
            return;
        }

        Console.WriteLine(new string('-', 106));
        Console.WriteLine($"| {"ID",-10} | {"Name",-20} | {"Email",-25} | {"Department",-12} | {"Position",-15} | {"Salary",-10} | {"Status",-8} |");
        Console.WriteLine(new string('-', 106));

        foreach (var emp in list)
        {
            string status = emp.IsActive ? "Active" : "Inactive";
            Console.WriteLine($"| {emp.EmployeeId,-10} | {emp.FullName,-20} | {emp.Email,-25} | {emp.Department,-12} | {emp.Position,-15} | {emp.Salary,10:C0} | {status,-8} |");
        }

        Console.WriteLine(new string('-', 106));
    }

    private string GetRequiredInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? val = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(val))
            {
                return val;
            }
            PrintError("Error: This field is required.");
        }
    }

    private string? GetOptionalInput(string prompt)
    {
        Console.Write(prompt);
        string? val = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(val) ? null : val;
    }

    private decimal GetPositiveDecimalInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (decimal.TryParse(input, out decimal val) && val > 0)
            {
                return val;
            }
            PrintError("Error: Must be a positive number.");
        }
    }

    private DateTime GetPastOrPresentDateInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (DateTime.TryParse(input, out DateTime date))
            {
                if (date <= DateTime.Today)
                {
                    return date;
                }
                PrintError("Error: Date cannot be in the future.");
            }
            else
            {
                PrintError("Error: Invalid date format. Use yyyy-MM-dd.");
            }
        }
    }

    private void PrintError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void PrintSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
}
