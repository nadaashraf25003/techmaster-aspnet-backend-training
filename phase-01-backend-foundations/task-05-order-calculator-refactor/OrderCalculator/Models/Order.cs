namespace OrderCalculatorApp.Models;

public class Order
{
    private string _productName = string.Empty;
    private decimal _price;
    private int _quantity;

    public Customer Customer { get; set; }

    public string ProductName
    {
        get => _productName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Product name cannot be empty.", nameof(value));
            }
            _productName = value.Trim();
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Price must be positive.");
            }
            _price = value;
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be positive.");
            }
            _quantity = value;
        }
    }

    public Order(Customer customer, string productName, decimal price, int quantity)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }
}
