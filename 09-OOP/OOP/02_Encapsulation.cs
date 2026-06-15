// ============================================================
// CONCEPT 2: ENCAPSULATION — Protecting Data
// ============================================================
//
// ANALOGY: A TV remote has buttons (public interface) but hides
//          the circuit board inside (private implementation).
//          You use the remote without knowing how it works inside.
//
// DEFINITION: Bundling data + methods together AND controlling
//             access to that data using access modifiers.
//
// WHY: Prevents invalid state. E.g., age can't be -5.
//      Lets you change internals without breaking outside code.

namespace OOP;

public class BankAccount
{
    // ---- ACCESS MODIFIERS ----
    //
    // public          → accessible from ANYWHERE
    // private         → accessible only inside THIS class (most restrictive for data)
    // protected       → accessible in this class + derived (child) classes
    // internal        → accessible within the same ASSEMBLY (project/dll)
    // protected internal → protected OR internal (either rule allows access)
    // private protected  → protected AND internal (both rules must apply)

    private decimal _balance;       // nobody outside can touch this directly
    public string OwnerName { get; set; } // public: anyone can read or write

    // ----- TYPES OF PROPERTIES -----

    // 1. AUTO-PROPERTY — compiler generates the hidden backing field for you
    //    Use when you don't need validation logic
    public string Bank { get; set; } = "Default Bank"; // with default value

    // 2. READ-ONLY AUTO-PROPERTY — can only be set in constructor or initializer
    public string AccountNumber { get; private set; }  // set is private = read-only from outside

    // 3. FULL PROPERTY with backing field — use when you need validation
    private decimal _interestRate;
    public decimal InterestRate
    {
        get => _interestRate;
        set
        {
            if (value < 0 || value > 1)
                throw new ArgumentException("Interest rate must be between 0 and 1");
            _interestRate = value;
        }
    }

    // 4. COMPUTED PROPERTY (no backing field — calculated on the fly)
    public decimal AnnualInterest => _balance * _interestRate; // expression-bodied property

    // 5. GET-ONLY PROPERTY (truly immutable after construction)
    public bool IsOverdrawn => _balance < 0;

    public BankAccount(string owner, string accountNumber, decimal initialBalance)
    {
        OwnerName = owner;
        AccountNumber = accountNumber;  // set via private setter — only allowed in constructor
        _balance = initialBalance;
        _interestRate = 0.05m;
    }

    // ---- CONTROLLED ACCESS TO PRIVATE DATA VIA METHODS ----

    // Instead of making _balance public, we expose controlled operations
    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Deposit must be positive.");
        _balance += amount; // only this class can modify _balance
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (amount > _balance) return false; // business rule enforced here
        _balance -= amount;
        return true;
    }

    // Read-only access to balance — outside code can SEE it but not SET it directly
    public decimal GetBalance() => _balance;

    public override string ToString() =>
        $"[{AccountNumber}] {OwnerName} | Balance: {_balance:C} | Rate: {_interestRate:P}";
}

public static class EncapsulationDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 2. ENCAPSULATION ==========");

        var account = new BankAccount("Deepak", "ACC-001", 1000m);

        // We interact through the PUBLIC interface only
        account.Deposit(500);
        bool success = account.Withdraw(200);
        Console.WriteLine($"Withdraw success: {success}");
        Console.WriteLine(account);

        // account._balance = 999999; // COMPILE ERROR — private field, inaccessible
        // account.AccountNumber = "HACKED"; // COMPILE ERROR — private setter

        // Validation is enforced inside the property setter
        try
        {
            account.InterestRate = 5.0m; // invalid: > 1
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Caught: {ex.Message}");
        }

        account.InterestRate = 0.07m; // valid
        Console.WriteLine($"Annual Interest: {account.AnnualInterest:C}");
        Console.WriteLine($"Is Overdrawn: {account.IsOverdrawn}");

        // KEY TAKEAWAY:
        // Encapsulation = private fields + public properties/methods
        // You HIDE the "how" and expose only the "what"
        // This makes code safe, maintainable, and refactorable
    }
}
