// ============================================================
// FILE: Ch02_Encapsulation.cs
// PURPOSE: Teach Encapsulation — the first and most foundational
//          of the four OOP pillars.
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO: BAD vs GOOD account showing encapsulation's value ──

    // This class shows what happens WITHOUT encapsulation.
    // Every field is public. Anyone can write anything.
    internal class BadBankAccount
    {
        public string  Owner;    // Public field — no protection whatsoever
        public decimal Balance;  // Anyone can set Balance = -999999
        public string  Status;   // Anyone can set Status = "Hacked"
    }

    // This class shows encapsulation DONE RIGHT.
    // Internal state is private. Only controlled methods change it.
    internal class GoodBankAccount
    {
        // ── PRIVATE STATE — the object's internal truth ──────────────
        // These can ONLY be modified through the methods below.
        // No external code can write to these directly.
        private string  _owner;
        private decimal _balance;
        private bool    _isFrozen;
        private int     _failedWithdrawalCount;

        // ── READ-ONLY PROPERTIES — tell callers what they need to know ──
        // External code can READ these, but cannot SET them.
        public string  Owner    => _owner;
        public decimal Balance  => _balance;
        public bool    IsFrozen => _isFrozen;

        // ── COMPUTED PROPERTY — derived from private state, never stored ──
        // Always accurate because it's computed fresh from _balance each time.
        public bool IsOverdrawn => _balance < 0;

        // ── CONSTRUCTOR — the only way to create an account ──────────
        public GoodBankAccount(string owner, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner name is required.");
            if (initialBalance < 0)
                throw new ArgumentException("Cannot open account with negative balance.");

            _owner   = owner;
            _balance = initialBalance;
        }

        // ── PUBLIC METHODS — the ONLY ways to change state ───────────
        // Every business rule is enforced here, in one place.
        public void Deposit(decimal amount)
        {
            if (_isFrozen) throw new InvalidOperationException("Account is frozen.");
            if (amount <= 0) throw new ArgumentException("Deposit must be positive.");
            _balance += amount;
        }

        public Result Withdraw(decimal amount)
        {
            // All business rules in one place:
            if (_isFrozen) return Result.Fail("Account is frozen.");
            if (amount <= 0) return Result.Fail("Amount must be positive.");
            if (amount > _balance)
            {
                _failedWithdrawalCount++;
                if (_failedWithdrawalCount >= 3)
                {
                    _isFrozen = true;
                    return Result.Fail("Account frozen after repeated overdraft attempts.");
                }
                return Result.Fail("Insufficient funds.");
            }
            _balance -= amount;
            _failedWithdrawalCount = 0; // Reset on success
            return Result.Ok();
        }

        public void Freeze()   => _isFrozen = true;
        public void Unfreeze() => _isFrozen = false;

        public override string ToString() =>
            $"[{_owner}] ${_balance:F2} {(_isFrozen ? "🔒FROZEN" : "")}";
    }

    // Simple Result type — better than returning bool for 'why did it fail'
    internal class Result
    {
        public bool   IsSuccess { get; }
        public string Error     { get; }
        private Result(bool ok, string error) { IsSuccess = ok; Error = error; }
        public static Result Ok()           => new Result(true,  null);
        public static Result Fail(string e) => new Result(false, e);
        public override string ToString()   => IsSuccess ? "Success" : $"Failed: {Error}";
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch02_Encapsulation
    {
        public static void Run()
        {
            UI.Header("CHAPTER 2: ENCAPSULATION — GUARD YOUR OBJECT'S STATE");

            UI.Concept(
                "Encapsulation is about two things working together:\n" +
                "  1. BUNDLING: data and the methods that operate on it belong in the same class.\n" +
                "  2. HIDING: the internal details are private. Only a controlled public interface is exposed.\n\n" +
                "This is the first and most important pillar of OOP. If your objects aren't encapsulated, " +
                "everything else in OOP falls apart."
            );
            UI.Pause();

            Section1_TheProblemItSolves();
            Section2_AccessModifiers();
            Section3_Invariants();
            Section4_BestPractices();
            Section5_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_TheProblemItSolves()
        {
            UI.SubHeader("The Problem Encapsulation Solves");

            UI.Analogy("Bank Vault",
                "A bank vault doesn't let you reach in and grab money. " +
                "You interact with a teller who follows rules: you must prove identity, " +
                "you can't withdraw more than you have, the vault logs everything. " +
                "WITHOUT this: anyone could grab any amount, corrupt the balance, delete records. " +
                "Encapsulation is the 'teller' in your code — it controls access to internal state."
            );

            UI.Print("THE DISASTER WITHOUT ENCAPSULATION:\n");

            UI.LiveDemo("Breaking a public-field class in seconds", () =>
            {
                var bad = new BadBankAccount();
                bad.Owner   = "Deepak";
                bad.Balance = 1000;
                bad.Status  = "Active";

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     Starting balance: ${bad.Balance}");

                // Now show how ANY code anywhere can corrupt the state
                bad.Balance = -999999;
                bad.Status  = "HACKED";

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"     After external corruption:");
                Console.WriteLine($"     Balance: ${bad.Balance}   ← should never be negative");
                Console.WriteLine($"     Status:  {bad.Status}     ← completely invalid");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     No exception. No error. The object is silently broken.");
                Console.ResetColor();
            });

            UI.Concept(
                "With a public field, there is NO way to:\n" +
                "  • Prevent Balance from being set to -999999\n" +
                "  • Ensure Status only holds valid values\n" +
                "  • Log who changed the balance and when\n" +
                "  • Enforce business rules (can't overdraw, account must be active)\n\n" +
                "Encapsulation fixes ALL of this by making the internal state private."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_AccessModifiers()
        {
            UI.SubHeader("Access Modifiers — Your Visibility Control Knobs");

            UI.Concept(
                "Access modifiers control WHO can see and use a class member. " +
                "They are the primary mechanism of encapsulation. " +
                "Think of them as permission levels."
            );

            UI.Diagram("Access Modifier Visibility",
                "  Modifier          │ Same Class │ Derived Class │ Same Assembly │ Anywhere",
                "  ──────────────────┼────────────┼───────────────┼───────────────┼──────────",
                "  private           │     ✓      │       ✗       │       ✗       │    ✗",
                "  protected         │     ✓      │       ✓       │       ✗       │    ✗",
                "  internal          │     ✓      │       ✗       │       ✓       │    ✗",
                "  protected internal│     ✓      │       ✓       │       ✓       │    ✗",
                "  public            │     ✓      │       ✓       │       ✓       │    ✓",
                "",
                "  Assembly = one .dll or .exe project file"
            );

            UI.Code("When to use each modifier",
                "public class Employee",
                "{",
                "    // private: ONLY this class accesses it.",
                "    // Use for: fields, internal helpers, things no one else needs to know about.",
                "    private decimal _salary;",
                "    private void LogChange(string msg) { }  // internal audit helper",
                "",
                "    // public: ANYONE can use this.",
                "    // Use for: the intended, stable, tested API of your class.",
                "    public string Name { get; private set; }",
                "    public void GiveRaise(decimal amount) { }",
                "",
                "    // protected: this class AND any class that inherits from this.",
                "    // Use for: behavior that subclasses need to customize.",
                "    protected virtual void OnSalaryChanged() { }",
                "",
                "    // internal: anything in the same project (assembly).",
                "    // Use for: implementation details within a module that shouldn't be public API.",
                "    internal void ResetForTesting() { }",
                "}"
            );

            UI.Print("PRACTICAL RULE — what to default to:\n");
            var rules = new[]
            {
                ("Classes",    "public (if meant for external use) or internal (if project-internal)"),
                ("Fields",     "ALWAYS private. No exceptions. Use properties for external access."),
                ("Properties", "public getter, private or protected setter. Or full public for DTOs."),
                ("Methods",    "private unless external code genuinely needs to call them."),
                ("Constructors","public if external code creates objects; internal/private for factories."),
            };

            foreach (var (kind, rule) in rules)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  {kind,-16}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(rule);
            }
            Console.ResetColor();

            UI.KeyPoint("Default to the most restrictive access modifier. Loosen only when there's a clear reason.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_Invariants()
        {
            UI.SubHeader("Class Invariants — The Rules Your Object Must Always Satisfy");

            UI.Concept(
                "A CLASS INVARIANT is a condition that must be TRUE for the object at ALL times " +
                "(after construction, after every method call). " +
                "Encapsulation's real job is to protect and maintain these invariants."
            );

            UI.Print("EXAMPLES OF INVARIANTS:\n");
            var invariants = new[]
            {
                ("BankAccount",  "_balance >= 0 always (unless overdraft account)"),
                ("Order",        "Items list is never null; at least 1 item when status = 'Submitted'"),
                ("Employee",     "_salary > 0 always; _name is never empty"),
                ("Stack<T>",     "_count >= 0; _count never exceeds array capacity"),
                ("HttpResponse", "StatusCode is between 100 and 599"),
            };

            foreach (var (cls, inv) in invariants)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  {cls,-18}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(inv);
            }
            Console.ResetColor();

            UI.Code("How encapsulation protects invariants",
                "public class Temperature",
                "{",
                "    private double _celsius;  // private: invariant protected",
                "",
                "    // INVARIANT: Temperature cannot be below absolute zero (-273.15°C)",
                "    public double Celsius",
                "    {",
                "        get => _celsius;",
                "        set",
                "        {",
                "            // The setter is the GUARDIAN of the invariant.",
                "            // Without this, anyone could do: temp.Celsius = -1000;",
                "            if (value < -273.15)",
                "                throw new ArgumentOutOfRangeException(",
                "                    nameof(value), \"Temperature cannot be below absolute zero.\");",
                "            _celsius = value;",
                "        }",
                "    }",
                "",
                "    // Computed properties derived from the one source of truth (_celsius).",
                "    // These are always in sync because they compute from _celsius.",
                "    public double Fahrenheit => _celsius * 9 / 5 + 32;",
                "    public double Kelvin     => _celsius + 273.15;",
                "",
                "    public Temperature(double celsius) => Celsius = celsius; // Uses property (validates!)",
                "}"
            );

            UI.Mistake(
                "Storing redundant data that can get out of sync:\n" +
                "  public double Celsius { get; set; }\n" +
                "  public double Fahrenheit { get; set; }  // stored separately!\n" +
                "  // Now: temp.Celsius = 100 but someone forgets to update Fahrenheit → INCONSISTENCY",
                "Have ONE source of truth. Compute all other representations from it. " +
                "Use computed (read-only) properties for derived values."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_BestPractices()
        {
            UI.SubHeader("Encapsulation Best Practices");

            UI.Print("1. NEVER EXPOSE MUTABLE INTERNAL COLLECTIONS\n");
            UI.Code("Leaking internal list — caller can corrupt it",
                "// ❌ BAD: caller gets a reference to the ACTUAL internal list",
                "public List<string> GetHistory() => _history;",
                "// Caller can now: account.GetHistory().Clear(); — wipes audit log!",
                "",
                "// ✓ GOOD: return a read-only view (wrapper, no copy needed)",
                "public IReadOnlyList<string> GetHistory() => _history.AsReadOnly();",
                "",
                "// ✓ ALSO GOOD: return a copy (caller can modify their copy, not ours)",
                "public List<string> GetHistory() => new List<string>(_history);"
            );

            UI.Print("2. TELL, DON'T ASK — don't pull state out and decide outside\n");
            UI.Code("The 'Tell, don't Ask' principle",
                "// ❌ BAD: pulling state out and making decisions outside the object",
                "if (account.Balance >= amount)  // asking",
                "    account.Balance -= amount;   // then manipulating — BREAKS ENCAPSULATION",
                "",
                "// ✓ GOOD: tell the object to do it — let it decide internally",
                "bool success = account.Withdraw(amount);  // object decides and acts",
                "// The Withdraw method checks the balance, decides, and changes it atomically."
            );

            UI.Print("3. KEEP SETTERS PRIVATE UNLESS YOU TRULY NEED PUBLIC SETTERS\n");
            UI.Code("Auto-property setter visibility",
                "// ❌ BAD: both get and set are public — anyone can set anything",
                "public decimal Balance { get; set; }",
                "",
                "// ✓ GOOD for domain objects: only internal code can set balance",
                "public decimal Balance { get; private set; }",
                "",
                "// ✓ GOOD for DTOs/records: public set is fine if this is just a data bag",
                "public record CustomerDto(string Name, string Email);  // DTOs are fine with public data"
            );

            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_LiveDemo()
        {
            UI.SubHeader("Live Demo — Encapsulation Protecting Business Rules");

            UI.Print("The GoodBankAccount class above has these rules enforced internally:\n");
            UI.Print("  • Can't deposit/withdraw from a frozen account");
            UI.Print("  • Can't deposit/withdraw negative amounts");
            UI.Print("  • After 3 failed overdraft attempts, account auto-freezes");
            UI.Print("\nLet's watch these rules in action:\n");

            UI.LiveDemo("Encapsulation enforcing business rules", () =>
            {
                var account = new GoodBankAccount("Deepak", 500);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     Account: {account}");

                // Try to overdraw 3 times
                for (int i = 1; i <= 3; i++)
                {
                    var result = account.Withdraw(10000);
                    Console.ForegroundColor = result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"     Attempt {i} — Withdraw $10,000: {result}");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n     Account after 3 failed attempts: {account}");

                // Try to deposit into frozen account
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n     Trying to deposit $100 into frozen account...");
                try
                {
                    account.Deposit(100);
                }
                catch (InvalidOperationException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"     Exception caught: {ex.Message}");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Key: none of these rules exist in the CALLER's code.");
                Console.WriteLine("     They're all enforced INSIDE the class. That's encapsulation.");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE: You're building an 'Elevator' class for a building with 10 floors.\n\n" +
                "What are the INVARIANTS of this class? What business rules must always be true?\n" +
                "Which fields should be private? Which operations should be public?\n" +
                "What should happen if someone calls GoTo(-5) or GoTo(99)?",

                "INVARIANTS:\n" +
                "  • CurrentFloor is always between 1 and MaxFloors (10)\n" +
                "  • Cannot move while doors are open\n" +
                "  • Can't go to a floor that doesn't exist\n\n" +
                "FIELDS (all private):\n" +
                "  • _currentFloor, _maxFloors, _doorsOpen, _isMoving\n\n" +
                "PROPERTIES (public, read-only):\n" +
                "  • CurrentFloor, IsMoving, DoorsOpen\n\n" +
                "METHODS (public):\n" +
                "  • GoTo(int floor) — validates floor range, enforces 'doors closed' rule\n" +
                "  • OpenDoors() — only when not moving\n" +
                "  • CloseDoors() — transitions state\n\n" +
                "GoTo(-5) → ArgumentOutOfRangeException\n" +
                "GoTo(99) → ArgumentOutOfRangeException\n" +
                "GoTo(5) when doors open → InvalidOperationException"
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Encapsulation", new QuizQuestion[]
            {
                new(
                    "What is encapsulation? (Choose the best answer)",
                    new[] {
                        "Making all fields private so they're harder to access",
                        "Bundling data and behavior together, and hiding internal details behind a controlled public interface",
                        "Preventing a class from being inherited",
                        "Storing objects inside other objects"
                    },
                    1,
                    "Encapsulation has two aspects: bundling (data + methods together) and information hiding (private fields, controlled access)."
                ),
                new(
                    "You have a List<Order> _orders field inside OrderRepository. What should GetAllOrders() return?",
                    new[] {
                        "return _orders;  // direct reference to internal list",
                        "return (List<Order>)_orders.Clone();",
                        "return _orders.AsReadOnly();  // or return a copy",
                        "Make _orders public instead"
                    },
                    2,
                    "Returning the internal list leaks a mutable reference. Callers could add/remove items, bypassing business rules. Return AsReadOnly() or a copy."
                ),
                new(
                    "What is a class invariant?",
                    new[] {
                        "A constant field (readonly) that never changes",
                        "A condition that must always be true about the object's state",
                        "A method that never modifies the object's fields",
                        "A property with only a getter and no setter"
                    },
                    1,
                    "An invariant is a rule the object must always satisfy. E.g., BankAccount: balance >= 0. Encapsulation protects invariants by controlling state changes."
                ),
                new(
                    "What is wrong with: if (account.Balance >= amount) account.Balance -= amount;",
                    new[] {
                        "Nothing — this is the correct way to withdraw money",
                        "Balance should use double, not decimal",
                        "It bypasses the object's own logic, breaks encapsulation, and can violate business rules",
                        "The comparison should use > not >="
                    },
                    2,
                    "'Tell, don't ask.' External code should call account.Withdraw(amount) and let the object handle all rules internally."
                ),
            });
        }
    }
}
