// ============================================================
// FILE: Ch05_Polymorphism.cs
// PURPOSE: Teach Polymorphism — "many forms". The pillar that
//          makes OOP so powerful for extensible systems.
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO: Notification System ────────────────────────────────

    // Base interface — the polymorphic contract
    internal interface INotification
    {
        void Send(string recipient, string message);
        string ChannelName { get; }
    }

    // Three completely different implementations, same interface
    internal class EmailNotification : INotification
    {
        public string ChannelName => "Email";
        public void Send(string recipient, string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"     📧 Email → {recipient}: {message}");
            Console.ResetColor();
        }
    }

    internal class SmsNotification : INotification
    {
        public string ChannelName => "SMS";
        public void Send(string recipient, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"     📱 SMS  → {recipient}: {message.Substring(0, Math.Min(message.Length, 40))}...");
            Console.ResetColor();
        }
    }

    internal class PushNotification : INotification
    {
        public string ChannelName => "Push";
        public void Send(string recipient, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"     🔔 Push → {recipient}: {message}");
            Console.ResetColor();
        }
    }

    // Demo: method overloading (compile-time polymorphism)
    internal class Calculator
    {
        // Three methods with the SAME name but DIFFERENT signatures.
        // The compiler picks the right one based on argument types.
        public int    Add(int    a, int    b) => a + b;
        public double Add(double a, double b) => a + b;
        public string Add(string a, string b) => a + b;  // string concatenation
        public int    Add(int    a, int b, int c) => a + b + c; // extra parameter
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch05_Polymorphism
    {
        public static void Run()
        {
            UI.Header("CHAPTER 5: POLYMORPHISM — ONE INTERFACE, MANY BEHAVIORS");

            UI.Concept(
                "Polymorphism comes from Greek: poly (many) + morphe (forms). " +
                "In OOP: the same interface (method name, property) produces DIFFERENT BEHAVIOR " +
                "depending on the ACTUAL TYPE of the object at runtime. " +
                "It's the pillar that makes systems extensible — add new behavior without changing existing code."
            );
            UI.Pause();

            Section1_TwoTypes();
            Section2_CompileTime();
            Section3_Runtime();
            Section4_HowVirtualDispatchWorks();
            Section5_OpenClosedConnection();
            Section6_PatternMatching();
            Section7_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_TwoTypes()
        {
            UI.SubHeader("Two Types of Polymorphism");

            UI.Diagram("Polymorphism Overview",
                "  POLYMORPHISM",
                "  ├── COMPILE-TIME (Static Polymorphism)",
                "  │       Method Overloading: same name, different parameter signatures",
                "  │       Resolved by the COMPILER at build time",
                "  │       Example: Add(int,int) vs Add(double,double) vs Add(string,string)",
                "  │",
                "  └── RUNTIME (Dynamic Polymorphism)",
                "          Method Overriding: virtual + override",
                "          Resolved at RUNTIME based on the actual object type",
                "          Example: shape.Area() calls Circle.Area() or Rectangle.Area()"
            );

            UI.Analogy("Universal Remote",
                "The 'Volume Up' button on your remote is the SAME button regardless of which TV you point it at. " +
                "But a Samsung TV and an LG TV respond differently to that signal. " +
                "Same message (VolumeUp). Different behavior per device type. " +
                "That's polymorphism. The button is the interface. The TVs are the implementations."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_CompileTime()
        {
            UI.SubHeader("Compile-Time Polymorphism — Method Overloading");

            UI.Concept(
                "Method overloading means multiple methods in the SAME class share the same name " +
                "but differ in their parameter list (count, types, or order). " +
                "The compiler picks the correct one based on the arguments you pass."
            );

            UI.Code("Method overloading rules",
                "public class DataConverter",
                "{",
                "    // SAME name 'Convert', DIFFERENT signatures:",
                "    public string Convert(int value)     => value.ToString();    // int → string",
                "    public string Convert(double value)  => value.ToString(\"F2\");// double → string",
                "    public int    Convert(string value)  => int.Parse(value);    // string → int",
                "    public double Convert(string value, bool asDouble) => double.Parse(value); // 2 params",
                "",
                "    // ❌ INVALID — differs ONLY in return type — this will NOT compile",
                "    // public int    Convert(string value) => int.Parse(value);",
                "    // public double Convert(string value) => double.Parse(value);",
                "    // Return type alone is not enough to distinguish overloads.",
                "}",
                "",
                "// Usage — compiler picks the right method automatically:",
                "var c = new DataConverter();",
                "c.Convert(42);         // → calls Convert(int value)",
                "c.Convert(3.14);       // → calls Convert(double value)",
                "c.Convert(\"123\");    // → calls Convert(string value) → returns int"
            );

            UI.LiveDemo("Overloaded Calculator in action", () =>
            {
                var calc = new Calculator();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     calc.Add(3, 4)            = {calc.Add(3, 4)}");
                Console.WriteLine($"     calc.Add(3.5, 1.2)        = {calc.Add(3.5, 1.2):F1}");
                Console.WriteLine($"     calc.Add(\"Hello \",\"World\") = {calc.Add("Hello ", "World")}");
                Console.WriteLine($"     calc.Add(1, 2, 3)          = {calc.Add(1, 2, 3)}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     Same method name. Compiler chose the right version each time.");
                Console.ResetColor();
            });

            UI.Print("WHEN TO USE OVERLOADING:\n");
            UI.Good("Good use case", "Same logical operation on different types: Math.Abs(int), Math.Abs(double), Math.Abs(decimal).");
            UI.Good("Good use case", "Providing convenience overloads: Method(string) vs Method(string, bool) vs Method(string, int, bool).");
            UI.Bad("Avoid overloading when", "The methods do conceptually different things. Use different names then.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_Runtime()
        {
            UI.SubHeader("Runtime Polymorphism — The Power of virtual/override");

            UI.Concept(
                "Runtime polymorphism means: the SAME method call produces DIFFERENT behavior " +
                "depending on the ACTUAL RUNTIME TYPE of the object — not the declared type of the variable. " +
                "This is resolved at runtime (not compile time), hence the name."
            );

            UI.Code("Runtime polymorphism in full detail",
                "// These three lines all call .Send() on the same variable type (INotification).",
                "// But each calls a COMPLETELY DIFFERENT implementation at runtime.",
                "",
                "INotification n1 = new EmailNotification();",
                "INotification n2 = new SmsNotification();",
                "INotification n3 = new PushNotification();",
                "",
                "// The variable type is 'INotification' — same for all three.",
                "// The ACTUAL object types are Email, Sms, Push.",
                "// The runtime looks at the ACTUAL type and calls its Send().",
                "n1.Send(\"user@test.com\", \"Hello\"); // → EmailNotification.Send() is called",
                "n2.Send(\"+1-555-0100\",  \"Hello\"); // → SmsNotification.Send() is called",
                "n3.Send(\"deviceABC\",    \"Hello\"); // → PushNotification.Send() is called",
                "",
                "// The MAGIC: a single list, a single loop, handles ALL types:",
                "var notifications = new List<INotification> { n1, n2, n3 };",
                "foreach (var n in notifications)",
                "    n.Send(\"recipient\", \"Your order shipped!\");",
                "// Adding PushV2Notification tomorrow? Just add it to the list.",
                "// Zero changes to the loop. That's the Open/Closed Principle."
            );

            UI.Diagram("Runtime dispatch — what happens under the hood",
                "  Source code:  notification.Send(recipient, message);",
                "                         ↓",
                "  Runtime:  'What is the ACTUAL type of notification?'",
                "                         ↓",
                "  Answer:   EmailNotification",
                "                         ↓",
                "  VTable lookup: EmailNotification.Send → calls that specific method",
                "",
                "  Every class has a VTable (virtual method table) — a lookup table",
                "  mapping method names to the correct implementation for that type."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_HowVirtualDispatchWorks()
        {
            UI.SubHeader("How Virtual Dispatch Actually Works (Under the Hood)");

            UI.Concept(
                "This is 'intermediate' knowledge but understanding it eliminates confusion " +
                "about WHY virtual/override works the way it does."
            );

            UI.Diagram("VTable — Virtual Method Dispatch Table",
                "  When you write: INotification n = new EmailNotification();",
                "",
                "  STACK:                    HEAP:",
                "  ┌──────────────┐          ┌────────────────────────────────────┐",
                "  │ n (reference)│─────────▶│ EmailNotification object            │",
                "  └──────────────┘          │ ┌──────────────────────────────┐   │",
                "                            │ │ VTable pointer ─────────────┐│   │",
                "                            │ │ (private fields...)         ││   │",
                "                            │ └─────────────────────────────┘│   │",
                "                            └────────────────────────────────┘   │",
                "                                      ↓                          │",
                "                            EmailNotification's VTable:          │",
                "                            ┌──────────────────────────────────┐ │",
                "                            │ Send → EmailNotification.Send()  │ │",
                "                            │ ChannelName → EmailNotification  │ │",
                "                            └──────────────────────────────────┘",
                "",
                "  When n.Send() is called: follow the VTable pointer → find Send → call it.",
                "  This is why the ACTUAL object's method runs, regardless of the variable type."
            );

            UI.Code("Non-virtual vs virtual performance note",
                "// NON-VIRTUAL call (no 'virtual' keyword):",
                "// Compiler knows EXACTLY which method to call at compile time.",
                "// Direct call — fastest possible.",
                "regularObject.NonVirtualMethod();",
                "",
                "// VIRTUAL call (with 'virtual/override'):",
                "// Runtime must look up VTable to find the right method.",
                "// Tiny overhead (~1-5 nanoseconds), almost never matters in practice.",
                "// Only optimize if profiler proves this is actually a bottleneck (very rare).",
                "polymorphicRef.VirtualMethod();",
                "",
                "// 'sealed' on a class or method tells the JIT: no more overrides exist.",
                "// JIT can then devirtualize (convert to direct call) — performance win.",
                "public sealed class SpecificImpl : IInterface { ... }"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_OpenClosedConnection()
        {
            UI.SubHeader("Polymorphism Enables the Open/Closed Principle");

            UI.Concept(
                "The Open/Closed Principle (OCP) says:\n" +
                "'Software should be OPEN for EXTENSION but CLOSED for MODIFICATION.'\n\n" +
                "Polymorphism is HOW you achieve this. You add new behaviors by adding new " +
                "CLASSES that implement the same interface — without touching existing code."
            );

            UI.Code("Adding a new notification type — zero existing code changes",
                "// EXISTING CODE (you don't touch this):",
                "List<INotification> channels = GetChannels();",
                "foreach (var channel in channels)",
                "    channel.Send(recipient, message);",
                "",
                "// TOMORROW: Product team wants WhatsApp notifications.",
                "// What do you change?  NOTHING above. You just ADD:",
                "public class WhatsAppNotification : INotification",
                "{",
                "    public string ChannelName => \"WhatsApp\";",
                "    public void Send(string recipient, string message)",
                "    {",
                "        Console.WriteLine($\"💬 WhatsApp → {recipient}: {message}\");",
                "    }",
                "}",
                "",
                "// Add to the list. Done. The loop handles it with no changes.",
                "channels.Add(new WhatsAppNotification());"
            );

            UI.KeyPoint("Polymorphism + Interfaces = the ability to add features without touching working code.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_PatternMatching()
        {
            UI.SubHeader("Pattern Matching — Modern C# Type Dispatch");

            UI.Concept(
                "Sometimes you don't own the types or can't add virtual methods to them. " +
                "C# 8+ pattern matching lets you dispatch on type without polymorphism. " +
                "Use it when you MUST — but prefer polymorphism when you can."
            );

            UI.Code("Type switch — explicit type dispatching",
                "// When you CONTROL the types: prefer polymorphism (virtual dispatch)",
                "// When you DON'T control the types: use pattern matching",
                "",
                "double GetArea(object shape)  // shape is some unknown object type",
                "{",
                "    return shape switch",
                "    {",
                "        Circle    c  => Math.PI * c.Radius * c.Radius,  // if it's a Circle",
                "        Rectangle r  => r.Width * r.Height,             // if it's a Rectangle",
                "        Triangle  t  => 0.5 * t.Base * t.Height,        // if it's a Triangle",
                "        null         => throw new ArgumentNullException(nameof(shape)),",
                "        _            => throw new ArgumentException(\"Unknown shape type\")",
                "    };",
                "}",
                "",
                "// Property pattern — match AND destructure simultaneously:",
                "string Classify(Employee emp) => emp switch",
                "{",
                "    { Salary: > 200000 }           => \"Executive\",",
                "    { Salary: > 100000 }           => \"Senior\",",
                "    Director { Department: \"Eng\" } => \"Engineering Director\",",
                "    Manager  m when m.DirectReports.Count > 10 => \"Large team manager\",",
                "    _                              => \"Staff\"",
                "};"
            );

            UI.Bad("When NOT to use pattern matching",
                "Don't type-switch on your own classes if you control them — add a virtual method instead. " +
                "Pattern matching on your own types violates OCP: adding a new type requires finding and " +
                "updating every switch statement. Virtual dispatch is O(1) and requires no changes.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section7_LiveDemo()
        {
            UI.SubHeader("Live Demo — Polymorphism Making Systems Extensible");

            UI.LiveDemo("Notification system sending to all channels", () =>
            {
                var channels = new List<INotification>
                {
                    new EmailNotification(),
                    new SmsNotification(),
                    new PushNotification(),
                };

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     Sending to all channels via ONE loop:");
                Console.WriteLine("     foreach (var n in channels) n.Send(recipient, msg);");
                Console.WriteLine();
                Console.ResetColor();

                foreach (var channel in channels)
                    channel.Send("deepak@example.com", "Your USB testing results are ready!");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n     {channels.Count} channels notified. Zero if-else statements.");
                Console.WriteLine("     To add Slack: implement INotification, add to list. Done.");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE:\n\n" +
                "You're building a payment processing system for an e-commerce app.\n" +
                "The app needs to support: Stripe, PayPal, Apple Pay, and eventually others.\n\n" +
                "1. What interface would you define?\n" +
                "2. What methods would it have?\n" +
                "3. How does polymorphism make it easy to add new payment providers?\n" +
                "4. What happens in the checkout code when you use this interface?",

                "SUGGESTED DESIGN:\n\n" +
                "  interface IPaymentProcessor\n" +
                "  {\n" +
                "      Task<PaymentResult> ProcessAsync(PaymentRequest request);\n" +
                "      Task<RefundResult>  RefundAsync(string transactionId, decimal amount);\n" +
                "      bool                SupportsRefunds { get; }\n" +
                "      string              ProviderName    { get; }\n" +
                "  }\n\n" +
                "  class StripeProcessor : IPaymentProcessor  { ... }\n" +
                "  class PayPalProcessor : IPaymentProcessor  { ... }\n" +
                "  class ApplePayProcessor : IPaymentProcessor{ ... }\n\n" +
                "  CheckoutService only depends on IPaymentProcessor:\n" +
                "    public CheckoutService(IPaymentProcessor processor) ...\n\n" +
                "  Adding 'GooglePay': implement IPaymentProcessor. Zero checkout code changes.\n" +
                "  Testing: inject FakePaymentProcessor that always returns success."
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Polymorphism", new QuizQuestion[]
            {
                new(
                    "What is the key difference between method overloading and method overriding?",
                    new[] {
                        "Overloading is for interfaces; overriding is for classes",
                        "Overloading is compile-time (same class, different signatures); overriding is runtime (derived class changes virtual method)",
                        "Overloading requires 'virtual'; overriding requires 'new'",
                        "There is no difference — they are the same concept"
                    },
                    1,
                    "Overloading: same name, different parameters, same class, resolved at compile time. Overriding: replaces a virtual method in a derived class, resolved at runtime."
                ),
                new(
                    "Animal a = new Dog(); a.Speak(); — Which Speak() is called if Speak() is virtual in Animal and Dog overrides it?",
                    new[] {
                        "Animal.Speak() — because the variable is declared as Animal type",
                        "Dog.Speak() — because the actual runtime object is a Dog",
                        "Both are called, in order: Animal first, then Dog",
                        "A compile error — you can't assign Dog to Animal variable"
                    },
                    1,
                    "Runtime polymorphism: the actual type (Dog) determines which method runs, not the declared type of the variable (Animal)."
                ),
                new(
                    "You have a list of IPaymentProcessor objects. You add a new class GooglePayProcessor. What must change in the checkout loop?",
                    new[] {
                        "Add an if-else case for GooglePayProcessor in the loop",
                        "Rewrite the loop to handle the new type",
                        "Nothing — just add GooglePayProcessor to the list. Polymorphism handles it.",
                        "Update every class that uses IPaymentProcessor"
                    },
                    2,
                    "This is the Open/Closed Principle via polymorphism. New type = new class. Existing code is unchanged. That's the power."
                ),
                new(
                    "What is a VTable (Virtual Dispatch Table)?",
                    new[] {
                        "A database table used by ORMs for storing type information",
                        "A compile-time lookup used to resolve method overloads",
                        "A per-class runtime table mapping virtual method names to their actual implementations",
                        "A list of all virtual methods in an interface"
                    },
                    2,
                    "Every class with virtual methods has a VTable. At runtime, the CLR looks up the object's VTable to find and call the correct overriding implementation."
                ),
            });
        }
    }
}
