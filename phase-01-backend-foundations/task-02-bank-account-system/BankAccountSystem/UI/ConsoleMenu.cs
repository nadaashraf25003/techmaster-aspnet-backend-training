using BankAccountSystem.Models;
using BankAccountSystem.Services;

namespace BankAccountSystem.UI;

public class ConsoleMenu
{
    private readonly BankService _bankService;

    public ConsoleMenu()
    {
        _bankService = new BankService();
    }

    public void Run()
    {
        
           
           
  
            ShowMainMenu();
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        CreateCustomerAccount();
                        break;
                    case "2":
                        DepositMoney();
                        break;
                    case "3":
                        WithdrawMoney();
                        break;
                    case "4":
                        TransferMoney();
                        break;
                    case "5":
                        ViewAccountDetails();
                        break;
                    case "6":
                        ViewTransactionHistory();
                        break;
                    case "7":
                        ViewAllAccounts();
                        break;
                    case "8":
                        Exit();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        
  
         Console.WriteLine("Did you want to continue with Bank Account System? (yes/no)");
                   string?  continueChoice = Console.ReadLine();
                while (continueChoice != null && continueChoice.Trim().ToLower() == "yes")
            {
                 ShowMainMenu();
           choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        CreateCustomerAccount();
                        break;
                    case "2":
                        DepositMoney();
                        break;
                    case "3":
                        WithdrawMoney();
                        break;
                    case "4":
                        TransferMoney();
                        break;
                    case "5":
                        ViewAccountDetails();
                        break;
                    case "6":
                        ViewTransactionHistory();
                        break;
                    case "7":
                        ViewAllAccounts();
                        break;
                    case "8":
                        Exit();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
                Console.WriteLine("Did you want to continue with Bank Account System? (yes/no)");
                continueChoice = Console.ReadLine();
            }
    
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("====== TechMaster Bank System ======");
        Console.WriteLine("1. Create Customer Account");
        Console.WriteLine("2. Deposit Money");
        Console.WriteLine("3. Withdraw Money");
        Console.WriteLine("4. Transfer Money");
        Console.WriteLine("5. View Account Details");
        Console.WriteLine("6. View Transaction History");
        Console.WriteLine("7. View All Accounts");
        Console.WriteLine("8. Exit");
        Console.Write("Choose an option: ");
    }

    private void CreateCustomerAccount()
    {
        Console.WriteLine("\n=== Create Customer Account ===");
        
        Console.Write("Enter full name: ");
        string? fullName = Console.ReadLine();

        Console.Write("Enter email: ");
        string? email = Console.ReadLine();

        Console.Write("Enter phone number: ");
        string? phoneNumber = Console.ReadLine();

        Console.WriteLine("Select account type:");
        Console.WriteLine("1. Checking");
        Console.WriteLine("2. Savings");
        Console.WriteLine("3. Business");
        Console.Write("Choose account type: ");
        string? accountTypeChoice = Console.ReadLine();

        AccountType accountType = accountTypeChoice switch
        {
            "1" => AccountType.Checking,
            "2" => AccountType.Savings,
            "3" => AccountType.Business,
            _ => throw new InvalidOperationException("Invalid account type.")
        };

        Console.Write("Enter initial balance (or 0): ");
        string? balanceInput = Console.ReadLine();
        decimal initialBalance = decimal.Parse(balanceInput ?? "0");

        var customer = _bankService.CreateCustomer(fullName!, email!, phoneNumber!);
        var account = _bankService.CreateAccount(customer, accountType, initialBalance);

        Console.WriteLine($"\nAccount created successfully!");
        Console.WriteLine($"Account Number: {account.AccountNumber}");
        Console.WriteLine($"Customer ID: {customer.CustomerId}");
        Console.WriteLine($"Account Type: {account.AccountType}");
        Console.WriteLine($"Initial Balance: {account.Balance:C}");
    }

    private void DepositMoney()
    {
        Console.WriteLine("\n=== Deposit Money ===");
        
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();

        Console.Write("Enter deposit amount: ");
        string? amountInput = Console.ReadLine();
        decimal amount = decimal.Parse(amountInput ?? "0");

        _bankService.Deposit(accountNumber!, amount);

        var account = _bankService.GetAccount(accountNumber!);
        Console.WriteLine($"Deposit successful! New balance: {account!.Balance:C}");
    }

    private void WithdrawMoney()
    {
        Console.WriteLine("\n=== Withdraw Money ===");
        
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();

        Console.Write("Enter withdrawal amount: ");
        string? amountInput = Console.ReadLine();
        decimal amount = decimal.Parse(amountInput ?? "0");

        _bankService.Withdraw(accountNumber!, amount);

        var account = _bankService.GetAccount(accountNumber!);
        Console.WriteLine($"Withdrawal successful! New balance: {account!.Balance:C}");
    }

    private void TransferMoney()
    {
        Console.WriteLine("\n=== Transfer Money ===");
        
        Console.Write("Enter source account number: ");
        string? fromAccountNumber = Console.ReadLine();

        Console.Write("Enter destination account number: ");
        string? toAccountNumber = Console.ReadLine();

        Console.Write("Enter transfer amount: ");
        string? amountInput = Console.ReadLine();
        decimal amount = decimal.Parse(amountInput ?? "0");

        _bankService.Transfer(fromAccountNumber!, toAccountNumber!, amount);

        var fromAccount = _bankService.GetAccount(fromAccountNumber!);
        Console.WriteLine($"Transfer successful! New balance: {fromAccount!.Balance:C}");
    }

    private void ViewAccountDetails()
    {
        Console.WriteLine("\n=== Account Details ===");
        
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();

        var details = _bankService.GetAccountDetails(accountNumber!);
        
        if (details == null)
        {
            Console.WriteLine("Account not found.");
            return;
        }

        Console.WriteLine($"Account Number: {details.AccountNumber}");
        Console.WriteLine($"Customer Name: {details.CustomerName}");
        Console.WriteLine($"Email: {details.Email}");
        Console.WriteLine($"Phone Number: {details.PhoneNumber}");
        Console.WriteLine($"Account Type: {details.AccountType}");
        Console.WriteLine($"Balance: {details.Balance:C}");
        Console.WriteLine($"Created At: {details.CreatedAt:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Status: {(details.IsActive ? "Active" : "Inactive")}");
    }

    private void ViewTransactionHistory()
    {
        Console.WriteLine("\n=== Transaction History ===");
        
        Console.Write("Enter account number: ");
        string? accountNumber = Console.ReadLine();

        var transactions = _bankService.GetTransactionHistory(accountNumber!);
        
        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions found.");
            return;
        }

        Console.WriteLine("\nTransaction History (Most Recent First):");
        Console.WriteLine("Date | Type | Amount | Balance | Description");
        Console.WriteLine(new string('-', 80));
        
        foreach (var transaction in transactions)
        {
            Console.WriteLine($"{transaction.TransactionDate:yyyy-MM-dd HH:mm} | {transaction.TransactionType,-10} | {transaction.Amount,10:C} | {transaction.BalanceAfterTransaction,10:C} | {transaction.Description}");
        }
    }

    private void ViewAllAccounts()
    {
        Console.WriteLine("\n=== All Accounts ===");
        
        var accounts = _bankService.GetAllAccounts();

        if (accounts.Count == 0)
        {
            Console.WriteLine("No accounts created yet.");
            return;
        }

        Console.WriteLine("Account Number | Customer Name | Type | Balance | Status");
        Console.WriteLine(new string('-', 80));

        foreach (var account in accounts)
        {
            Console.WriteLine($"{account.AccountNumber,-15} | {account.Customer.FullName,-15} | {account.AccountType,-10} | {account.Balance,10:C} | {(account.IsActive ? "Active" : "Inactive")}");
        }
    }

    private void Exit()
    {
        Console.WriteLine("\nThank you for using TechMaster Bank System. Goodbye!");
    }
}
