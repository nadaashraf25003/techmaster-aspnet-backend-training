namespace BankAccountSystem.Models;

public class BankAccount
{
    public string AccountNumber { get; private set; }
    public Customer Customer { get; private set; }
    public decimal Balance { get; private set; }
    public AccountType AccountType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public List<Transaction> Transactions { get; private set; }

    // Critical: Balance must be private set or protected from direct external editing.
    // Balance changes only through controlled behavior: Deposit(), Withdraw(), Transfer()

    public BankAccount(string accountNumber, Customer customer, AccountType accountType, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required.", nameof(accountNumber));
        
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));

        AccountNumber = accountNumber;
        Customer = customer;
        AccountType = accountType;
        Balance = initialBalance;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        Transactions = new List<Transaction>();

        if (initialBalance > 0)
        {
            var depositTransaction = new Transaction(
                accountNumber,
                TransactionType.Deposit,
                initialBalance,
                initialBalance,
                "Initial deposit");
            Transactions.Add(depositTransaction);
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

        Balance += amount;
        
        var transaction = new Transaction(
            AccountNumber,
            TransactionType.Deposit,
            amount,
            Balance,
            "Deposit");
        Transactions.Add(transaction);
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount;
        
        var transaction = new Transaction(
            AccountNumber,
            TransactionType.Withdrawal,
            amount,
            Balance,
            "Withdrawal");
        Transactions.Add(transaction);
    }

    public void Transfer(BankAccount targetAccount, decimal amount)
    {
        if (targetAccount == null)
            throw new ArgumentNullException(nameof(targetAccount));

        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance for transfer.");

        Balance -= amount;
        targetAccount.Balance += amount;
        
        var transaction = new Transaction(
            AccountNumber,
            TransactionType.Transfer,
            amount,
            Balance,
            $"Transfer to {targetAccount.AccountNumber}");
        Transactions.Add(transaction);
        
        var targetTransaction = new Transaction(
            targetAccount.AccountNumber,
            TransactionType.Deposit,
            amount,
            targetAccount.Balance,
            $"Transfer from {AccountNumber}");
        targetAccount.Transactions.Add(targetTransaction);
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