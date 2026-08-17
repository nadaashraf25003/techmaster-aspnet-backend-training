namespace EmployeeManagement.Models;

public class Employee
{
    public string EmployeeId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; private set; }
    public string Department { get; private set; }
    public string Position { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; private set; }

    public Employee(
        string employeeId, 
        string firstName, 
        string lastName, 
        string email, 
        string department, 
        string position, 
        decimal salary, 
        DateTime hireDate)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            throw new ArgumentException("Employee ID is required.", nameof(employeeId));
        
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        
        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("Department is required.", nameof(department));
        
        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Position is required.", nameof(position));
        
        if (salary <= 0)
            throw new ArgumentException("Salary must be positive.", nameof(salary));
        
        if (hireDate > DateTime.Today)
            throw new ArgumentException("Hire date cannot be in the future.", nameof(hireDate));

        EmployeeId = employeeId.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        Department = department.Trim();
        Position = position.Trim();
        Salary = salary;
        HireDate = hireDate;
        IsActive = true;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        
        Email = email.Trim();
    }

    public void UpdateDepartment(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("Department cannot be empty.", nameof(department));
        
        Department = department.Trim();
    }

    public void UpdatePosition(string position)
    {
        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Position cannot be empty.", nameof(position));
        
        Position = position.Trim();
    }

    public void UpdateSalary(decimal salary)
    {
        if (salary <= 0)
            throw new ArgumentException("Salary must be positive.", nameof(salary));
        
        Salary = salary;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
