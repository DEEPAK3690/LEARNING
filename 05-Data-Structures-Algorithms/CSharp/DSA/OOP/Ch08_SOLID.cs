// ============================================================
// FILE: Ch08_SOLID.cs
// PURPOSE: Teach all 5 SOLID principles with clear violations
//          and correct implementations.
// ============================================================

namespace DSA.OOP
{
    // ─── S: SINGLE RESPONSIBILITY ──────────────────────────────────────
    // BAD: One class doing everything
    internal class UserServiceBad
    {
        public void CreateUser(string name, string email) { /* ... */ }
        public void SendWelcomeEmail(string email)   { /* sends email  */ }
        public void SaveToDatabase(string name, string email) { /* SQL queries */ }
        public string GenerateReport(string email)   { return "report"; /* formats report */ }
        // 4 reasons to change = 4 responsibilities. Violates SRP.
    }

    // GOOD: Each class has one responsibility
    internal class UserRepository_SRP
    {
        public void Save(string name, string email) { /* only DB logic */ }
        public string GetByEmail(string email)      { return _name; }
        private string _name = "Deepak"; // simplified for demo
    }
    internal class EmailService_SRP
    {
        public void SendWelcome(string email) { /* only email logic */ }
    }
    internal class UserReportGenerator
    {
        public string Generate(string email) { return $"Report for {email}"; /* only reporting */ }
    }
    // Now: change email template → only EmailService changes. No other class touched.

    // ─── O: OPEN/CLOSED ────────────────────────────────────────────────
    internal interface IDiscountStrategy_OCP
    {
        decimal Calculate(decimal price);
        string  Name { get; }
    }

    internal class RegularDiscount : IDiscountStrategy_OCP
    {
        public string  Name => "Regular (5%)";
        public decimal Calculate(decimal price) => price * 0.05m;
    }
    internal class PremiumDiscount : IDiscountStrategy_OCP
    {
        public string  Name => "Premium (10%)";
        public decimal Calculate(decimal price) => price * 0.10m;
    }
    internal class VIPDiscount : IDiscountStrategy_OCP
    {
        public string  Name => "VIP (20%)";
        public decimal Calculate(decimal price) => price * 0.20m;
    }
    // Adding 'Corporate' discount = new class. Zero changes to existing code. OCP satisfied.

    // ─── L: LISKOV SUBSTITUTION ────────────────────────────────────────
    // GOOD LSP-safe hierarchy: use abstract with different types, not IS-A violation
    internal abstract class Bird_LSP
    {
        public string Name { get; }
        protected Bird_LSP(string name) => Name = name;
        public abstract void Describe();
    }

    internal abstract class FlyingBird : Bird_LSP
    {
        protected FlyingBird(string name) : base(name) { }
        public abstract void Fly();
        public override void Describe() => Console.WriteLine($"     {Name} can fly");
    }

    internal abstract class NonFlyingBird : Bird_LSP
    {
        protected NonFlyingBird(string name) : base(name) { }
        public abstract void Run();
        public override void Describe() => Console.WriteLine($"     {Name} runs fast");
    }

    internal class Eagle   : FlyingBird    { public Eagle()   : base("Eagle")   { } public override void Fly() => Console.WriteLine($"     {Name} soars high"); }
    internal class Penguin : NonFlyingBird { public Penguin() : base("Penguin") { } public override void Run() => Console.WriteLine($"     {Name} waddles"); }
    // Now: you can NEVER call Fly() on Penguin. LSP is safe.

    // ─── I: INTERFACE SEGREGATION ──────────────────────────────────────
    // Already covered in Ch07. Brief reference here.
    internal interface IReadOnlyUserStore  { string GetById(int id); IEnumerable<string> GetAll(); }
    internal interface IWriteOnlyUserStore { void Add(string user); void Delete(int id); }
    internal interface IUserStore : IReadOnlyUserStore, IWriteOnlyUserStore { } // Full access

    // ─── D: DEPENDENCY INVERSION ───────────────────────────────────────
    internal interface IOrderRepository_DIP { void Save(string order); string GetById(int id); }
    internal interface IEmailSender_DIP     { void Send(string to, string body); }
    internal interface ILogger_DIP          { void Log(string message); }

    internal class ConsoleLogger_DIP : ILogger_DIP
    {
        public void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"     [LOG] {msg}");
            Console.ResetColor();
        }
    }

    internal class InMemoryOrderRepo_Solid : IOrderRepository_DIP
    {
        private readonly Dictionary<int, string> _orders = new();
        private int _nextId = 1;
        public void Save(string order) { _orders[_nextId++] = order; }
        public string GetById(int id)  => _orders.TryGetValue(id, out var o) ? o : null;
    }

    internal class ConsoleEmailSender : IEmailSender_DIP
    {
        public void Send(string to, string body)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"     📧 Email to {to}: {body}");
            Console.ResetColor();
        }
    }

    // HIGH-LEVEL module depends on ABSTRACTIONS (interfaces), not concrete classes
    internal class OrderService_DIP
    {
        // All dependencies are interfaces — never concrete classes
        private readonly IOrderRepository_DIP _repo;
        private readonly IEmailSender_DIP     _email;
        private readonly ILogger_DIP          _logger;

        // Injected from outside — never 'new'd internally
        public OrderService_DIP(IOrderRepository_DIP repo, IEmailSender_DIP email, ILogger_DIP logger)
        {
            _repo   = repo;
            _email  = email;
            _logger = logger;
        }

        public void PlaceOrder(string customerEmail, string orderDetails)
        {
            _logger.Log($"Placing order for {customerEmail}");
            _repo.Save(orderDetails);
            _email.Send(customerEmail, $"Order confirmed: {orderDetails}");
            _logger.Log("Order placed successfully");
        }
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch08_SOLID
    {
        public static void Run()
        {
            UI.Header("CHAPTER 8: SOLID — THE 5 LAWS OF GOOD OOP DESIGN");

            UI.Concept(
                "SOLID is an acronym for 5 design principles introduced by Robert C. Martin ('Uncle Bob'). " +
                "These principles, when followed, produce code that is:\n" +
                "  • Maintainable — easy to change without breaking other things\n" +
                "  • Extensible — add features by adding code, not modifying existing code\n" +
                "  • Testable — each piece can be tested in isolation\n" +
                "  • Understandable — clear responsibilities, minimal surprises\n\n" +
                "Interviewer tip: every .NET senior role interview covers SOLID."
            );
            UI.Pause();

            Section_S();
            Section_O();
            Section_L();
            Section_I();
            Section_D();
            Section_Together();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section_S()
        {
            UI.SubHeader("S — Single Responsibility Principle (SRP)");

            UI.Print("\"A class should have ONE reason to change.\"\n");
            UI.Print("More practically: a class should DO ONE THING AND DO IT WELL.\n");

            UI.Analogy("A Swiss Army Knife",
                "A Swiss Army knife is clever but you wouldn't use it to cook a meal, " +
                "perform surgery, and cut a wire. For serious work, you use specialized tools: " +
                "a chef's knife for cooking, a scalpel for surgery. " +
                "Classes are the same — specialization makes them sharp and reliable."
            );

            UI.Print("HOW TO IDENTIFY AN SRP VIOLATION:\n");
            UI.Print("  Ask: 'How many different REASONS could cause me to change this class?'");
            UI.Print("  If the answer is more than one, you have an SRP violation.\n");

            UI.Code("VIOLATION — UserServiceBad has 4 responsibilities",
                "public class UserServiceBad",
                "{",
                "    // Reason 1 to change: user creation business rules change",
                "    public void CreateUser(string name, string email) { ... }",
                "",
                "    // Reason 2 to change: email template or SMTP provider changes",
                "    public void SendWelcomeEmail(string email) { ... }",
                "",
                "    // Reason 3 to change: database schema changes",
                "    public void SaveToDatabase(string name, string email) { ... }",
                "",
                "    // Reason 4 to change: report format changes",
                "    public string GenerateReport(string email) { ... }",
                "}"
            );

            UI.Code("FIX — separate classes, each with one responsibility",
                "// Each class changes for exactly ONE reason:",
                "class UserRepository  { void Save(...)     { /* DB only */     } }",
                "class EmailService    { void SendWelcome() { /* Email only */   } }",
                "class UserReports     { string Generate()  { /* Reports only */ } }",
                "class UserService     { void Register()    { orchestrates others } }"
            );

            UI.Good("Benefits of SRP", "Smaller classes that are easier to test, understand, and modify. " +
                "When the email template changes, you only open EmailService — not a 500-line god class.");

            UI.Mistake(
                "Taking SRP too far — one method per class, every tiny thing in its own file. " +
                "That's over-engineering. SRP is about LOGICAL responsibilities, not lines of code.",
                "Group by REASON TO CHANGE. If two methods always change together for the same reason, " +
                "they belong in the same class."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section_O()
        {
            UI.SubHeader("O — Open/Closed Principle (OCP)");

            UI.Print("\"Software should be OPEN for EXTENSION but CLOSED for MODIFICATION.\"\n");
            UI.Print("Add new features by adding new CODE — don't modify existing, working code.\n");

            UI.Analogy("Power Strip",
                "A power strip is 'open' for extension — you can plug in new devices. " +
                "It's 'closed' for modification — you don't rewire the strip itself each time you add a device. " +
                "OCP: design your classes like power strips. New features plug in, don't rewire."
            );

            UI.Code("VIOLATION — Adding a discount type requires modifying existing method",
                "// ❌ BAD: Every new discount type = modify this existing method",
                "public decimal GetDiscount(string customerType, decimal price)",
                "{",
                "    if (customerType == \"Regular\") return price * 0.05m;",
                "    if (customerType == \"Premium\") return price * 0.10m;",
                "    if (customerType == \"VIP\")     return price * 0.20m;",
                "    // To add 'Corporate': MODIFY THIS METHOD. Risk: break existing logic.",
                "    return 0;",
                "}"
            );

            UI.Code("FIX — Add new discount via new class, zero changes to existing code",
                "// ✓ GOOD: New discount = new class implementing IDiscountStrategy",
                "public interface IDiscountStrategy { decimal Calculate(decimal price); }",
                "public class RegularDiscount : IDiscountStrategy { ... } // 5%",
                "public class PremiumDiscount : IDiscountStrategy { ... } // 10%",
                "public class VIPDiscount     : IDiscountStrategy { ... } // 20%",
                "",
                "// Adding Corporate discount tomorrow:",
                "public class CorporateDiscount : IDiscountStrategy",
                "{",
                "    public decimal Calculate(decimal price) => price * 0.30m; // 30%",
                "}",
                "// Zero changes to existing discount classes or the service using them."
            );

            UI.LiveDemo("OCP in action — adding discount types", () =>
            {
                var discounts = new List<IDiscountStrategy_OCP>
                {
                    new RegularDiscount(),
                    new PremiumDiscount(),
                    new VIPDiscount(),
                };

                decimal price = 100m;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     Order price: ${price}");
                Console.ResetColor();

                foreach (var d in discounts)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"     {d.Name,-20} saves: ${d.Calculate(price):F2}");
                }
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section_L()
        {
            UI.SubHeader("L — Liskov Substitution Principle (LSP)");

            UI.Print("\"Derived types must be substitutable for their base types.\"\n");
            UI.Print("If you use a derived class anywhere the base class is expected, it must BEHAVE CORRECTLY.\n");

            UI.Analogy("Hiring a Replacement",
                "If your manager goes on vacation and their replacement is hired, " +
                "you expect the replacement to be able to do everything the manager did. " +
                "If the replacement refuses to run team meetings (a core manager responsibility), " +
                "the substitution fails. LSP says the same about classes."
            );

            UI.Code("The Classic LSP Violation — Rectangle and Square",
                "public class Rectangle",
                "{",
                "    public virtual double Width  { get; set; }",
                "    public virtual double Height { get; set; }",
                "    public double Area => Width * Height;",
                "}",
                "",
                "public class Square : Rectangle",
                "{",
                "    // Square keeps Width = Height always",
                "    public override double Width  { set { base.Width = value; base.Height = value; } }",
                "    public override double Height { set { base.Width = value; base.Height = value; } }",
                "}",
                "",
                "// This code 'works' for Rectangle but BREAKS for Square (LSP violated):",
                "void SetAndCheck(Rectangle r)",
                "{",
                "    r.Width  = 5;",
                "    r.Height = 3;",
                "    // Expected: Area = 15",
                "    // Actual with Square: Area = 9 (because setting Height also set Width to 3!)",
                "    Console.WriteLine(r.Area);  // Wrong for Square!",
                "}"
            );

            UI.Code("FIX — don't force IS-A that isn't truly substitutable",
                "// ✓ GOOD: Separate hierarchy, no false IS-A",
                "public abstract class Shape { public abstract double Area(); }",
                "public class Rectangle : Shape { ... Width, Height ... }",
                "public class Square    : Shape { ... Side ...         }",
                "// Square IS-A Shape (valid). Square IS NOT-A Rectangle (they're siblings).",
                "",
                "// LSP CHECKLIST for derived classes:",
                "// ✓ Don't throw exceptions the base doesn't throw",
                "// ✓ Don't strengthen preconditions (accept at least what base accepts)",
                "// ✓ Don't weaken postconditions (guarantee at least what base guarantees)",
                "// ✓ Don't violate invariants the base establishes"
            );

            UI.LiveDemo("LSP-safe Bird hierarchy", () =>
            {
                var birds = new List<Bird_LSP> { new Eagle(), new Penguin() };
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     All birds can Describe() — that's safe:");
                Console.ResetColor();
                foreach (var b in birds) b.Describe();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Flying birds can Fly():");
                Console.ResetColor();
                var flying = new List<FlyingBird> { new Eagle() };
                foreach (var b in flying) b.Fly();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Non-flying birds Run():");
                Console.ResetColor();
                var running = new List<NonFlyingBird> { new Penguin() };
                foreach (var b in running) b.Run();
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section_I()
        {
            UI.SubHeader("I — Interface Segregation Principle (ISP)");

            UI.Print("\"Clients should not be forced to depend on interfaces they don't use.\"\n");
            UI.Print("Break large interfaces into smaller, focused ones.\n");

            UI.Concept(
                "We covered this deeply in Chapter 7. Brief recap:\n\n" +
                "  Fat interface → classes implement methods they can't do → throws NotImplementedException\n" +
                "  Many small interfaces → each class implements only what it actually supports\n\n" +
                "In production, this shows up as: IReadableRepository, IWritableRepository, ISearchableRepository — " +
                "not one giant IRepository that forces every consumer to have write access."
            );

            UI.Code("ISP applied to a data store",
                "// Segregated: consumers only get what they need",
                "interface IReadOnlyUserStore  { string Get(int id); IEnumerable<string> All(); }",
                "interface IWriteOnlyUserStore { void Add(string u); void Delete(int id); }",
                "",
                "// Admin panel: needs full access",
                "class AdminPanel { public AdminPanel(IUserStore store) { } }",
                "",
                "// Public API: read-only is enough — can't accidentally call Delete()",
                "class PublicUserApi { public PublicUserApi(IReadOnlyUserStore store) { } }",
                "",
                "// Background cleanup: only needs to delete",
                "class UserCleanupJob { public UserCleanupJob(IWriteOnlyUserStore store) { } }"
            );

            UI.KeyPoint("Principle of Least Privilege: give each component the minimum access it needs to do its job.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section_D()
        {
            UI.SubHeader("D — Dependency Inversion Principle (DIP)");

            UI.Print("\"High-level modules should not depend on low-level modules.\"\n");
            UI.Print("\"Both should depend on abstractions (interfaces).\"\n");

            UI.Analogy("Electrical Plug Standards",
                "Your laptop (high-level) doesn't care which power outlet (low-level) you plug it into " +
                "as long as it provides the right voltage and connector type (the interface/standard). " +
                "The outlet depends on the standard, the laptop depends on the standard. " +
                "Neither depends on the other directly."
            );

            UI.Code("VIOLATION — high-level OrderService depends on concrete low-level class",
                "// ❌ BAD: OrderService creates its own dependencies (tightly coupled)",
                "public class OrderService_Bad",
                "{",
                "    // These are CONCRETE types — changing them requires changing OrderService",
                "    private SqlOrderDatabase _db    = new SqlOrderDatabase(connectionString);",
                "    private SmtpEmailSender  _email = new SmtpEmailSender(smtpServer);",
                "    private FileLogger       _log   = new FileLogger(\"/logs/orders.log\");",
                "",
                "    // To test: need SQL Server + SMTP server + file system. Impossible to test in isolation.",
                "    // To switch to MongoDB: modify OrderService. Change email to SendGrid: modify OrderService.",
                "}"
            );

            UI.Code("FIX — depend on interfaces, inject from outside",
                "// ✓ GOOD: OrderService depends on INTERFACES only",
                "public class OrderService",
                "{",
                "    private readonly IOrderRepository _repo;   // not SqlOrderDatabase",
                "    private readonly IEmailSender     _email;  // not SmtpEmailSender",
                "    private readonly ILogger          _logger; // not FileLogger",
                "",
                "    // Dependencies INJECTED — caller decides which implementations to use",
                "    public OrderService(IOrderRepository repo, IEmailSender email, ILogger logger)",
                "    { _repo = repo; _email = email; _logger = logger; }",
                "",
                "    // In tests:  inject InMemoryOrderRepo, FakeEmailSender, NullLogger",
                "    // In prod:   inject SqlOrderRepo, SendGridEmailSender, SerilogLogger",
                "    // Zero code changes in OrderService itself when implementations swap.",
                "}"
            );

            UI.LiveDemo("DIP — wiring OrderService with injected dependencies", () =>
            {
                // Wire up with concrete implementations — this is the "composition root"
                var repo    = new InMemoryOrderRepo_Solid();
                var email   = new ConsoleEmailSender();
                var logger  = new ConsoleLogger_DIP();
                var service = new OrderService_DIP(repo, email, logger);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     service.PlaceOrder(\"deepak@example.com\", \"2x USB Hub\");");
                Console.ResetColor();
                Console.WriteLine();

                service.PlaceOrder("deepak@example.com", "2x USB Hub");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     To test: use InMemoryOrderRepo + FakeEmailSender + NullLogger");
                Console.WriteLine("     To prod: swap in SqlOrderRepo + SendGridEmailSender + SerilogLogger");
                Console.ResetColor();
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section_Together()
        {
            UI.SubHeader("SOLID Working Together — The Full Picture");

            UI.Diagram("How the 5 principles reinforce each other",
                "  S — One class, one job → small, focused classes",
                "       ↓  these small classes are easier to...",
                "  O — Extend by adding new classes, not editing old ones",
                "       ↓  new classes are substitutable because of...",
                "  L — Derived types behave exactly like their base types",
                "       ↓  which requires well-designed contracts...",
                "  I — Small, focused interfaces everyone can use exactly as needed",
                "       ↓  and depend on those interfaces via...",
                "  D — Inject dependencies — high-level modules stay pure business logic",
                "",
                "  Result: code that is testable, extensible, and maintainable."
            );

            UI.Print("QUICK VIOLATION CHECKLIST:\n");
            var checks = new[]
            {
                ("SRP violated?", "Class has multiple reasons to change, or 'and' in its description"),
                ("OCP violated?", "Adding a feature requires modifying existing if-else or switch"),
                ("LSP violated?", "Subclass throws NotImplementedException or behaves unexpectedly"),
                ("ISP violated?", "Class implements methods it throws or leaves empty"),
                ("DIP violated?", "Class uses 'new ConcreteClass()' inside for its dependencies"),
            };

            foreach (var (principle, sign) in checks)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  {principle,-18}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(sign);
            }
            Console.ResetColor();
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("SOLID Principles", new QuizQuestion[]
            {
                new(
                    "A UserService class handles user registration, sends emails, saves to database, and generates reports. Which SOLID principle does this violate?",
                    new[] {
                        "Open/Closed Principle",
                        "Single Responsibility Principle",
                        "Liskov Substitution Principle",
                        "Dependency Inversion Principle"
                    },
                    1,
                    "SRP: the class has 4 reasons to change (email change, DB change, report format change, registration logic change). Split into focused classes."
                ),
                new(
                    "You add a 'switch' statement to handle a new discount type, modifying existing code. Which principle is violated?",
                    new[] {
                        "Single Responsibility Principle",
                        "Liskov Substitution Principle",
                        "Open/Closed Principle",
                        "Interface Segregation Principle"
                    },
                    2,
                    "OCP: open for extension, closed for modification. New discount types should be new classes, not modifications to existing switch logic."
                ),
                new(
                    "A class inherits from Bird and is forced to implement Fly(), but throws NotSupportedException. Which principle is violated?",
                    new[] {
                        "Single Responsibility Principle",
                        "Liskov Substitution Principle",
                        "Interface Segregation Principle",
                        "Dependency Inversion Principle"
                    },
                    1,
                    "LSP: a Penguin is a Bird, but cannot fly. Forcing Fly() on it means it's NOT a valid substitute for Bird in flying contexts. Redesign the hierarchy."
                ),
                new(
                    "A class implements IWorker which has Work(), Eat(), and Sleep(). The class is a Robot and throws on Eat() and Sleep(). Which principle is violated?",
                    new[] {
                        "Open/Closed Principle",
                        "Liskov Substitution Principle",
                        "Interface Segregation Principle",
                        "Dependency Inversion Principle"
                    },
                    2,
                    "ISP: Robot is forced to implement methods it doesn't use. Split IWorker into IWorkable, IFeedable, ISleepable. Robot implements only IWorkable."
                ),
                new(
                    "OrderService creates 'new SqlDatabase()' and 'new SmtpSender()' internally. Which principle is violated?",
                    new[] {
                        "Open/Closed Principle",
                        "Single Responsibility Principle",
                        "Interface Segregation Principle",
                        "Dependency Inversion Principle"
                    },
                    3,
                    "DIP: high-level OrderService depends on concrete low-level classes. It should depend on IDatabase and IEmailSender interfaces, injected via constructor."
                ),
            });
        }
    }
}
