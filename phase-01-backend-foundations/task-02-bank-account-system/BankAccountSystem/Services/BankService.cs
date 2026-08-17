using BankAccountSystem.Models;

namespace BankAccountSystem.Services;

public class BankService
{
    private readonly List<Customer> _customers;
    private readonly List<BankAccount> _accounts;
    private int _accountNumberCounter;

    public BankService()
    {
        _customers = new List<Customer>();
        _accounts = new List<BankAccount>();
        _accountNumberCounter = 1000; // Starting account number
    }

    /// <summary>
    /// Creates a customer account with validation.
    /// </summary>
    /// 
    public Customer CreateCustomer(string fullName, string email, string phoneNumber)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

        var customer = new Customer(fullName, email, phoneNumber);
        _customers.Add(customer);
        return customer;
    }

    /// <summary>
    /// Creates a bank account with validation.
    /// </summary>
    public BankAccount CreateAccount(Customer customer, AccountType accountType, decimal initialBalance = 0)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));

        // Generate unique account number
        string accountNumber = GenerateUniqueAccountNumber();

        var account = new BankAccount(accountNumber, customer, accountType, initialBalance);
        _accounts.Add(account);
        return account;
    }

    /// <summary>
    /// Generates a unique account number.
    /// </summary>
    private string GenerateUniqueAccountNumber()
    {
        string accountNumber;
        do
        {
            _accountNumberCounter++;
            accountNumber = _accountNumberCounter.ToString();
        } 
        while (_accounts.Any(a => a.AccountNumber == accountNumber));

        return accountNumber;
    }

    /// <summary>
    /// Gets an account by account number.
    /// </summary>
    public BankAccount? GetAccount(string accountNumber)
    {
        return _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
    }

    /// <summary>
    /// Gets all accounts.
    /// </summary>
    public List<BankAccount> GetAllAccounts()
    {
        return _accounts.ToList();
    }

    /// <summary>
    /// Gets all customers.
    /// </summary>
    public List<Customer> GetAllCustomers()
    {
        return _customers.ToList();
    }

    /// <summary>
    /// Deposits money into an account.
    /// </summary>
    public void Deposit(string accountNumber, decimal amount)
    {
        var account = GetAccount(accountNumber) 
            ?? throw new InvalidOperationException("Account not found.");
        
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

        account.Deposit(amount);
    }

    /// <summary>
    /// Withdraws money from an account.
    /// </summary>
    public void Withdraw(string accountNumber, decimal amount)
    {
        var account = GetAccount(accountNumber) 
            ?? throw new InvalidOperationException("Account not found.");
        
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

        account.Withdraw(amount);
    }

    /// <summary>
    /// Transfers money between two accounts.
    /// </summary>
    public void Transfer(string fromAccountNumber, string toAccountNumber, decimal amount)
    {
        if (fromAccountNumber == toAccountNumber)
            throw new InvalidOperationException("Source and destination accounts cannot be the same.");

        var fromAccount = GetAccount(fromAccountNumber) 
            ?? throw new InvalidOperationException("Source account not found.");
        
        var toAccount = GetAccount(toAccountNumber) 
            ?? throw new InvalidOperationException("Destination account not found.");
        
        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

        fromAccount.Transfer(toAccount, amount);
    }

    /// <summary>
    /// Gets account details.
    /// </summary>
    public AccountDetails? GetAccountDetails(string accountNumber)
    {
        var account = GetAccount(accountNumber);
        if (account == null)
            return null;

        return new AccountDetails
        {
            AccountNumber = account.AccountNumber,
            CustomerName = account.Customer.FullName,
            Email = account.Customer.Email,
            PhoneNumber = account.Customer.PhoneNumber,
            AccountType = account.AccountType.ToString(),
            Balance = account.Balance,
            CreatedAt = account.CreatedAt,
            IsActive = account.IsActive
        };
    }

    /// <summary>
    /// Gets transaction history for an account, sorted by date descending.
    /// </summary>
    public List<Transaction> GetTransactionHistory(string accountNumber)
    {
        var account = GetAccount(accountNumber) 
            ?? throw new InvalidOperationException("Account not found.");
        
        return account.Transactions
            .OrderByDescending(t => t.TransactionDate)
            .ToList();
    }
}

/// <summary>
/// Account details DTO for display purposes.
/// </summary>
public class AccountDetails
{
    public string AccountNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}