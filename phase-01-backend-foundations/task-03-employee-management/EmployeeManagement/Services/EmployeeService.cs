using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class EmployeeService
{
    private readonly List<Employee> _employees;
    private int _employeeIdCounter;

    public EmployeeService()
    {
        _employees = new List<Employee>();
        _employeeIdCounter = 12;
        SeedDefaultEmployees();
    }

    private void SeedDefaultEmployees()
    {
        // Seed initial employees from the custom data table
        AddEmployee("EMP-001", "Mohamed", "Ayman", "mohamed@test.com", "IT", "Backend Developer", 20000, new DateTime(2025, 1, 10));
        AddEmployee("EMP-002", "Sara", "Adel", "sara@test.com", "HR", "HR Specialist", 12000, new DateTime(2024, 5, 15));
        AddEmployee("EMP-003", "Ahmed", "Tarek", "ahmed@test.com", "IT", "Junior Developer", 9000, new DateTime(2026, 1, 1));
        AddEmployee("EMP-004", "Omar", "Samir", "omar@test.com", "Sales", "Sales Executive", 11000, new DateTime(2023, 11, 20));
        AddEmployee("EMP-005", "Mariam", "Hassan", "mariam@test.com", "Finance", "Accountant", 14000, new DateTime(2022, 9, 11));
        AddEmployee("EMP-006", "Khaled", "Ali", "khaled@test.com", "IT", "DevOps Trainee", 10000, new DateTime(2026, 2, 1));
        AddEmployee("EMP-007", "Nour", "Emad", "nour@test.com", "Marketing", "Content Specialist", 9500, new DateTime(2025, 7, 8));
        
        var youssef = AddEmployee("EMP-008", "Youssef", "Nabil", "youssef@test.com", "Sales", "Sales Manager", 18000, new DateTime(2021, 3, 17));
        youssef.Deactivate();

        AddEmployee("EMP-009", "Dina", "Farouk", "dina@test.com", "HR", "Recruiter", 10500, new DateTime(2024, 2, 13));
        AddEmployee("EMP-010", "Hady", "Mahmoud", "hady@test.com", "IT", "QA Engineer", 13000, new DateTime(2025, 10, 1));
        AddEmployee("EMP-011", "Salma", "Taha", "salma@test.com", "Finance", "Finance Manager", 26000, new DateTime(2020, 12, 12));
        AddEmployee("EMP-012", "Ali", "Mostafa", "ali@test.com", "Support", "Support Agent", 8000, new DateTime(2026, 3, 5));
        AddEmployee("EMP-013", "Ali", "Hasan", "alihasan@test.com", "IT", "Backend Developer", 8000, new DateTime(2026, 3, 5));
    }

    public Employee AddEmployee(
        string? employeeId, 
        string firstName, 
        string lastName, 
        string email, 
        string department, 
        string position, 
        decimal salary, 
        DateTime hireDate)
    {
        string finalEmployeeId;
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            finalEmployeeId = GenerateUniqueEmployeeId();
        }
        else
        {
            var trimmedId = employeeId.Trim().ToUpper();
            if (_employees.Any(e => e.EmployeeId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Employee ID '{trimmedId}' already exists.");
            }
            finalEmployeeId = trimmedId;
        }

        var employee = new Employee(
            finalEmployeeId, 
            firstName, 
            lastName, 
            email, 
            department, 
            position, 
            salary, 
            hireDate);
        
        _employees.Add(employee);
        return employee;
    }

    private string GenerateUniqueEmployeeId()
    {
        string id;
        do
        {
            _employeeIdCounter++;
            id = $"EMP-{_employeeIdCounter:D3}";
        } 
        while (_employees.Any(e => e.EmployeeId.Equals(id, StringComparison.OrdinalIgnoreCase)));
        return id;
    }

    public Employee? GetEmployee(string employeeId)
    {
        return _employees.FirstOrDefault(e => e.EmployeeId.Equals(employeeId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public List<Employee> GetAllEmployees()
    {
        return _employees.ToList();
    }

    public void UpdateEmployee(
        string employeeId, 
        string? email, 
        string? department, 
        string? position, 
        decimal? salary)
    {
        var employee = GetEmployee(employeeId) 
            ?? throw new KeyNotFoundException($"Employee with ID '{employeeId}' not found.");

        if (!string.IsNullOrWhiteSpace(email))
        {
            employee.UpdateEmail(email);
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            employee.UpdateDepartment(department);
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            employee.UpdatePosition(position);
        }

        if (salary.HasValue)
        {
            employee.UpdateSalary(salary.Value);
        }
    }

    public void DeactivateEmployee(string employeeId)
    {
        var employee = GetEmployee(employeeId) 
            ?? throw new KeyNotFoundException($"Employee with ID '{employeeId}' not found.");
        
        employee.Deactivate();
    }

    public List<Employee> SearchEmployees(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return _employees.ToList();

        var lowerQuery = query.Trim().ToLower();
        
        return _employees.Where(e => 
            e.EmployeeId.ToLower().Contains(lowerQuery) || 
            e.FullName.ToLower().Contains(lowerQuery)
        ).ToList();
    }

    public List<Employee> FilterByDepartment(string department, bool activeOnly = true)
    {
        var lowerDept = department.Trim().ToLower();
        
        return _employees.Where(e => 
            e.Department.ToLower() == lowerDept && 
            (!activeOnly || e.IsActive)
        ).ToList();
    }

    public List<Employee> SortEmployees(string criteria)
    {
        return criteria.ToLower() switch
        {
            "salary_asc" => _employees.OrderBy(e => e.Salary).ToList(),
            "salary_desc" => _employees.OrderByDescending(e => e.Salary).ToList(),
            "hire_asc" => _employees.OrderBy(e => e.HireDate).ToList(),
            "hire_desc" => _employees.OrderByDescending(e => e.HireDate).ToList(),
            "name" => _employees.OrderBy(e => e.FullName, StringComparer.OrdinalIgnoreCase).ToList(),
            _ => _employees.ToList()
        };
    }
}
