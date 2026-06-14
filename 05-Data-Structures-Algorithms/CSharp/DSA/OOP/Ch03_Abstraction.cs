// ============================================================
// FILE: Ch03_Abstraction.cs
// PURPOSE: Teach Abstraction — defining WHAT without HOW,
//          hiding complexity behind simple interfaces.
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO CLASSES ─────────────────────────────────────────────

    // ABSTRACT CLASS: partially implemented, cannot be instantiated.
    // It defines WHAT every shape must be able to do,
    // but EACH SHAPE decides HOW it implements it.
    internal abstract class Shape
    {
        // Auto-property: concrete (implemented in the base class, shared by all shapes)
        public string Color { get; set; } = "Black";

        // ABSTRACT methods: no body here — each subclass MUST provide its own implementation.
        // 'abstract' forces subclasses to implement — if they don't, it's a compile error.
        public abstract double Area();
        public abstract double Perimeter();
        public abstract string ShapeName();

        // CONCRETE method: shared behavior all shapes use without needing to override.
        // Subclasses get this for FREE — they inherit it directly.
        public void Describe()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"     {Color} {ShapeName()}: Area = {Area():F2}, Perimeter = {Perimeter():F2}");
            Console.ResetColor();
        }
    }

    // Each subclass provides its OWN implementation of the abstract methods.
    internal class Circle : Shape
    {
        private double _radius;
        public Circle(double radius, string color = "Red")
        {
            if (radius <= 0) throw new ArgumentException("Radius must be positive");
            _radius = radius;
            Color   = color;
        }
        public override double Area()      => Math.PI * _radius * _radius;
        public override double Perimeter() => 2 * Math.PI * _radius;
        public override string ShapeName() => $"Circle (r={_radius})";
    }

    internal class Rectangle : Shape
    {
        private double _width, _height;
        public Rectangle(double w, double h, string color = "Blue")
        {
            if (w <= 0 || h <= 0) throw new ArgumentException("Dimensions must be positive");
            _width = w; _height = h; Color = color;
        }
        public override double Area()      => _width * _height;
        public override double Perimeter() => 2 * (_width + _height);
        public override string ShapeName() => $"Rectangle ({_width}x{_height})";
    }

    internal class Triangle : Shape
    {
        private double _a, _b, _c; // three sides
        public Triangle(double a, double b, double c, string color = "Green")
        {
            if (a + b <= c || a + c <= b || b + c <= a)
                throw new ArgumentException("Invalid triangle: sides violate triangle inequality");
            _a = a; _b = b; _c = c; Color = color;
        }
        public override double Area()
        {
            // Heron's formula — you don't need to know this, just that Shape doesn't know it either
            double s = (_a + _b + _c) / 2;
            return Math.Sqrt(s * (s - _a) * (s - _b) * (s - _c));
        }
        public override double Perimeter() => _a + _b + _c;
        public override string ShapeName() => $"Triangle ({_a},{_b},{_c})";
    }

    // INTERFACE: pure contract — no fields, no constructors, no implementation (mostly).
    // Defines a capability, not an identity.
    internal interface ISaveable
    {
        string Serialize();              // Must be implemented by any class that uses this interface
        void Deserialize(string data);
    }

    internal interface IPrintable
    {
        void Print();
    }

    // A class can implement MULTIPLE interfaces (but can only inherit ONE class)
    internal class Report : ISaveable, IPrintable
    {
        public string Title   { get; set; }
        public string Content { get; set; }

        public string Serialize()         => $"{Title}||{Content}";
        public void   Deserialize(string d){ var p = d.Split("||"); Title = p[0]; Content = p[1]; }
        public void   Print()             => Console.WriteLine($"     === {Title} ===\n     {Content}");
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch03_Abstraction
    {
        public static void Run()
        {
            UI.Header("CHAPTER 3: ABSTRACTION — SHOW THE WHEEL, HIDE THE ENGINE");

            UI.Concept(
                "Abstraction means: expose WHAT something does, hide HOW it does it. " +
                "You interact with a simplified model of a complex system. " +
                "The complexity is still there — but it's tucked away, out of sight."
            );
            UI.Pause();

            Section1_ConceptAndAnalogy();
            Section2_AbstractionVsEncapsulation();
            Section3_AbstractClasses();
            Section4_Interfaces();
            Section5_AbstractClassVsInterface();
            Section6_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_ConceptAndAnalogy()
        {
            UI.SubHeader("What Is Abstraction and Why Does It Exist?");

            UI.Analogy("Driving a Car",
                "When you drive a car, you interact with: steering wheel, pedals, gear shift. " +
                "You do NOT interact with: fuel injection timing, ABS brake calculations, " +
                "transmission gear ratios, engine combustion chamber pressure. " +
                "All that complexity is ABSTRACTED AWAY behind a simple interface. " +
                "If you had to understand all of that to drive, almost no one would. " +
                "Abstraction makes complex systems usable by hiding their inner workings."
            );

            UI.Diagram("Abstraction Layers",
                "  YOU (the caller/user):              press 'Send Email' button",
                "  ┌──────────────────────────────────────────────────────────┐",
                "  │   EmailService.Send(to, subject, body)  ← your interface │  Layer 1",
                "  └──────────────────────────────────────────────────────────┘",
                "  ┌──────────────────────────────────────────────────────────┐",
                "  │   SMTP client: EHLO, AUTH, MAIL FROM, RCPT TO, DATA...  │  Layer 2",
                "  └──────────────────────────────────────────────────────────┘",
                "  ┌──────────────────────────────────────────────────────────┐",
                "  │   TCP/IP packets, DNS resolution, TLS handshake...       │  Layer 3",
                "  └──────────────────────────────────────────────────────────┘",
                "  ┌──────────────────────────────────────────────────────────┐",
                "  │   Electrical signals over fiber/copper/wireless           │  Layer 4",
                "  └──────────────────────────────────────────────────────────┘",
                "  Each layer only knows about the ONE layer above and below it.",
                "  Change layer 3 (use a different email protocol)? Layers 1 and 4 don't care."
            );

            UI.Print("WHY ABSTRACTION IS ESSENTIAL:\n");
            var reasons = new[]
            {
                ("Complexity management", "You can't hold the entire system in your head at once. Abstraction lets you work on one layer at a time."),
                ("Reduced coupling",      "Callers depend on the INTERFACE (what it does), not the IMPLEMENTATION (how). Swap implementations freely."),
                ("Team productivity",     "One team builds the interface. Another builds the implementation. Both work in parallel."),
                ("Testability",           "You can swap real implementations with fakes in tests. Test the caller's behavior, not the dependency."),
                ("Extensibility",         "Add new implementations without touching existing code. This is the Open/Closed Principle."),
            };

            foreach (var (reason, desc) in reasons)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n  ◆ {reason}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"    {desc}");
            }
            Console.ResetColor();
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_AbstractionVsEncapsulation()
        {
            UI.SubHeader("Abstraction vs Encapsulation — The Confusion Cleared Up");

            UI.Concept(
                "Students often confuse these two. They're related but different:\n\n" +
                "  ENCAPSULATION = HOW you hide internal data (private fields, properties)\n" +
                "  ABSTRACTION   = WHAT you expose in your public interface (design decision)\n\n" +
                "Encapsulation is the MECHANISM. Abstraction is the DESIGN PRINCIPLE.\n" +
                "You USE encapsulation to ACHIEVE abstraction."
            );

            UI.Diagram("The Relationship",
                "  ABSTRACTION (design goal):  'Show only what's needed. Hide the rest.'",
                "       ↓  implemented via",
                "  ENCAPSULATION (mechanism):  Private fields + Public methods/properties",
                "       ↓  result is",
                "  CLEAN PUBLIC INTERFACE:     Callers only see what they need. Details hidden."
            );

            UI.Code("Concrete example of the difference",
                "public class EmailService",
                "{",
                "    // ENCAPSULATION: these private fields are HIDDEN from callers",
                "    private SmtpClient _smtpClient;      // internal detail",
                "    private string     _serverAddress;    // internal detail",
                "    private int        _port;             // internal detail",
                "    private string     _apiKey;           // internal detail",
                "",
                "    // ABSTRACTION: this is the SIMPLE PUBLIC INTERFACE callers see",
                "    // Callers don't know about SMTP, ports, API keys, or TLS.",
                "    // They just call Send() with human-readable parameters.",
                "    public void Send(string to, string subject, string body)",
                "    {",
                "        // All the SMTP complexity hidden in here",
                "    }",
                "}",
                "",
                "// The CALLER experiences abstraction:",
                "emailService.Send(\"deepak@example.com\", \"Hello\", \"Body text\");",
                "// No SMTP knowledge required. The complexity is abstracted away."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_AbstractClasses()
        {
            UI.SubHeader("Abstract Classes — Partial Abstraction with Shared Code");

            UI.Concept(
                "An ABSTRACT CLASS is a class you can't instantiate (no 'new AbstractClass()'). " +
                "It exists to be inherited from. It can have:\n" +
                "  • ABSTRACT methods: declared but not implemented — subclasses MUST implement them\n" +
                "  • CONCRETE methods: fully implemented — subclasses inherit them for free\n" +
                "  • Fields, properties, constructors — everything a regular class can have"
            );

            UI.Code("Abstract class anatomy — the Shape hierarchy",
                "// 'abstract' keyword = cannot create 'new Shape()' directly.",
                "// You can only create 'new Circle()' or 'new Rectangle()' etc.",
                "public abstract class Shape",
                "{",
                "    // CONCRETE property: every shape HAS a color. Shared code.",
                "    public string Color { get; set; } = \"Black\";",
                "",
                "    // ABSTRACT methods: every shape MUST have Area and Perimeter,",
                "    // but HOW they're calculated is DIFFERENT for each shape.",
                "    // No body — the {} would be an error here.",
                "    public abstract double Area();",
                "    public abstract double Perimeter();",
                "    public abstract string ShapeName();",
                "",
                "    // CONCRETE method: every shape gets this for free.",
                "    // It USES the abstract methods — polymorphism in action.",
                "    // At runtime: the correct subclass's Area() and Perimeter() are called.",
                "    public void Describe()",
                "    {",
                "        Console.WriteLine($\"{Color} {ShapeName()}: Area={Area():F2}\");",
                "    }",
                "}",
                "",
                "// This WON'T compile:",
                "// var s = new Shape();  // ERROR: Cannot create instance of abstract class",
                "",
                "// These work fine:",
                "var c = new Circle(5);       // Creates a Circle",
                "var r = new Rectangle(4, 6); // Creates a Rectangle",
                "",
                "// But you can USE them through the abstract type (polymorphism!):",
                "Shape s = new Circle(5);  // 'Shape' type, but 'Circle' object",
                "s.Describe();             // Calls Circle's Area() and Perimeter()"
            );

            UI.Diagram("Abstract Class Hierarchy",
                "         Shape (abstract — can't instantiate)",
                "        /    |    \\",
                "       ↓     ↓     ↓",
                "    Circle  Rect  Triangle   ← these CAN be instantiated",
                "    Area()  Area()  Area()   ← each has own implementation",
                "",
                "  Shape says: 'All shapes must have Area() and Perimeter().'",
                "  Each subclass answers: 'Here's how MY shape computes them.'"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_Interfaces()
        {
            UI.SubHeader("Interfaces — Pure Abstraction, Zero Implementation");

            UI.Concept(
                "An INTERFACE is a 100% abstract contract. It defines a set of methods (and properties) " +
                "that implementing classes MUST provide. An interface has:\n" +
                "  • No fields (except constants)\n" +
                "  • No constructors\n" +
                "  • No implementation (except optional default methods in C# 8+)\n\n" +
                "Think of an interface as a LEGAL CONTRACT: 'If you want to be called a Logger, " +
                "you MUST provide a Log() method. What you do inside is your business.'"
            );

            UI.Code("Interface — defining a capability contract",
                "// Naming convention: 'I' prefix (ILogger, IRepository, IPrintable)",
                "// This tells readers: 'this is an interface'",
                "public interface ILogger",
                "{",
                "    // These are METHOD SIGNATURES — no body, no implementation.",
                "    // Any class claiming to be an ILogger MUST implement these.",
                "    void Log(string message);",
                "    void LogError(string message, Exception ex);",
                "    void LogWarning(string message);",
                "}",
                "",
                "// IMPLEMENTATION 1: Writes to console",
                "public class ConsoleLogger : ILogger",
                "{",
                "    public void Log(string msg)        => Console.WriteLine($\"[INFO] {msg}\");",
                "    public void LogError(string m, Exception e) => Console.Error.WriteLine($\"[ERR] {m}: {e.Message}\");",
                "    public void LogWarning(string msg) => Console.WriteLine($\"[WARN] {msg}\");",
                "}",
                "",
                "// IMPLEMENTATION 2: Writes to a file",
                "public class FileLogger : ILogger",
                "{",
                "    private string _path;",
                "    public FileLogger(string path) => _path = path;",
                "    public void Log(string msg)        => File.AppendAllText(_path, $\"[INFO] {msg}\\n\");",
                "    public void LogError(string m, Exception e) => File.AppendAllText(_path, $\"[ERR] {m}\\n\");",
                "    public void LogWarning(string msg) => File.AppendAllText(_path, $\"[WARN] {msg}\\n\");",
                "}",
                "",
                "// THE POWER: callers depend on ILogger, not Console/File specifics.",
                "// Swap logger without changing ANY caller code.",
                "public class OrderService",
                "{",
                "    private ILogger _logger;",
                "    public OrderService(ILogger logger) => _logger = logger;",
                "    public void PlaceOrder() => _logger.Log(\"Order placed\");",
                "}"
            );

            UI.Code("Multiple interface implementation — a class can wear many hats",
                "// A class can implement as many interfaces as needed.",
                "// This gives C# a form of 'multiple inheritance' for contracts.",
                "public class SmartReport : ISaveable, IPrintable, IComparable<SmartReport>",
                "{",
                "    public string Title { get; }",
                "    public DateTime Created { get; }",
                "",
                "    // From ISaveable:",
                "    public string Serialize()         => $\"{Title}|{Created:O}\";",
                "    public void   Deserialize(string d) { /* ... */ }",
                "",
                "    // From IPrintable:",
                "    public void Print() => Console.WriteLine($\"Report: {Title}\");",
                "",
                "    // From IComparable<SmartReport>:",
                "    public int CompareTo(SmartReport other) => Created.CompareTo(other.Created);",
                "}"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_AbstractClassVsInterface()
        {
            UI.SubHeader("Abstract Class vs Interface — When to Use Which");

            UI.Concept(
                "This is one of the most-asked OOP interview questions. Here's the definitive guide:"
            );

            UI.Diagram("Decision Guide",
                "  Should your types share CODE (not just a contract)?",
                "    YES → Use Abstract Class",
                "    NO  → Use Interface",
                "",
                "  Is the relationship IS-A (true specialization)?",
                "    YES → Consider Abstract Class (Dog IS-A Animal)",
                "    NO  → Use Interface (Robot CAN-DO IWorkable)",
                "",
                "  Does the implementing class ALREADY have a base class?",
                "    YES → Must use Interface (C# has single inheritance)",
                "    NO  → Either can work",
                "",
                "  Are you defining a ROLE or CAPABILITY, not an identity?",
                "    YES → Interface (ILoggable, ISaveable, IDisposable)",
                "    NO  → Abstract Class (Vehicle, Animal, Shape)"
            );

            var comparisons = new[]
            {
                ("Fields",            "✓ Yes (private, protected)",     "✗ No (only constants)"),
                ("Constructors",      "✓ Yes",                          "✗ No"),
                ("Implementation",    "✓ Partial (concrete + abstract)", "✗ None (or minimal C# 8+ defaults)"),
                ("Multiple inherit",  "✗ One base class max",            "✓ Implement as many as needed"),
                ("IS-A meaning",      "✓ Strong (Circle IS-A Shape)",    "~ Capability (Dog IS ITrainable)"),
                ("Best for",          "Shared code in related types",    "Defining contracts/capabilities"),
            };

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n  {"Feature",-22}{"Abstract Class",-36}{"Interface"}");
            Console.WriteLine("  " + new string('─', 70));
            Console.ResetColor();

            foreach (var (feature, absClass, iface) in comparisons)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"  {feature,-22}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{absClass,-36}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(iface);
            }
            Console.ResetColor();

            UI.KeyPoint("When in doubt: prefer interfaces. They're more flexible and don't lock in an inheritance hierarchy.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_LiveDemo()
        {
            UI.SubHeader("Live Demo — Abstract Class in Action");

            UI.Print("The Shape hierarchy is defined at the top of this file. Watch:\n");

            UI.LiveDemo("Polymorphism through abstract base class", () =>
            {
                // Each is a different concrete type, but all used as 'Shape'
                var shapes = new List<Shape>
                {
                    new Circle(5, "Red"),
                    new Rectangle(4, 6, "Blue"),
                    new Triangle(3, 4, 5, "Green"),
                    new Circle(2, "Purple"),
                };

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     foreach (Shape shape in shapes)");
                Console.WriteLine("         shape.Describe();");
                Console.WriteLine();
                Console.ResetColor();

                double totalArea = 0;
                foreach (var shape in shapes)
                {
                    shape.Describe();
                    totalArea += shape.Area();
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n     Total area of all shapes: {totalArea:F2}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     One loop. Four types. Zero if-else or type-checking.");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE:\n\n" +
                "You're building a document editor that supports:\n" +
                "  - PDF documents (can be read, printed, exported to text)\n" +
                "  - Word documents (can be read, printed, saved as draft, converted to PDF)\n" +
                "  - Spreadsheets (can be read, calculated, printed, exported to CSV)\n\n" +
                "Would you use an abstract class, interfaces, or both? Sketch the hierarchy.",

                "SUGGESTED DESIGN:\n\n" +
                "  Use BOTH:\n\n" +
                "  Abstract class 'Document':\n" +
                "    - Shared state: _filePath, _createdAt, _lastModified\n" +
                "    - Shared behavior: Open(), Close(), GetMetadata()\n" +
                "    - Abstract: abstract string GetContent(); abstract void Save();\n\n" +
                "  Interfaces for capabilities:\n" +
                "    IExportable: ExportTo(string format)\n" +
                "    IPrintable:  Print(PrintSettings settings)\n" +
                "    ICalculable: Recalculate()  (only spreadsheets)\n\n" +
                "  Classes:\n" +
                "    PdfDocument : Document, IPrintable, IExportable\n" +
                "    WordDocument: Document, IPrintable, IExportable\n" +
                "    Spreadsheet : Document, IPrintable, IExportable, ICalculable\n\n" +
                "  Why abstract class for Document? All docs share file path, dates, open/close.\n" +
                "  Why interfaces for capabilities? Not all docs support all operations."
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Abstraction", new QuizQuestion[]
            {
                new(
                    "What is the main purpose of abstraction?",
                    new[] {
                        "To make fields private so they can't be accessed",
                        "To hide implementation details and expose only what callers need to know",
                        "To prevent classes from being inherited",
                        "To allow multiple classes to share the same method"
                    },
                    1,
                    "Abstraction hides complexity behind a simple interface. Callers know WHAT a thing does, not HOW it does it."
                ),
                new(
                    "What happens if a class inherits from an abstract class but doesn't implement all abstract methods?",
                    new[] {
                        "The missing methods are automatically given empty implementations",
                        "A runtime exception is thrown when you call the missing method",
                        "A compile-time error — the class must also be declared abstract or implement all methods",
                        "The base class's abstract method is called (which throws NotImplementedException)"
                    },
                    2,
                    "The compiler enforces abstract method implementation. The derived class must implement all abstract members OR itself be declared abstract."
                ),
                new(
                    "Can you create an instance of an abstract class?",
                    new[] {
                        "Yes, by using 'new AbstractClass()' like any other class",
                        "Only if the abstract class has no abstract methods",
                        "No — abstract classes cannot be instantiated, only inherited",
                        "Yes, but only inside the same assembly"
                    },
                    2,
                    "Abstract classes exist to be inherited from, not instantiated. Their 'new' constructor is intentionally disabled."
                ),
                new(
                    "A class needs to be BOTH a Vehicle AND implement IElectric and IConnected. What's the only valid approach?",
                    new[] {
                        "Make Vehicle an interface too, since C# allows multiple interface inheritance",
                        "Inherit from Vehicle, and implement both IElectric and IConnected",
                        "Create a new class that inherits from both Vehicle, IElectric, and IConnected",
                        "Use abstract classes for all three"
                    },
                    1,
                    "C# allows one base class (abstract or concrete) plus any number of interfaces. Inherit from Vehicle (the IS-A part) and implement IElectric, IConnected (the CAN-DO parts)."
                ),
                new(
                    "When should you use an interface over an abstract class?",
                    new[] {
                        "When you want to share implementation code between related classes",
                        "When you need fields and constructors in the base type",
                        "When defining a capability that unrelated classes might share, or when a class needs to satisfy multiple contracts",
                        "Always — abstract classes are outdated"
                    },
                    2,
                    "Interfaces define capabilities (CAN-DO). Any unrelated class can implement ILoggable. Abstract classes define identity (IS-A) with shared code."
                ),
            });
        }
    }
}
