// ============================================================
// CONCEPT 3: CONSTRUCTORS — Initializing Objects
// ============================================================
//
// A constructor is a special method that runs automatically
// when you create an object with `new`.
// Its job: put the object into a valid starting state.
//
// RULES:
//  - Same name as the class
//  - No return type (not even void)
//  - Called exactly once per object creation
//  - If you write no constructor, C# provides a free default one

namespace OOP;

public class Person
{
    public string Name { get; private set; }
    public int Age { get; private set; }
    public string Email { get; private set; }

    // ---- 1. DEFAULT (PARAMETERLESS) CONSTRUCTOR ----
    // Called when: new Person()
    // C# provides this automatically ONLY if you define NO constructors.
    // Once you define any constructor, C# removes the free default one.
    public Person()
    {
        Name = "Unknown";
        Age = 0;
        Email = "none@none.com";
        Console.WriteLine("  [Default constructor called]");
    }

    // ---- 2. PARAMETERIZED CONSTRUCTOR ----
    // Called when: new Person("Deepak", 25)
    // : this()  ← CONSTRUCTOR CHAINING — calls the default constructor first
    //             before running this body. Avoids duplicating initialization logic.
    public Person(string name, int age) : this() // chains to Person()
    {
        Name = name;
        Age = age;
        Console.WriteLine($"  [2-param constructor called for {Name}]");
    }

    // ---- 3. CONSTRUCTOR OVERLOADING + CHAINING ----
    // : this(name, age) ← calls the 2-param constructor first, then this runs
    // Chain constructors to avoid copy-pasting default logic
    public Person(string name, int age, string email) : this(name, age)
    {
        Email = email;
        Console.WriteLine($"  [3-param constructor called, Email={Email}]");
    }

    public override string ToString() => $"Person: {Name}, Age: {Age}, Email: {Email}";
}

// ---- 4. STATIC CONSTRUCTOR ----
// Runs ONCE for the TYPE (not per instance) — before the first use of the class.
// Used to initialize static fields.
// Cannot have parameters. Cannot be called manually.
public class AppConfig
{
    public static string Version { get; private set; }
    public static string Environment { get; private set; }
    public string InstanceId { get; }

    // Static constructor — runs once when AppConfig is first used
    static AppConfig()
    {
        Version = "1.0.0";
        Environment = System.Environment.GetEnvironmentVariable("APP_ENV") ?? "Development";
        Console.WriteLine("  [Static constructor ran — type initialized]");
    }

    // Instance constructor — runs per object
    public AppConfig()
    {
        InstanceId = Guid.NewGuid().ToString()[..8];
        Console.WriteLine($"  [Instance constructor ran — id={InstanceId}]");
    }

    public override string ToString() =>
        $"AppConfig v{Version} [{Environment}] — instance: {InstanceId}";
}

// ---- 5. OBJECT INITIALIZER SYNTAX ----
// A C# syntactic shortcut — lets you set PUBLIC properties right after new()
// WITHOUT needing a special constructor for every combination.
// The constructor runs first, THEN the property assignments happen.
public class Product
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = "General";

    public Product() { } // parameterless constructor required for object initializer

    public override string ToString() =>
        $"{Name} ({Category}) — ${Price} | Stock: {Stock}";
}

public static class ConstructorsDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 3. CONSTRUCTORS ==========");

        Console.WriteLine("\n-- Default constructor:");
        var p1 = new Person();
        Console.WriteLine(p1);

        Console.WriteLine("\n-- 2-param constructor (chains to default first):");
        var p2 = new Person("Deepak", 28);
        Console.WriteLine(p2);

        Console.WriteLine("\n-- 3-param constructor (chains up the whole chain):");
        var p3 = new Person("Priya", 25, "priya@example.com");
        Console.WriteLine(p3);

        Console.WriteLine("\n-- Static constructor (fires once on first use of AppConfig):");
        var config1 = new AppConfig();
        var config2 = new AppConfig(); // static ctor does NOT run again
        Console.WriteLine(config1);
        Console.WriteLine(config2);
        Console.WriteLine($"Static Version (same for all): {AppConfig.Version}");

        Console.WriteLine("\n-- Object Initializer syntax:");
        // No need for a (string, decimal, int, string) constructor!
        var product = new Product
        {
            Name = "Laptop",
            Price = 999.99m,
            Stock = 10,
            Category = "Electronics"
        };
        Console.WriteLine(product);

        // You can mix constructor args + initializer
        // new Product("Laptop") { Price = 999 } ← constructor arg first, then properties

        // KEY TAKEAWAY:
        // Constructor chaining (: this / : base) avoids duplicated init code.
        // Static constructor runs once per type — use for one-time type-level setup.
        // Object initializers give flexibility without constructor explosion.
    }
}
