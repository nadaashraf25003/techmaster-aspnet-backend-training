public class Drill_09_ShoppingCartTotal
{
    public static void CalculateShoppingCartTotal()
    {
        Console.Write("Enter the number of items in the shopping cart: ");
        string? itemCountInput = Console.ReadLine();
        decimal itemCount = decimal.Parse(itemCountInput);
        if (itemCount <= 0)
        {
            Console.WriteLine("Item count must be a positive number.");
            return;
        }
        
        decimal total = 0;
        for (int i = 1; i <= itemCount; i++)
        {
            Console.Write($"Enter the price of item {i}: ");
            string? itemPriceInput = Console.ReadLine();
            if (!decimal.TryParse(itemPriceInput, out decimal itemPrice) || itemPrice < 0)
            {
                Console.WriteLine("Invalid price. Please enter a non-negative number.");
                i--; // Decrement i to repeat this iteration
                continue;
            }
            else
            {
                itemPrice = decimal.Parse(itemPriceInput);
            }
            Console.WriteLine($"Enter the quantity of item {i}:");
            string? itemQuantityInput = Console.ReadLine();
            decimal itemQuantity = decimal.Parse(itemQuantityInput);
            total += itemPrice * itemQuantity;
        }

        decimal discount = total > 1000 ? total * 0.1m : 0; // Apply 10% discount if total exceeds $1000
        decimal finalTotal = total - discount;

        if (discount > 0)
        {
            Console.WriteLine($"discount {discount}, final {finalTotal}");
        }
        else
        {
            Console.WriteLine($"no discount , total: {finalTotal}");
        }
    
     
    }
}
