// ============================================================
// FILE: Ch09_Patterns.cs
// PURPOSE: Teach the most important design patterns with
//          runnable demos and real-world context.
// ============================================================

namespace DSA.OOP
{
    // ─── SINGLETON PATTERN ─────────────────────────────────────────────
    // Thread-safe Singleton using Lazy<T> — the modern C# way
    internal sealed class AppSettings
    {
        // Lazy<T> initializes on first access, thread-safe by default.
        // 'sealed' prevents inheritance that could create second instances.
        private static readonly Lazy<AppSettings> _instance =
            new(() => new AppSettings());

        public static AppSettings Instance => _instance.Value;

        // Private constructor: no one can call 'new AppSettings()' from outside
        private AppSettings()
        {
            MaxRetries = 3;
            ApiBaseUrl = "https://api.example.com";
            Timeout    = TimeSpan.FromSeconds(30);
        }

        public int      MaxRetries { get; }
        public string   ApiBaseUrl { get; }
        public TimeSpan Timeout    { get; }
    }

    // ─── FACTORY METHOD PATTERN ────────────────────────────────────────
    internal interface ILoggerFactory_P
    {
        ILogger_P CreateLogger(string category);
    }

    internal interface ILogger_P
    {
        void Log(string level, string message);
    }

    internal class ConsoleLoggerFactory : ILoggerFactory_P
    {
        public ILogger_P CreateLogger(string category) => new ConsoleLogger_P(category);
    }

    internal class ConsoleLogger_P : ILogger_P
    {
        private readonly string _category;
        public ConsoleLogger_P(string category) => _category = category;
        public void Log(string level, string message)
        {
            var color = level == "ERROR" ? ConsoleColor.Red
                      : level == "WARN"  ? ConsoleColor.Yellow
                      : ConsoleColor.White;
            Console.ForegroundColor = color;
            Console.WriteLine($"     [{level}] [{_category}] {message}");
            Console.ResetColor();
        }
    }

    // ─── BUILDER PATTERN ───────────────────────────────────────────────
    internal class SqlQuery
    {
        public string Table      { get; }
        public string Columns    { get; }
        public string Conditions { get; }
        public string OrderBy    { get; }
        public int?   Limit      { get; }

        private SqlQuery(Builder b)
        {
            Table      = b._table;
            Columns    = b._columns.Any() ? string.Join(", ", b._columns) : "*";
            Conditions = b._conditions.Any() ? string.Join(" AND ", b._conditions) : null;
            OrderBy    = b._orderBy;
            Limit      = b._limit;
        }

        public override string ToString()
        {
            var sql = $"SELECT {Columns} FROM {Table}";
            if (Conditions != null) sql += $" WHERE {Conditions}";
            if (OrderBy    != null) sql += $" ORDER BY {OrderBy}";
            if (Limit.HasValue)     sql += $" LIMIT {Limit}";
            return sql;
        }

        // Nested Builder class — pattern: inner class with fluent methods
        internal class Builder
        {
            internal string       _table;
            internal List<string> _columns    = new();
            internal List<string> _conditions = new();
            internal string       _orderBy;
            internal int?         _limit;

            public Builder From(string table)         { _table = table; return this; }
            public Builder Select(params string[] c)  { _columns.AddRange(c); return this; }
            public Builder Where(string condition)     { _conditions.Add(condition); return this; }
            public Builder OrderBy(string col)         { _orderBy = col; return this; }
            public Builder Limit(int n)                { _limit = n; return this; }
            public SqlQuery Build()                    => new SqlQuery(this);
        }
    }

    // ─── DECORATOR PATTERN ─────────────────────────────────────────────
    internal interface ITextProcessor
    {
        string Process(string input);
    }

    internal class PlainTextProcessor : ITextProcessor
    {
        public string Process(string input) => input; // baseline: no-op
    }

    // Each decorator wraps another ITextProcessor — adds one layer of behavior
    internal class TrimDecorator : ITextProcessor
    {
        private readonly ITextProcessor _inner;
        public TrimDecorator(ITextProcessor inner) => _inner = inner;
        public string Process(string input) => _inner.Process(input).Trim();
    }

    internal class UpperCaseDecorator : ITextProcessor
    {
        private readonly ITextProcessor _inner;
        public UpperCaseDecorator(ITextProcessor inner) => _inner = inner;
        public string Process(string input) => _inner.Process(input).ToUpper();
    }

    internal class CensorDecorator : ITextProcessor
    {
        private readonly ITextProcessor _inner;
        private readonly string[] _badWords = { "bad", "ugly", "wrong" };
        public CensorDecorator(ITextProcessor inner) => _inner = inner;
        public string Process(string input)
        {
            var result = _inner.Process(input);
            foreach (var word in _badWords)
                result = result.Replace(word, new string('*', word.Length), StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }

    // ─── OBSERVER PATTERN ──────────────────────────────────────────────
    // C# events ARE the Observer pattern built into the language
    internal class StockTicker
    {
        public string Symbol   { get; }
        private decimal _price;

        public event EventHandler<PriceChangedEventArgs> PriceChanged;

        public StockTicker(string symbol, decimal price) { Symbol = symbol; _price = price; }

        public void UpdatePrice(decimal newPrice)
        {
            var old = _price;
            _price = newPrice;
            // Raise the event — all subscribers get notified
            PriceChanged?.Invoke(this, new PriceChangedEventArgs(Symbol, old, newPrice));
        }
    }

    internal class PriceChangedEventArgs : EventArgs
    {
        public string  Symbol   { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public decimal Change   => NewPrice - OldPrice;
        public PriceChangedEventArgs(string s, decimal o, decimal n) { Symbol = s; OldPrice = o; NewPrice = n; }
    }

    // ─── STRATEGY PATTERN ──────────────────────────────────────────────
    internal interface ISortStrategy_P<T> where T : IComparable<T>
    {
        void Sort(List<T> items);
        string Name { get; }
    }

    internal class BubbleSort_P<T> : ISortStrategy_P<T> where T : IComparable<T>
    {
        public string Name => "Bubble Sort";
        public void Sort(List<T> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
                for (int j = 0; j < items.Count - i - 1; j++)
                    if (items[j].CompareTo(items[j + 1]) > 0)
                        (items[j], items[j + 1]) = (items[j + 1], items[j]);
        }
    }

    internal class LinqSort_P<T> : ISortStrategy_P<T> where T : IComparable<T>
    {
        public string Name => "LINQ Sort";
        public void Sort(List<T> items)
        {
            var sorted = items.OrderBy(x => x).ToList();
            items.Clear();
            items.AddRange(sorted);
        }
    }

    // ─── COMMAND PATTERN ───────────────────────────────────────────────
    internal interface ICommand_P
    {
        void Execute();
        void Undo();
        string Description { get; }
    }

    internal class TextDocument
    {
        private string _content = "";
        public string Content => _content;
        public void Insert(string text, int pos) => _content = _content.Insert(pos, text);
        public void Remove(int pos, int len) => _content = _content.Remove(pos, len);
    }

    internal class InsertTextCommand : ICommand_P
    {
        private readonly TextDocument _doc;
        private readonly string _text;
        private readonly int _pos;
        public string Description => $"Insert '{_text}' at {_pos}";

        public InsertTextCommand(TextDocument doc, string text, int pos)
        { _doc = doc; _text = text; _pos = pos; }

        public void Execute() => _doc.Insert(_text, _pos);
        public void Undo()    => _doc.Remove(_pos, _text.Length);
    }

    internal class CommandHistory
    {
        private readonly Stack<ICommand_P> _history = new();

        public void Execute(ICommand_P command)
        {
            command.Execute();
            _history.Push(command);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"     ✓ {command.Description}");
            Console.ResetColor();
        }

        public void Undo()
        {
            if (_history.TryPop(out var cmd))
            {
                cmd.Undo();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"     ↩ Undone: {cmd.Description}");
                Console.ResetColor();
            }
        }
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch09_Patterns
    {
        public static void Run()
        {
            UI.Header("CHAPTER 9: DESIGN PATTERNS — PROVEN SOLUTIONS TO COMMON PROBLEMS");

            UI.Concept(
                "Design patterns are reusable solutions to commonly occurring problems in software design. " +
                "They're not code you copy — they're PROVEN BLUEPRINTS for solving particular types of problems.\n\n" +
                "The classic catalog has 23 patterns (Gang of Four, 1994). We'll cover the 6 most used in " +
                ".NET production code."
            );

            UI.Diagram("Three Pattern Categories",
                "  CREATIONAL: How objects are created",
                "     Singleton, Factory Method, Abstract Factory, Builder, Prototype",
                "",
                "  STRUCTURAL: How objects are assembled/composed",
                "     Adapter, Decorator, Facade, Proxy, Composite",
                "",
                "  BEHAVIORAL: How objects communicate and distribute responsibility",
                "     Observer, Strategy, Command, Iterator, State, Template Method"
            );

            UI.Pause();
            Pattern1_Singleton();
            Pattern2_Factory();
            Pattern3_Builder();
            Pattern4_Decorator();
            Pattern5_Observer();
            Pattern6_Strategy();
            Pattern7_Command();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern1_Singleton()
        {
            UI.SubHeader("Pattern 1: Singleton — Only One Instance");

            UI.Concept(
                "Singleton ensures a class has exactly ONE instance and provides a global access point to it. " +
                "Use for: application configuration, shared caches, connection pools, logging infrastructure."
            );

            UI.Code("Thread-safe Singleton with Lazy<T>",
                "public sealed class AppSettings",
                "{",
                "    // Lazy<T>: created only when first accessed, thread-safe by default",
                "    private static readonly Lazy<AppSettings> _instance =",
                "        new Lazy<AppSettings>(() => new AppSettings());",
                "",
                "    // The global access point:",
                "    public static AppSettings Instance => _instance.Value;",
                "",
                "    // PRIVATE constructor: 'new AppSettings()' is impossible from outside",
                "    private AppSettings() { MaxRetries = 3; ... }",
                "",
                "    public int    MaxRetries { get; }",
                "    public string ApiBaseUrl { get; }",
                "}"
            );

            UI.LiveDemo("Singleton — always the same instance", () =>
            {
                var s1 = AppSettings.Instance;
                var s2 = AppSettings.Instance;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     s1.MaxRetries: {s1.MaxRetries}");
                Console.WriteLine($"     s2.MaxRetries: {s2.MaxRetries}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"     Same instance? {object.ReferenceEquals(s1, s2)}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     They're the same object. Creating twice = no second allocation.");
                Console.ResetColor();
            });

            UI.Bad("Modern warning", "Static singletons make testing hard (you can't inject a fake). " +
                "In ASP.NET Core, prefer registering as 'services.AddSingleton<IAppSettings, AppSettings>()' " +
                "in the DI container. The framework manages the lifetime and you can still test.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern2_Factory()
        {
            UI.SubHeader("Pattern 2: Factory Method — Delegate Object Creation");

            UI.Concept(
                "Factory Method defines a method for creating objects, but lets subclasses (or a factory class) " +
                "decide which class to instantiate. The caller says 'give me a logger' — the factory " +
                "decides WHICH logger and HOW to create it."
            );

            UI.Code("Factory Method — hide creation complexity",
                "public interface ILoggerFactory { ILogger CreateLogger(string category); }",
                "",
                "public class ConsoleLoggerFactory : ILoggerFactory",
                "{",
                "    // Factory method: caller asks for ILogger, factory provides the concrete type",
                "    public ILogger CreateLogger(string category) => new ConsoleLogger(category);",
                "}",
                "",
                "// The caller never knows (or cares) what type of logger it gets:",
                "ILoggerFactory factory = new ConsoleLoggerFactory();  // swap in prod",
                "ILogger logger = factory.CreateLogger(\"OrderService\");",
                "logger.Log(\"INFO\", \"Order placed\");"
            );

            UI.LiveDemo("Factory Method creating loggers", () =>
            {
                ILoggerFactory_P factory = new ConsoleLoggerFactory();
                var orderLogger  = factory.CreateLogger("OrderService");
                var paymentLogger = factory.CreateLogger("PaymentService");

                orderLogger.Log("INFO",  "Order #1001 created");
                paymentLogger.Log("WARN", "Payment retry attempt 2");
                orderLogger.Log("ERROR", "Order #1001 item out of stock");
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern3_Builder()
        {
            UI.SubHeader("Pattern 3: Builder — Step-by-Step Complex Construction");

            UI.Concept(
                "Builder constructs complex objects step-by-step. It separates the construction " +
                "of a complex object from its representation. " +
                "Best for: objects with many optional parameters, fluent APIs, query construction."
            );

            UI.Code("Builder pattern — fluent query building",
                "var query = new SqlQuery.Builder()",
                "    .From(\"Orders\")",
                "    .Select(\"OrderId\", \"CustomerName\", \"Total\")",
                "    .Where(\"Status = 'Active'\")",
                "    .Where(\"Total > 100\")",
                "    .OrderBy(\"CreatedAt DESC\")",
                "    .Limit(50)",
                "    .Build();",
                "",
                "// vs the constructor-overload nightmare without Builder:",
                "// new SqlQuery(\"Orders\", new[]{\"OrderId\",\"CustomerName\",\"Total\"},",
                "//              new[]{\"Status='Active'\",\"Total>100\"}, \"CreatedAt DESC\", 50)",
                "// Which param does the 50 go to? Is condition order preserved? Unclear."
            );

            UI.LiveDemo("Builder constructing SQL queries", () =>
            {
                var q1 = new SqlQuery.Builder()
                    .From("Products")
                    .Select("Id", "Name", "Price")
                    .Where("Stock > 0")
                    .OrderBy("Price ASC")
                    .Limit(10)
                    .Build();

                var q2 = new SqlQuery.Builder()
                    .From("Orders")
                    .Where("Status = 'Pending'")
                    .Where("CreatedAt > '2025-01-01'")
                    .Build();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"     Query 1: {q1}");
                Console.WriteLine($"     Query 2: {q2}");
                Console.ResetColor();
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern4_Decorator()
        {
            UI.SubHeader("Pattern 4: Decorator — Wrap to Add Behavior");

            UI.Concept(
                "Decorator attaches additional responsibilities to an object dynamically, " +
                "without modifying the class. Decorators provide a flexible alternative to subclassing for extending functionality.\n\n" +
                "Real-world use: ASP.NET Core middleware IS the Decorator pattern. " +
                "Logging, authentication, caching wrappers."
            );

            UI.Code("Decorator — wrapping a text processor with layers",
                "ITextProcessor processor = new CensorDecorator(",
                "                           new UpperCaseDecorator(",
                "                           new TrimDecorator(",
                "                           new PlainTextProcessor())));",
                "",
                "// Each decorator wraps the next one.",
                "// When Process() is called, it chains through all layers:",
                "// 1. PlainText: returns input unchanged",
                "// 2. TrimDecorator: removes leading/trailing whitespace",
                "// 3. UpperCaseDecorator: converts to uppercase",
                "// 4. CensorDecorator: replaces bad words with ***",
                "",
                "// This is like middleware pipeline in ASP.NET Core.",
                "// Each 'decorator' is a middleware component that wraps the next."
            );

            UI.LiveDemo("Decorator layers transforming text", () =>
            {
                // Build a pipeline: Trim → UpperCase → Censor
                ITextProcessor pipeline = new CensorDecorator(
                                          new UpperCaseDecorator(
                                          new TrimDecorator(
                                          new PlainTextProcessor())));

                var inputs = new[]
                {
                    "   hello world   ",
                    "  this is bad content  ",
                    "  the ugly truth  ",
                };

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     Input                       → Output");
                Console.WriteLine("     " + new string('─', 50));
                Console.ResetColor();

                foreach (var input in inputs)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"     \"{input,-28}\"");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" → \"{pipeline.Process(input)}\"");
                }
                Console.ResetColor();
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern5_Observer()
        {
            UI.SubHeader("Pattern 5: Observer — Subscribe and Be Notified");

            UI.Concept(
                "Observer defines a one-to-many relationship: when one object changes state, " +
                "all its dependents are notified and updated automatically.\n\n" +
                "C# events and delegates ARE the Observer pattern. " +
                "Real-world: stock price alerts, UI data binding, event-driven microservices, " +
                "real-time notifications."
            );

            UI.Code("Observer — C# events as the observer mechanism",
                "public class StockTicker",
                "{",
                "    // Event declaration: observers subscribe to this",
                "    public event EventHandler<PriceChangedEventArgs> PriceChanged;",
                "",
                "    public void UpdatePrice(decimal newPrice)",
                "    {",
                "        var old = _price;",
                "        _price = newPrice;",
                "        // '?.' = only raise if anyone is subscribed (avoids NullReferenceException)",
                "        PriceChanged?.Invoke(this, new PriceChangedEventArgs(Symbol, old, newPrice));",
                "    }",
                "}",
                "",
                "// Observers subscribe with '+=', unsubscribe with '-=':",
                "var ticker = new StockTicker(\"AAPL\", 150m);",
                "ticker.PriceChanged += (sender, e) => Console.WriteLine($\"Alert: {e.Symbol} → {e.NewPrice}\");",
                "ticker.PriceChanged += (sender, e) => { if (e.Change < 0) Buy(e.Symbol); };",
                "ticker.UpdatePrice(145m);  // Both subscribers notified"
            );

            UI.LiveDemo("Observer pattern — stock ticker", () =>
            {
                var aapl = new StockTicker("AAPL", 150m);

                int alertCount = 0;

                // Subscriber 1: alert service
                aapl.PriceChanged += (s, e) =>
                {
                    alertCount++;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"     📊 ALERT: {e.Symbol} changed from ${e.OldPrice:F2} to ${e.NewPrice:F2} ");
                    Console.ForegroundColor = e.Change >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(e.Change >= 0 ? $"(+${e.Change:F2})" : $"(-${Math.Abs(e.Change):F2})");
                };

                // Subscriber 2: trading bot
                aapl.PriceChanged += (s, e) =>
                {
                    if (e.NewPrice < e.OldPrice * 0.97m) // 3% drop = buy signal
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"     🤖 BOT: Price dropped > 3% — BUY signal for {e.Symbol}");
                    }
                };

                Console.ResetColor();
                aapl.UpdatePrice(148m);
                aapl.UpdatePrice(145m);
                aapl.UpdatePrice(144m);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n     Total alerts raised: {alertCount}");
                Console.ResetColor();
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern6_Strategy()
        {
            UI.SubHeader("Pattern 6: Strategy — Swap Algorithms at Runtime");

            UI.Concept(
                "Strategy defines a family of algorithms, encapsulates each one, and makes them interchangeable. " +
                "Clients can switch the algorithm they use without changing the context code.\n\n" +
                "Real-world use: sorting strategies, payment processing strategies, " +
                "routing algorithms, compression algorithms, discount calculations."
            );

            UI.LiveDemo("Strategy pattern — swappable sorting", () =>
            {
                var data = new List<int> { 64, 25, 12, 22, 11 };
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     Original: [{string.Join(", ", data)}]");
                Console.ResetColor();

                ISortStrategy_P<int> strategy = new BubbleSort_P<int>();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"     Using {strategy.Name}: ");
                strategy.Sort(data);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{string.Join(", ", data)}]");

                data = new List<int> { 64, 25, 12, 22, 11 }; // reset
                strategy = new LinqSort_P<int>(); // SWAP the strategy at runtime
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"     Using {strategy.Name}: ");
                strategy.Sort(data);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{string.Join(", ", data)}]");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     Same calling code. Different algorithm. No if-else needed.");
                Console.ResetColor();
            });
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Pattern7_Command()
        {
            UI.SubHeader("Pattern 7: Command — Encapsulate Operations as Objects");

            UI.Concept(
                "Command encapsulates a request as an object. This lets you:\n" +
                "  • Queue operations for later execution\n" +
                "  • Support undo/redo\n" +
                "  • Log all operations\n" +
                "  • Retry failed operations\n\n" +
                "Real-world use: text editors (undo/redo), transaction logs, " +
                "job queues, macro recording."
            );

            UI.LiveDemo("Command pattern — text editor with undo", () =>
            {
                var doc     = new TextDocument();
                var history = new CommandHistory();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     Building document with commands:");
                Console.ResetColor();

                history.Execute(new InsertTextCommand(doc, "Hello",  0));
                history.Execute(new InsertTextCommand(doc, " World", 5));
                history.Execute(new InsertTextCommand(doc, "!",      11));

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n     Document: \"{doc.Content}\"");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Undoing last 2 operations:");
                Console.ResetColor();
                history.Undo();
                history.Undo();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n     Document after 2 undos: \"{doc.Content}\"");
                Console.ResetColor();
            });

            UI.Exercise(
                "PATTERN RECOGNITION EXERCISE:\n\n" +
                "For each scenario, identify which pattern is the best fit:\n\n" +
                "  1. A logging system where new log destinations (file, console, cloud) can be added\n" +
                "     without changing the Logger class itself.\n\n" +
                "  2. Building an HTTP request with optional headers, timeout, retries, body.\n\n" +
                "  3. A financial system that tracks every trade operation and can replay or undo them.\n\n" +
                "  4. A database connection pool that provides one shared pool to the entire app.\n\n" +
                "  5. A system that notifies multiple analytics services when a user signs up.",

                "ANSWERS:\n\n" +
                "  1. Strategy Pattern — ILogDestination interface, implementations for each destination.\n" +
                "     (Also: Decorator if you want to layer them)\n\n" +
                "  2. Builder Pattern — HttpRequestBuilder.WithHeader().WithTimeout().WithBody().Build()\n\n" +
                "  3. Command Pattern — each trade is an ICommand with Execute() and Undo()/Compensate()\n\n" +
                "  4. Singleton Pattern — one ConnectionPool instance shared by all code\n\n" +
                "  5. Observer Pattern — UserRegistered event, multiple analytics services subscribe"
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Design Patterns", new QuizQuestion[]
            {
                new(
                    "You need exactly ONE instance of a configuration class shared across the app. Which pattern?",
                    new[] { "Factory Method", "Singleton", "Builder", "Observer" },
                    1,
                    "Singleton ensures one instance with a global access point. For modern apps, manage it via DI container as a singleton lifetime."
                ),
                new(
                    "ASP.NET Core middleware (Logging, Auth, CORS wrapping the request pipeline) is which pattern?",
                    new[] { "Observer", "Command", "Decorator", "Strategy" },
                    2,
                    "Decorator: each middleware wraps the next one, adding behavior (logging, auth checking, etc.) without modifying the handlers themselves."
                ),
                new(
                    "A payment system needs to support Stripe, PayPal, or crypto — swappable at runtime. Which pattern?",
                    new[] { "Singleton", "Builder", "Strategy", "Command" },
                    2,
                    "Strategy: IPaymentStrategy interface with StripeStrategy, PayPalStrategy, CryptoStrategy implementations, swappable at runtime."
                ),
                new(
                    "You're building a text editor with undo/redo for every operation. Which pattern?",
                    new[] { "Observer", "Command", "Decorator", "Factory" },
                    1,
                    "Command: encapsulate each operation (Insert, Delete, Format) as an ICommand with Execute() and Undo(). Stack commands for undo history."
                ),
                new(
                    "Multiple UI components need to update when a data model changes. Which pattern?",
                    new[] { "Strategy", "Singleton", "Builder", "Observer" },
                    3,
                    "Observer: the data model is the subject. UI components are subscribers. When data changes, all subscribers (UI components) are notified automatically."
                ),
            });
        }
    }
}
