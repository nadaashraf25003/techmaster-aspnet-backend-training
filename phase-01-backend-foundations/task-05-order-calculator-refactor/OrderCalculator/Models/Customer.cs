namespace OrderCalculatorApp.Models;

public class Customer
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Customer name cannot be empty.", nameof(value));
            }
            _name = value.Trim();
        }
    }

    public CustomerType Type { get; set; }

    public Customer(string name, CustomerType type)
    {
        Name = name;
        Type = type;
    }
}
