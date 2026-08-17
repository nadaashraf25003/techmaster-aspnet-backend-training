namespace BankAccountSystem.Models;

public class Transaction
{
    public Guid TransactionId { get; private set; }
    public string AccountNumber { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string Description { get; private set; }
    public decimal BalanceAfterTransaction { get; private set; }

    public Transaction(string accountNumber, TransactionType transactionType, decimal amount, decimal balanceAfterTransaction, string description)
    {
        TransactionId = Guid.NewGuid();
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        TransactionType = transactionType;
        Amount = amount;
        BalanceAfterTransaction = balanceAfterTransaction;
        TransactionDate = DateTime.UtcNow;
        Description = description ?? string.Empty;
    }
}