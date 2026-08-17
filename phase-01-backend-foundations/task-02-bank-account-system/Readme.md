# 🏦 Task 02 - Bank Account System

A console-based bank account management system built with C# (.NET), demonstrating core OOP principles, layered architecture, and business logic encapsulation.

---

## Features

| # | Feature | Description |
|---|---------|-------------|
| 1 | **Create Customer Account** | Register a new customer and open a bank account with an initial balance |
| 2 | **Deposit Money** | Deposit a positive amount into an existing account |
| 3 | **Withdraw Money** | Withdraw money while preventing overdrafts |
| 4 | **Transfer Money** | Transfer funds between two different accounts |
| 5 | **View Account Details** | Display full account and customer information |
| 6 | **View Transaction History** | Show all transactions sorted by date (newest first) |
| 7 | **View All Accounts** | List all accounts with a summary view |
| 8 | **Exit** | Gracefully exit the application |

---

##  Project Structure

```
task-02-bank-account-system/
│
├── Program.cs                        ← Entry point
├── Readme.md
│
└── BankAccountSystem/
    ├── Models/                        ← Data layer
    │   ├── Customer.cs                ← Customer entity
    │   ├── BankAccount.cs             ← Account entity + business methods
    │   ├── Transaction.cs             ← Transaction record
    │   ├── AccountType.cs             ← Enum: Checking | Savings | Business
    │   └── TransactionType.cs         ← Enum: Deposit | Withdrawal | Transfer
    │
    ├── Services/                      ← Business logic layer
    │   └── BankService.cs             ← Orchestrates all operations + AccountDetails DTO
    │
    └── UI/                            ← Presentation layer
        └── ConsoleMenu.cs             ← Console menu and user interaction
```

---

##  Architecture

The system follows a **3-Layer Architecture** to separate concerns:

```
[ UI Layer ]         ConsoleMenu.cs
      ↓  calls
[ Service Layer ]    BankService.cs
      ↓  creates/uses
[ Model Layer ]      BankAccount, Customer, Transaction
```

- **UI Layer** — handles all Console.ReadLine() / Console.WriteLine() interactions. Contains zero business logic.
- **Service Layer** — validates inputs, coordinates between models, and exposes clean methods to the UI.
- **Model Layer** — owns the data and enforces business rules internally (balance can only change through Deposit(), Withdraw(), Transfer()).

---

##  Key Classes

### `Customer`
Stores customer personal data. All properties are `private set` — immutable after creation.

| Property | Type | Description |
|----------|------|-------------|
| `CustomerId` | `Guid` | Auto-generated unique ID |
| `FullName` | `string` | Required |
| `Email` | `string` | Required |
| `PhoneNumber` | `string` | Required |
| `CreatedAt` | `DateTime` | Timestamp (UTC) |

---

### `BankAccount`
The core entity. Balance is **protected** — only changed through controlled methods.

| Member | Description |
|--------|-------------|
| `Balance` | `private set` — cannot be set directly from outside |
| `Deposit(amount)` | Validates amount > 0, increases balance, logs transaction |
| `Withdraw(amount)` | Validates amount > 0 and amount <= Balance, decreases balance, logs transaction |
| `Transfer(target, amount)` | Debits source, credits target, logs two transactions |
| `Transactions` | `List<Transaction>` — full audit trail |

---

### `Transaction`
An immutable record created automatically on every operation.

| Property | Description |
|----------|-------------|
| `TransactionId` | `Guid` — unique per transaction |
| `TransactionType` | `Deposit` / `Withdrawal` / `Transfer` |
| `Amount` | The transaction amount |
| `BalanceAfterTransaction` | Balance snapshot after the operation |
| `TransactionDate` | UTC timestamp |
| `Description` | Human-readable label |

---

### `BankService`
Holds in-memory lists of customers and accounts. Acts as the bridge between UI and Models.

Key methods:
- `CreateCustomer(name, email, phone)` → validates and returns a Customer
- `CreateAccount(customer, type, balance)` → generates a unique account number and returns BankAccount
- `Deposit(accountNumber, amount)` → finds account, delegates to account.Deposit()
- `Withdraw(accountNumber, amount)` → finds account, delegates to account.Withdraw()
- `Transfer(from, to, amount)` → validates both accounts, delegates to fromAccount.Transfer()
- `GetAccountDetails(accountNumber)` → returns an AccountDetails DTO for display
- `GetTransactionHistory(accountNumber)` → returns transactions sorted newest-first

---

##  Validation Rules

### Create Account
- Full name, email, and phone are **required**
- Initial balance **cannot be negative**

### Deposit
- Account must **exist**
- Amount must be **greater than zero**

### Withdraw
- Account must **exist**
- Amount must be **greater than zero**
- Amount must **not exceed current balance**

### Transfer
- Both accounts must **exist**
- Source and destination **cannot be the same account**
- Amount must be **greater than zero**
- Source balance must be **sufficient**

---

##  Invalid Cases Covered

| Operation | Invalid Case | Error Thrown |
|-----------|-------------|--------------|
| Deposit | Account not found | `InvalidOperationException` |
| Deposit | Amount = 0 or negative | `ArgumentException` |
| Withdraw | Account not found | `InvalidOperationException` |
| Withdraw | Amount exceeds balance | `InvalidOperationException` |
| Withdraw | Negative amount | `ArgumentException` |
| Transfer | Source/destination not found | `InvalidOperationException` |
| Transfer | Same account | `InvalidOperationException` |
| Transfer | Insufficient balance | `InvalidOperationException` |

> All errors are caught by the try/catch block in ConsoleMenu.Run() and displayed to the user without crashing the application.

---

##  Console Menu

```
====== TechMaster Bank System ======
1. Create Customer Account
2. Deposit Money
3. Withdraw Money
4. Transfer Money
5. View Account Details
6. View Transaction History
7. View All Accounts
8. Exit
Choose an option:
```

---

##  Manual Testing Scenarios

### Scenario 1 — Happy Path
1. Create a customer account with $1000 initial balance → note the account number
2. Deposit $500 → balance should be $1500
3. Withdraw $200 → balance should be $1300
4. Create a second account, then transfer $300 from the first to the second
5. View transaction history on the first account → should show 4 transactions

### Scenario 2 — Error Handling
1. Try depositing 0 → should show error
2. Try withdrawing more than the balance → should show "Insufficient balance"
3. Try transferring to the same account number → should show error
4. Enter a non-existent account number → should show "Account not found"

---

##  OOP Concepts Demonstrated

| Concept | Where Applied |
|---------|--------------|
| **Encapsulation** | `Balance` with `private set`, all business rules inside model methods |
| **Object-Oriented Design** | Each entity is a dedicated class with its own responsibilities |
| **Layered Architecture** | Models / Services / UI — strict separation of concerns |
| **Exception Handling** | `try/catch` at the UI boundary, exceptions thrown from Models and Services |
| **Enums** | `AccountType`, `TransactionType` for type-safe constants |
| **DTO Pattern** | `AccountDetails` class used to expose data to UI without exposing the full model |
| **LINQ** | `FirstOrDefault`, `OrderByDescending`, `Any`, `ToList` |

