public class Drill_10_SimpleAtmMenu
{
    public static void ShowAtmMenu()
    {
        int balance = 1000; // Initial balance
        string[]? menuChoice = ["1. Check Balance", "2. Deposit", "3. Withdraw", "4. Exit"];
        while (true)
        {
            Console.WriteLine("Welcome to the Simple ATM");
            Console.WriteLine("Please select an option:");
            foreach (string choice in menuChoice)
            {
                Console.WriteLine(choice);
            }
            string? userChoice = Console.ReadLine();
            if (userChoice == "1")
            {
                Console.WriteLine("Your balance is: " + balance);
            }
            else if (userChoice == "2")
            {
                Console.Write("Enter amount to deposit: ");
                string? depositInput = Console.ReadLine();
                if (int.TryParse(depositInput, out int depositAmount) && depositAmount > 0)
                {
                    balance += depositAmount;
                    Console.WriteLine("balance " + balance);
                }
                else
                {
                    Console.WriteLine("Invalid deposit amount.");
                }
            }
            else if (userChoice == "3")
            {
                Console.Write("Enter amount to withdraw: ");
                string? withdrawInput = Console.ReadLine();
                if (int.TryParse(withdrawInput, out int withdrawAmount) && withdrawAmount > 0)
                {
                    if (withdrawAmount <= balance)
                    {
                        balance -= withdrawAmount;
                        Console.WriteLine("Withdrawal successful. New balance: " + balance);
                    }
                    else
                    {
                        Console.WriteLine("Insufficient balance.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid withdrawal amount.");
                }
            }
            else if (userChoice == "4")
            {
                Console.WriteLine("Thank you for using the Simple ATM. Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. ");
            }
        }
    }
}
