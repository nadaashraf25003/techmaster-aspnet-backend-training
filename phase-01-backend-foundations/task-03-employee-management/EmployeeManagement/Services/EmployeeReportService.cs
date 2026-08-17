using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class EmployeeReportService
{
    private readonly EmployeeService _employeeService;

    public EmployeeReportService(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public SalaryReport GetSalaryReport()
    {
        var allEmployees = _employeeService.GetAllEmployees();
        var activeEmployees = allEmployees.Where(e => e.IsActive).ToList();

        decimal totalPayroll = activeEmployees.Sum(e => e.Salary);
        decimal averageSalary = activeEmployees.Any() ? activeEmployees.Average(e => e.Salary) : 0;
        
        Employee? highestSalaryEmployee = activeEmployees.Any() 
            ? activeEmployees.OrderByDescending(e => e.Salary).First() 
            : null;
            
        Employee? lowestSalaryEmployee = activeEmployees.Any() 
            ? activeEmployees.OrderBy(e => e.Salary).First() 
            : null;

        var countByDepartment = allEmployees
            .GroupBy(e => e.Department, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count());

        int activeCount = allEmployees.Count(e => e.IsActive);
        int inactiveCount = allEmployees.Count(e => !e.IsActive);

        return new SalaryReport
        {
            AverageSalary = averageSalary,
            HighestSalaryEmployee = highestSalaryEmployee,
            LowestSalaryEmployee = lowestSalaryEmployee,
            TotalPayroll = totalPayroll,
            CountByDepartment = countByDepartment,
            ActiveCount = activeCount,
            InactiveCount = inactiveCount
        };
    }
}

public class SalaryReport
{
    public decimal AverageSalary { get; set; }
    public Employee? HighestSalaryEmployee { get; set; }
    public Employee? LowestSalaryEmployee { get; set; }
    public decimal TotalPayroll { get; set; }
    public Dictionary<string, int> CountByDepartment { get; set; } = new();
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
}
