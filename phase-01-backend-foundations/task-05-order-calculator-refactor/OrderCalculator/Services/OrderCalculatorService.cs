using OrderCalculatorApp.Models;

namespace OrderCalculatorApp.Services;

public class OrderCalculationResult
{
    public decimal Subtotal { get; }
    public decimal Discount { get; }
    public decimal AfterDiscount { get; }
    public decimal Tax { get; }
    public decimal Shipping { get; }
    public decimal FinalTotal { get; }

    public OrderCalculationResult(decimal subtotal, decimal discount, decimal afterDiscount, decimal tax, decimal shipping, decimal finalTotal)
    {
        Subtotal = subtotal;
        Discount = discount;
        AfterDiscount = afterDiscount;
        Tax = tax;
        Shipping = shipping;
        FinalTotal = finalTotal;
    }
}

public class OrderCalculatorService
{
    public const decimal TaxRate = 0.14m;
    public const decimal StandardShippingCost = 50m;
    public const decimal FreeShippingThreshold = 1000m;

    public const decimal RegularDiscountRate = 0.00m;
    public const decimal SilverDiscountRate = 0.05m;
    public const decimal GoldDiscountRate = 0.10m;
    public const decimal VIPDiscountRate = 0.15m;

    public OrderCalculationResult Calculate(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        decimal subtotal = order.Price * order.Quantity;
        decimal discountRate = GetDiscountRate(order.Customer.Type);
        decimal discount = subtotal * discountRate;
        decimal afterDiscount = subtotal - discount;
        decimal tax = afterDiscount * TaxRate;
        decimal shipping = afterDiscount >= FreeShippingThreshold ? 0m : StandardShippingCost;
        decimal finalTotal = afterDiscount + tax + shipping;

        return new OrderCalculationResult(subtotal, discount, afterDiscount, tax, shipping, finalTotal);
    }

    public decimal GetDiscountRate(CustomerType customerType)
    {
        return customerType switch
        {
            CustomerType.Regular => RegularDiscountRate,
            CustomerType.Silver => SilverDiscountRate,
            CustomerType.Gold => GoldDiscountRate,
            CustomerType.VIP => VIPDiscountRate,
            _ => throw new ArgumentOutOfRangeException(nameof(customerType), $"Unsupported customer type: {customerType}")
        };
    }
}
