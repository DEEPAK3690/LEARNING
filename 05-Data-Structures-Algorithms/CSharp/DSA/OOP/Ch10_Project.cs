// ============================================================
// FILE: Ch10_Project.cs
// PURPOSE: Walk through a real-world Order Management System
//          explaining EVERY OOP decision, trade-off, and pattern.
// ============================================================

namespace DSA.OOP
{
    // ════════════════════════════════════════════════════════
    // DOMAIN LAYER — the heart of the system
    // Contains: Entities, Value Objects, Domain Services
    // Rule: NO dependencies on infrastructure (no EF Core, no HTTP)
    // ════════════════════════════════════════════════════════

    // VALUE OBJECT: immutable, no identity, compared by VALUE not reference
    // Why record? Records have structural equality + immutability + concise syntax
    internal record Money_OMS(decimal Amount, string Currency)
    {
        // Business rule: can't add different currencies
        public Money_OMS Add(Money_OMS other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Currency mismatch: {Currency} vs {other.Currency}");
            return this with { Amount = Amount + other.Amount };
        }
        public Money_OMS Multiply(int qty) => this with { Amount = Amount * qty };
        public override string ToString() => $"{Currency} {Amount:F2}";
    }

    internal record Address_OMS(string Street, string City, string PostalCode, string Country);

    // ENTITY: has an identity (Id), mutable state, enforces business rules
    internal class Product_OMS
    {
        public int      Id    { get; }
        public string   Name  { get; private set; }
        public Money_OMS Price { get; private set; }
        private int     _stock;
        public int      Stock => _stock;

        public Product_OMS(int id, string name, Money_OMS price, int initialStock)
        {
            if (id <= 0)    throw new ArgumentException("Invalid product ID");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required");
            if (price.Amount <= 0) throw new ArgumentException("Price must be positive");
            if (initialStock < 0) throw new ArgumentException("Stock cannot be negative");

            Id     = id;
            Name   = name;
            Price  = price;
            _stock = initialStock;
        }

        // Encapsulated business operations — external code can't directly modify _stock
        public void Reserve(int qty)
        {
            if (qty <= 0)    throw new ArgumentException("Quantity must be positive");
            if (qty > _stock) throw new InvalidOperationException($"Insufficient stock for {Name}. Available: {_stock}");
            _stock -= qty;
        }

        public void Release(int qty) => _stock += qty;
        public bool HasStock(int qty) => _stock >= qty;
    }

    // ORDER LINE ITEM: composition within Order
    internal class OrderItem_OMS
    {
        public int      ProductId   { get; }
        public string   ProductName { get; }
        public Money_OMS UnitPrice  { get; }
        public int      Quantity    { get; }
        public Money_OMS Subtotal   => UnitPrice.Multiply(Quantity);

        public OrderItem_OMS(int productId, string name, Money_OMS unitPrice, int qty)
        {
            if (qty <= 0) throw new ArgumentException("Quantity must be positive");
            ProductId   = productId;
            ProductName = name;
            UnitPrice   = unitPrice;
            Quantity    = qty;
        }
    }

    // ORDER STATUS: enum defining the state machine
    internal enum OrderStatus_OMS
    {
        Draft, Submitted, PaymentConfirmed, Processing, Shipped, Delivered, Cancelled
    }

    // ORDER AGGREGATE: the main domain entity
    // 'Aggregate' = a cluster of entities treated as one unit for data changes
    internal class Order_OMS
    {
        public int             OrderId     { get; }
        public int             CustomerId  { get; }
        public DateTime        CreatedAt   { get; }
        public OrderStatus_OMS Status      { get; private set; }
        public Address_OMS     ShipTo      { get; }

        private readonly List<OrderItem_OMS> _items = new();
        private readonly List<string>        _log   = new();

        // Read-only exposure of internal collection (Encapsulation!)
        public IReadOnlyList<OrderItem_OMS> Items => _items.AsReadOnly();

        // Computed properties — no stored state that can get out of sync
        public Money_OMS Subtotal => _items.Aggregate(
            new Money_OMS(0, "USD"), (sum, item) => sum.Add(item.Subtotal));
        public Money_OMS Discount { get; private set; } = new(0, "USD");
        public Money_OMS Total    => new Money_OMS(Subtotal.Amount - Discount.Amount, "USD");

        public Order_OMS(int orderId, int customerId, Address_OMS shipTo)
        {
            OrderId    = orderId;
            CustomerId = customerId;
            ShipTo     = shipTo ?? throw new ArgumentNullException(nameof(shipTo));
            Status     = OrderStatus_OMS.Draft;
            CreatedAt  = DateTime.UtcNow;
            AddLog("Order created");
        }

        // All state changes go through METHODS — never through public setters
        public void AddItem(Product_OMS product, int qty)
        {
            EnsureStatus(OrderStatus_OMS.Draft, "Cannot modify submitted order");
            if (!product.HasStock(qty)) throw new InvalidOperationException($"Insufficient stock for {product.Name}");

            product.Reserve(qty);  // Update inventory
            _items.Add(new OrderItem_OMS(product.Id, product.Name, product.Price, qty));
            AddLog($"Added {qty}x {product.Name}");
        }

        public void ApplyDiscount(Money_OMS discount)
        {
            if (discount.Amount < 0 || discount.Amount > Subtotal.Amount)
                throw new ArgumentException("Invalid discount amount");
            Discount = discount;
            AddLog($"Discount applied: {discount}");
        }

        public void Submit()
        {
            EnsureStatus(OrderStatus_OMS.Draft, "Order already submitted");
            if (!_items.Any()) throw new InvalidOperationException("Cannot submit empty order");
            TransitionTo(OrderStatus_OMS.Submitted);
        }

        public void ConfirmPayment()  => TransitionTo(OrderStatus_OMS.PaymentConfirmed);
        public void StartProcessing() => TransitionTo(OrderStatus_OMS.Processing);
        public void Ship(string tracking)
        {
            TransitionTo(OrderStatus_OMS.Shipped);
            AddLog($"Tracking: {tracking}");
        }
        public void Deliver() => TransitionTo(OrderStatus_OMS.Delivered);

        public void Cancel(string reason)
        {
            if (Status == OrderStatus_OMS.Shipped || Status == OrderStatus_OMS.Delivered)
                throw new InvalidOperationException("Cannot cancel shipped/delivered order");
            TransitionTo(OrderStatus_OMS.Cancelled);
            AddLog($"Cancelled: {reason}");
        }

        public IReadOnlyList<string> GetLog() => _log.AsReadOnly();

        private void TransitionTo(OrderStatus_OMS newStatus)
        {
            Status = newStatus;
            AddLog($"Status → {newStatus}");
        }

        private void EnsureStatus(OrderStatus_OMS required, string message)
        {
            if (Status != required) throw new InvalidOperationException(message);
        }

        private void AddLog(string msg) =>
            _log.Add($"[{DateTime.UtcNow:HH:mm:ss}] {msg}");
    }

    // ════════════════════════════════════════════════════════
    // APPLICATION LAYER — orchestrates domain objects
    // Contains: Services, DTOs, use-case handlers
    // ════════════════════════════════════════════════════════

    // INTERFACES (Dependency Inversion) — infrastructure implementations injected
    internal interface IOrderRepo_OMS     { void Save(Order_OMS o); Order_OMS Get(int id); }
    internal interface IProductRepo_OMS   { Product_OMS Get(int id); }
    internal interface INotifier_OMS      { void Notify(string to, string subject, string body); }
    internal interface ILogger_OMS        { void Log(string msg); void Error(string msg); }

    // APPLICATION SERVICE: orchestrates domain and infrastructure
    internal class OrderApplicationService
    {
        private readonly IOrderRepo_OMS   _orders;
        private readonly IProductRepo_OMS _products;
        private readonly INotifier_OMS    _notifier;
        private readonly ILogger_OMS      _logger;

        public OrderApplicationService(
            IOrderRepo_OMS orders, IProductRepo_OMS products,
            INotifier_OMS notifier, ILogger_OMS logger)
        {
            _orders   = orders;   _products = products;
            _notifier = notifier; _logger   = logger;
        }

        public Order_OMS CreateOrder(int customerId, Address_OMS address)
        {
            var order = new Order_OMS(GenerateId(), customerId, address);
            _orders.Save(order);
            _logger.Log($"Order {order.OrderId} created for customer {customerId}");
            return order;
        }

        public void AddItem(int orderId, int productId, int qty)
        {
            var order   = _orders.Get(orderId) ?? throw new KeyNotFoundException($"Order {orderId} not found");
            var product = _products.Get(productId) ?? throw new KeyNotFoundException($"Product {productId} not found");
            order.AddItem(product, qty);
            _orders.Save(order);
        }

        public void SubmitOrder(int orderId, string customerEmail)
        {
            var order = _orders.Get(orderId) ?? throw new KeyNotFoundException();
            order.Submit();
            _orders.Save(order);
            _notifier.Notify(customerEmail, "Order Received",
                $"Order #{order.OrderId} total: {order.Total}. We'll confirm payment shortly.");
            _logger.Log($"Order {orderId} submitted. Total: {order.Total}");
        }

        private static int _nextId = 1000;
        private static int GenerateId() => ++_nextId;
    }

    // ════════════════════════════════════════════════════════
    // INFRASTRUCTURE LAYER — concrete implementations
    // Implements interfaces defined in Application layer
    // ════════════════════════════════════════════════════════

    internal class InMemoryOrderRepo : IOrderRepo_OMS
    {
        private readonly Dictionary<int, Order_OMS> _store = new();
        public void      Save(Order_OMS o)  => _store[o.OrderId] = o;
        public Order_OMS Get(int id)        => _store.TryGetValue(id, out var o) ? o : null;
    }

    internal class InMemoryProductRepo : IProductRepo_OMS
    {
        private readonly Dictionary<int, Product_OMS> _catalog;
        public InMemoryProductRepo()
        {
            _catalog = new()
            {
                [101] = new Product_OMS(101, "USB-C Cable 3m",   new Money_OMS(19.99m, "USD"), 50),
                [102] = new Product_OMS(102, "HDMI Adapter",     new Money_OMS(24.99m, "USD"), 30),
                [103] = new Product_OMS(103, "USB Hub 7-Port",   new Money_OMS(39.99m, "USD"), 20),
            };
        }
        public Product_OMS Get(int id) => _catalog.TryGetValue(id, out var p) ? p : null;
    }

    internal class ConsoleNotifier_OMS : INotifier_OMS
    {
        public void Notify(string to, string subject, string body)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"     📧 To: {to}");
            Console.WriteLine($"     📋 Subject: {subject}");
            Console.WriteLine($"     📝 Body: {body}");
            Console.ResetColor();
        }
    }

    internal class ConsoleLogger_OMS : ILogger_OMS
    {
        public void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"     [LOG] {msg}");
            Console.ResetColor();
        }
        public void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"     [ERR] {msg}");
            Console.ResetColor();
        }
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch10_Project
    {
        public static void Run()
        {
            UI.Header("CHAPTER 10: REAL-WORLD PROJECT — ORDER MANAGEMENT SYSTEM");

            UI.Concept(
                "Now we bring EVERY concept together into a real system. " +
                "We'll build an Order Management System (OMS) and explain EVERY decision:\n" +
                "  Why this class? Why this interface? Why this pattern?\n" +
                "  What are the alternatives? What are the trade-offs?"
            );
            UI.Pause();

            Section1_Architecture();
            Section2_DomainLayer();
            Section3_ApplicationLayer();
            Section4_InfrastructureLayer();
            Section5_LiveDemo();
            Section6_DesignDecisions();
            Section7_ScalingConsiderations();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_Architecture()
        {
            UI.SubHeader("System Architecture — The Layered Approach");

            UI.Diagram("Three-Layer Architecture",
                "  ┌─────────────────────────────────────────────────────────────────┐",
                "  │  DOMAIN LAYER (innermost — no external dependencies)           │",
                "  │  • Entities: Order, Product, Customer                          │",
                "  │  • Value Objects: Money, Address                               │",
                "  │  • Business Rules: invariants, state machines                  │",
                "  └──────────────────────────┬──────────────────────────────────────┘",
                "                             │ used by",
                "  ┌──────────────────────────▼──────────────────────────────────────┐",
                "  │  APPLICATION LAYER (orchestration)                              │",
                "  │  • Services: OrderService, ProductService                      │",
                "  │  • Defines interfaces for infrastructure                       │",
                "  │  • Coordinates domain objects + infrastructure                 │",
                "  └──────────────────────────┬──────────────────────────────────────┘",
                "                             │ depends on implementations of",
                "  ┌──────────────────────────▼──────────────────────────────────────┐",
                "  │  INFRASTRUCTURE LAYER (outermost — knows about external world)  │",
                "  │  • Repositories: SqlOrderRepo, MongoProductRepo                │",
                "  │  • Notifiers: SmtpEmailNotifier, SmsNotifier                  │",
                "  │  • Loggers: SerilogLogger, AppInsightsLogger                  │",
                "  └─────────────────────────────────────────────────────────────────┘"
            );

            UI.Concept(
                "WHY THIS ARCHITECTURE?\n\n" +
                "  1. TESTABILITY: Domain logic has no dependencies. Test it with zero mocks.\n" +
                "  2. FLEXIBILITY: Swap SQL for MongoDB? Change only the infrastructure layer.\n" +
                "  3. MAINTAINABILITY: Business rules live in ONE place (Domain). Not scattered.\n" +
                "  4. UNDERSTANDABILITY: Each layer has a clear, single responsibility.\n\n" +
                "This is called 'Clean Architecture' or 'Onion Architecture'. " +
                "Dependencies always point INWARD — toward the Domain."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_DomainLayer()
        {
            UI.SubHeader("Domain Layer — The Business Rules Live Here");

            UI.Print("OOP DECISIONS MADE IN THE DOMAIN LAYER:\n");

            UI.Code("Decision 1: Money as a Value Object (not decimal)",
                "// ❌ BAD: Using decimal for money",
                "decimal price = 19.99;",
                "decimal total = price * 2;  // no currency, no validation",
                "// Can you add USD to EUR? Nothing stops you. Bug waiting to happen.",
                "",
                "// ✓ GOOD: Money as a Value Object",
                "public record Money(decimal Amount, string Currency)",
                "{",
                "    public Money Add(Money other)  // Can't add USD to EUR",
                "    {",
                "        if (Currency != other.Currency) throw new InvalidOperationException();",
                "        return this with { Amount = Amount + other.Amount };",
                "    }",
                "}",
                "// Now the compiler enforces currency consistency.",
                "// record = immutable + structural equality. Perfect for value objects."
            );

            UI.Code("Decision 2: Order as an Aggregate (not just a data class)",
                "// ❌ BAD: Anemic domain model (data bags with no behavior)",
                "public class Order",
                "{",
                "    public int Status { get; set; }    // anyone can set anything",
                "    public List<OrderItem> Items { get; set; } // public mutable list",
                "    public decimal Total { get; set; } // can be wrong if not updated",
                "}",
                "// Service layer then has ALL the business logic — scattered, untestable.",
                "",
                "// ✓ GOOD: Rich domain model — Order enforces its own rules",
                "public class Order",
                "{",
                "    private OrderStatus _status = Draft;  // private!",
                "    private List<OrderItem> _items = new(); // private!",
                "    public Money Total => _items.Aggregate(...); // computed, always correct",
                "",
                "    public void Submit()  // Business rule: can only submit non-empty Draft",
                "    {",
                "        if (_status != Draft) throw ...;",
                "        if (!_items.Any()) throw ...;",
                "        _status = Submitted;",
                "    }",
                "}"
            );

            UI.Code("Decision 3: Status as an Enum State Machine",
                "// Status is a finite state machine — only valid transitions allowed",
                "// Draft → Submitted → PaymentConfirmed → Processing → Shipped → Delivered",
                "//              ↓                             ↓",
                "//          Cancelled                    Cancelled",
                "",
                "public void Ship(string tracking)",
                "{",
                "    // Only Processing orders can be shipped",
                "    if (Status != OrderStatus.Processing)",
                "        throw new InvalidOperationException(\"Can only ship Processing orders\");",
                "    Status = OrderStatus.Shipped;",
                "    _log.Add($\"Tracking: {tracking}\");",
                "}"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_ApplicationLayer()
        {
            UI.SubHeader("Application Layer — Orchestration Without Business Logic");

            UI.Concept(
                "The Application Service's job: coordinate domain objects and infrastructure. " +
                "It should NOT contain business logic (that's in the Domain). " +
                "It should NOT contain infrastructure details (that's in Infrastructure). " +
                "It's the conductor — it tells the musicians when to play, but doesn't play itself."
            );

            UI.Code("OrderApplicationService — pure orchestration",
                "public class OrderApplicationService",
                "{",
                "    // ALL dependencies are interfaces (DIP)",
                "    // ALL injected from outside (Dependency Injection)",
                "    private readonly IOrderRepository    _orders;",
                "    private readonly IProductRepository  _products;",
                "    private readonly IEmailNotifier      _notifier;",
                "    private readonly ILogger             _logger;",
                "",
                "    // Constructor injection: who provides these? The DI container.",
                "    public OrderApplicationService(/* 4 interface args */) { ... }",
                "",
                "    public void SubmitOrder(int orderId, string customerEmail)",
                "    {",
                "        // 1. Get the order (repository — infrastructure)",
                "        var order = _orders.Get(orderId);",
                "",
                "        // 2. Tell the ORDER to submit itself (domain — business rule!)",
                "        order.Submit();  // Order validates its own rules",
                "",
                "        // 3. Persist the change (repository — infrastructure)",
                "        _orders.Save(order);",
                "",
                "        // 4. Send notification (notifier — infrastructure)",
                "        _notifier.Notify(customerEmail, \"Order Received\", $\"Total: {order.Total}\");",
                "",
                "        // 5. Log the event (logger — infrastructure)",
                "        _logger.Log($\"Order {orderId} submitted\");",
                "    }",
                "    // Notice: NO business logic here. The ORDER decides if it can be submitted.",
                "    // Notice: NO SQL here. The REPOSITORY handles storage.",
                "}"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_InfrastructureLayer()
        {
            UI.SubHeader("Infrastructure Layer — Implementing the Contracts");

            UI.Concept(
                "The Infrastructure layer contains CONCRETE IMPLEMENTATIONS of the interfaces " +
                "defined in the Application layer. This is where SQL queries, HTTP calls, " +
                "file I/O, email clients, and message queues live.\n\n" +
                "The rest of the application never imports these directly — they're injected."
            );

            UI.Code("InMemoryOrderRepo implementing IOrderRepo",
                "// For tests and development: fast, no database needed",
                "public class InMemoryOrderRepository : IOrderRepository",
                "{",
                "    private readonly Dictionary<int, Order> _store = new();",
                "    public void  Save(Order o)  => _store[o.OrderId] = o;",
                "    public Order Get(int id)    => _store.TryGetValue(id, out var o) ? o : null;",
                "}",
                "",
                "// For production: EF Core + SQL Server",
                "public class SqlOrderRepository : IOrderRepository",
                "{",
                "    private readonly AppDbContext _db;",
                "    public SqlOrderRepository(AppDbContext db) => _db = db;",
                "    public void  Save(Order o) { _db.Orders.Update(o); _db.SaveChanges(); }",
                "    public Order Get(int id)   => _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);",
                "}",
                "",
                "// Switching from InMemory to SQL: ONE LINE in Program.cs DI registration.",
                "// Zero changes in OrderApplicationService or domain."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_LiveDemo()
        {
            UI.SubHeader("Live Demo — The Full Order Management System Running");

            UI.LiveDemo("Complete order lifecycle from creation to submission", () =>
            {
                // ── COMPOSITION ROOT: wire everything together ──────────
                // In production: ASP.NET Core DI container does this automatically
                IOrderRepo_OMS    orderRepo   = new InMemoryOrderRepo();
                IProductRepo_OMS  productRepo = new InMemoryProductRepo();
                INotifier_OMS     notifier    = new ConsoleNotifier_OMS();
                ILogger_OMS       logger      = new ConsoleLogger_OMS();

                var service = new OrderApplicationService(orderRepo, productRepo, notifier, logger);

                var address = new Address_OMS("123 Main St", "San Francisco", "94105", "USA");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("     ── Step 1: Create Order ──");
                Console.ResetColor();
                var order = service.CreateOrder(customerId: 42, address);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n     ── Step 2: Add Items ──");
                Console.ResetColor();
                service.AddItem(order.OrderId, productId: 101, qty: 2);
                service.AddItem(order.OrderId, productId: 102, qty: 1);

                // Reload from repo to see current state
                order = orderRepo.Get(order.OrderId);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     Order #{order.OrderId} — Status: {order.Status}");
                foreach (var item in order.Items)
                    Console.WriteLine($"     • {item.Quantity}x {item.ProductName} @ {item.UnitPrice} = {item.Subtotal}");
                Console.WriteLine($"     Subtotal: {order.Subtotal}");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n     ── Step 3: Apply Discount ──");
                Console.ResetColor();
                order.ApplyDiscount(new Money_OMS(5.00m, "USD"));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     Discount: {order.Discount}  →  Total: {order.Total}");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n     ── Step 4: Submit Order ──");
                Console.ResetColor();
                service.SubmitOrder(order.OrderId, "deepak@example.com");

                order = orderRepo.Get(order.OrderId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n     ✓ Final Status: {order.Status}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Order Audit Log:");
                foreach (var entry in order.GetLog())
                    Console.WriteLine($"     {entry}");
                Console.ResetColor();
            });
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_DesignDecisions()
        {
            UI.SubHeader("Every Design Decision Explained");

            UI.Print("DECISION TABLE — Why we made each OOP choice:\n");

            var decisions = new[]
            {
                ("Money as record (not decimal)",
                 "Encapsulates currency + amount together. Prevents adding USD to EUR. " +
                 "Immutable (record) — Money can't be accidentally mutated."),

                ("Order as rich domain model",
                 "Business rules (can't submit empty order, status transitions) live IN the Order. " +
                 "Not scattered across service classes. Makes rules testable in isolation."),

                ("IOrderRepository interface",
                 "Application Service never knows if storage is SQL, MongoDB, or in-memory. " +
                 "DIP: high-level doesn't depend on low-level. Swap storage with zero service changes."),

                ("Private _items list, public IReadOnlyList",
                 "Caller can read items but CANNOT add/remove. " +
                 "AddItem() enforces all business rules (stock check, status check). Encapsulation."),

                ("Enum for OrderStatus",
                 "Status transitions are explicit. Business logic enforces valid transitions. " +
                 "Enum gives you type safety — can't set Status = 999 accidentally."),

                ("Constructor injection for all dependencies",
                 "Makes every dependency explicit and visible. Easy to test: pass test doubles in. " +
                 "Class requirements are self-documenting through constructor parameters."),
            };

            foreach (var (decision, reason) in decisions)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  ◆ {decision}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"    {reason}");
            }
            Console.ResetColor();
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section7_ScalingConsiderations()
        {
            UI.SubHeader("How This Design Scales in Production");

            UI.Concept(
                "The OOP design we've built makes certain scaling paths easy. " +
                "Here's what changes and what stays the same:"
            );

            var scenarios = new[]
            {
                ("Scale: More payment methods",
                 "Add IPaymentProcessor implementations. Zero changes to OrderService."),

                ("Scale: Multiple notification channels",
                 "Use Composite pattern: CompositeNotifier wraps Email + SMS + Push. One line change."),

                ("Scale: High order volume",
                 "Submit order → publish to message queue → background workers process. " +
                 "OrderService.SubmitOrder publishes IOrderEvent. Zero domain code changes."),

                ("Scale: Switch database",
                 "Write new SqlOrderRepo or MongoOrderRepo. Register in DI. Done. " +
                 "Domain, Application, and all other Infrastructure classes unchanged."),

                ("Scale: Microservices",
                 "Each layer becomes a separate service. Domain objects become shared DTOs. " +
                 "Repository calls become HTTP calls. Interfaces still the contract."),
            };

            foreach (var (scenario, solution) in scenarios)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n  ▸ {scenario}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"    {solution}");
            }
            Console.ResetColor();

            UI.KeyPoint("Good OOP design doesn't just work today — it accommodates tomorrow's changes without rewrites.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Real-World OOP Architecture", new QuizQuestion[]
            {
                new(
                    "In Clean/Layered Architecture, which layer should contain business rules?",
                    new[] {
                        "Infrastructure layer — closest to the database",
                        "Application layer — it's the coordinator",
                        "Domain layer — business rules belong with the domain entities",
                        "All layers share business rules equally"
                    },
                    2,
                    "Domain layer owns business rules. This makes them testable without any infrastructure. Application layer orchestrates; Infrastructure layer implements."
                ),
                new(
                    "Why is Money modeled as a Value Object (record) rather than a decimal?",
                    new[] {
                        "Records are faster than decimals at runtime",
                        "To bundle Amount + Currency together and prevent mixing incompatible currencies",
                        "Because decimal can't represent monetary values accurately",
                        "Records are required by the ORM for database mapping"
                    },
                    1,
                    "Money encapsulates currency + amount. You can't accidentally add USD to EUR. Immutability prevents accidental mutation. This is Domain-Driven Design (DDD)."
                ),
                new(
                    "What's the key advantage of the 'anemic' vs 'rich' domain model distinction?",
                    new[] {
                        "Rich models use less memory than anemic models",
                        "Anemic models are easier to serialize to JSON",
                        "Rich models keep business rules inside domain entities, making them testable in isolation without infrastructure",
                        "Rich models automatically generate database schemas"
                    },
                    2,
                    "Rich domain models enforce their own rules. Order.Submit() validates its own preconditions. Test the domain with zero mocks, no database needed."
                ),
                new(
                    "When adding a new payment processor (e.g., Apple Pay), what should change?",
                    new[] {
                        "Add if/else in OrderService.ProcessPayment()",
                        "Modify the IPaymentProcessor interface to add ApplePay-specific methods",
                        "Create a new ApplePayProcessor implementing IPaymentProcessor. Register in DI. Done.",
                        "Rewrite the entire payment handling flow"
                    },
                    2,
                    "OCP + DIP: new processor = new class implementing existing interface. Zero changes to OrderService or other processors. This is extensibility through interfaces."
                ),
            });
        }
    }
}
