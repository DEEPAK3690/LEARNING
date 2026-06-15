// ============================================================
// CONCEPT 8: ADVANCED OOP — Sealed, Static, Generics, Records, Object
// ============================================================

namespace OOP;

// ============================================================
// 8A. SEALED CLASS — Prevent Further Inheritance
// ============================================================
//
// `sealed` on a class = no one can inherit from it.
// `sealed` on an override = no further class can override it.
// WHY: Lock in behavior, security (prevent tampering via subclass),
//      small perf gain (compiler can devirtualize calls).
//
// Examples in .NET: string, int (value types are sealed implicitly)

public class Vehicle
{
    public virtual string Type => "Vehicle";
    public virtual void StartEngine() => Console.WriteLine("  Engine starting...");
}

public sealed class SportsCar : Vehicle // no class can inherit from SportsCar
{
    public override string Type => "Sports Car";
    public override void StartEngine() => Console.WriteLine("  SportsCar: VROOOOM!");
    public void ActivateTurbo() => Console.WriteLine("  Turbo activated!");
}

// class UltraSportsCar : SportsCar { } // COMPILE ERROR — SportsCar is sealed

public class Truck : Vehicle
{
    public override string Type => "Truck";

    // `sealed override` — this Truck can override, but nobody can override further
    public sealed override void StartEngine() => Console.WriteLine("  Truck: Diesel rumble.");
}

// class BigTruck : Truck {
//     public override void StartEngine() { } // COMPILE ERROR — sealed override
// }

// ============================================================
// 8B. STATIC CLASS — Utility Container
// ============================================================
//
// A static class:
//  - Cannot be instantiated (no `new`)
//  - All members must be `static`
//  - Cannot inherit or be inherited
//  - Lives for the entire app lifetime
//  - Perfect for utility/helper functions, extension methods

public static class MathUtils
{
    public static double Clamp(double value, double min, double max)
        => Math.Max(min, Math.Min(max, value));

    public static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i <= Math.Sqrt(n); i++)
            if (n % i == 0) return false;
        return true;
    }

    public static int Fibonacci(int n)
    {
        if (n <= 1) return n;
        int a = 0, b = 1;
        for (int i = 2; i <= n; i++) (a, b) = (b, a + b);
        return b;
    }
}

// EXTENSION METHODS — live in a static class, extend existing types
// `this` on the first parameter = "extend this type"
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? s) => string.IsNullOrEmpty(s);
    public static string Truncate(this string s, int maxLen)
        => s.Length <= maxLen ? s : s[..maxLen] + "...";
    public static string ToTitleCase(this string s)
        => string.Join(" ", s.Split(' ').Select(w => w.Length > 0
            ? char.ToUpper(w[0]) + w[1..].ToLower() : w));
}

// ============================================================
// 8C. GENERICS — Type-Safe Reusable Code
// ============================================================
//
// ANALOGY: A box template. You can make a Box<Apple>, Box<Book>.
//          Same box logic, different content types.
//
// WHY: Without generics you'd use `object` and lose type safety.
//      Generics = type-safe + no boxing overhead.

// Generic class
public class Stack<T>
{
    private readonly List<T> _items = new();

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        if (_items.Count == 0) throw new InvalidOperationException("Stack is empty");
        var item = _items[^1]; // index from end: ^1 = last item
        _items.RemoveAt(_items.Count - 1);
        return item;
    }

    public T Peek() => _items.Count > 0 ? _items[^1] : throw new InvalidOperationException("Empty");
    public int Count => _items.Count;
    public bool IsEmpty => _items.Count == 0;
}

// Generic method — T is resolved per call
public static class GenericUtils
{
    // Swap two values of any type
    public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

    // Return the largest of two values (constraint: T must implement IComparable<T>)
    public static T Max<T>(T a, T b) where T : IComparable<T>
        => a.CompareTo(b) >= 0 ? a : b;

    // Generic pair
    public static (T1, T2) MakePair<T1, T2>(T1 first, T2 second) => (first, second);
}

// Generic with constraints
// where T : class         → T must be a reference type
// where T : struct        → T must be a value type
// where T : new()         → T must have a parameterless constructor
// where T : SomeBaseClass → T must inherit from SomeBaseClass
// where T : ISomeInterface→ T must implement that interface
public class Repository<T> where T : class, new()
{
    private readonly List<T> _store = new();

    public void Add(T item) => _store.Add(item);
    public T? GetFirst() => _store.FirstOrDefault();
    public IEnumerable<T> GetAll() => _store.AsReadOnly();
    public int Count => _store.Count;

    // Create a new T using the new() constraint
    public T CreateNew() => new T();
}

// ============================================================
// 8D. RECORDS — Immutable Data Objects (C# 9+)
// ============================================================
//
// A `record` is a class optimized for immutable data.
// Auto-generates: constructor, properties, ToString, Equals,
//                 GetHashCode, and with-expression support.
// Perfect for: DTOs, value objects, immutable data transfer.

public record Point(double X, double Y); // Positional record — compiler generates everything

public record Person2(string Name, int Age)
{
    // Can add computed properties
    public bool IsAdult => Age >= 18;

    // Can add methods too
    public string Greet() => $"Hi, I'm {Name} and I'm {Age} years old.";
}

// ============================================================
// 8E. THE `object` BASE CLASS — Root of All Types
// ============================================================
//
// Every class in C# implicitly inherits from `object` (System.Object).
// Key methods you can override:
//   ToString()    — string representation
//   Equals()      — value equality
//   GetHashCode() — hash code (must match Equals contract)

public class Temperature
{
    public double Celsius { get; }
    public Temperature(double celsius) => Celsius = celsius;

    public double Fahrenheit => Celsius * 9 / 5 + 32;

    // Override ToString — what you get from Console.WriteLine(temp)
    public override string ToString() => $"{Celsius}°C / {Fahrenheit:F1}°F";

    // Override Equals — two Temperature objects are equal if same Celsius value
    // Must ALSO override GetHashCode when you override Equals
    public override bool Equals(object? obj)
        => obj is Temperature other && Celsius == other.Celsius;

    // GetHashCode contract: if a.Equals(b) → a.GetHashCode() == b.GetHashCode()
    public override int GetHashCode() => Celsius.GetHashCode();
}

public static class AdvancedDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 8. ADVANCED OOP ==========");

        // --- SEALED ---
        Console.WriteLine("\n-- Sealed class:");
        var sports = new SportsCar();
        sports.StartEngine();
        sports.ActivateTurbo();
        Vehicle v = sports; // can still upcast
        Console.WriteLine($"Type via base ref: {v.Type}");

        // --- STATIC CLASS ---
        Console.WriteLine("\n-- Static class (MathUtils):");
        Console.WriteLine($"  Clamp(15, 0, 10) = {MathUtils.Clamp(15, 0, 10)}");
        Console.WriteLine($"  IsPrime(17) = {MathUtils.IsPrime(17)}");
        Console.WriteLine($"  Fibonacci(10) = {MathUtils.Fibonacci(10)}");

        Console.WriteLine("\n-- Extension methods on string:");
        string title = "hello world from csharp";
        Console.WriteLine($"  Original: {title}");
        Console.WriteLine($"  TitleCase: {title.ToTitleCase()}");
        Console.WriteLine($"  Truncated: {title.Truncate(11)}");

        // --- GENERICS ---
        Console.WriteLine("\n-- Generic Stack<T>:");
        var intStack = new Stack<int>();
        intStack.Push(10); intStack.Push(20); intStack.Push(30);
        Console.WriteLine($"  Pop: {intStack.Pop()}"); // 30
        Console.WriteLine($"  Peek: {intStack.Peek()}"); // 20
        Console.WriteLine($"  Count: {intStack.Count}");

        var stringStack = new Stack<string>();
        stringStack.Push("A"); stringStack.Push("B");
        Console.WriteLine($"  String pop: {stringStack.Pop()}"); // B

        Console.WriteLine("\n-- Generic methods:");
        int x = 5, y = 10;
        GenericUtils.Swap(ref x, ref y);
        Console.WriteLine($"  After swap: x={x}, y={y}");
        Console.WriteLine($"  Max(\"apple\", \"banana\") = {GenericUtils.Max("apple", "banana")}");
        var pair = GenericUtils.MakePair("Deepak", 28);
        Console.WriteLine($"  Pair: {pair.Item1}, {pair.Item2}");

        // --- RECORDS ---
        Console.WriteLine("\n-- Records (C# 9+):");
        var p1 = new Point(3, 4);
        var p2 = new Point(3, 4);
        var p3 = new Point(5, 6);

        Console.WriteLine($"  p1: {p1}");           // auto ToString: Point { X = 3, Y = 4 }
        Console.WriteLine($"  p1 == p2: {p1 == p2}"); // true — value equality!
        Console.WriteLine($"  p1 == p3: {p1 == p3}"); // false

        var person = new Person2("Deepak", 28);
        Console.WriteLine($"  {person.Greet()} | IsAdult: {person.IsAdult}");

        // `with` expression — non-destructive mutation (creates a copy with changes)
        var older = person with { Age = 30 };
        Console.WriteLine($"  Original: {person} | Copy: {older}");
        Console.WriteLine($"  Same ref: {ReferenceEquals(person, older)}"); // false — new object

        // --- OBJECT BASE CLASS ---
        Console.WriteLine("\n-- object base class (Equals, GetHashCode, ToString):");
        var t1 = new Temperature(100);
        var t2 = new Temperature(100);
        var t3 = new Temperature(37);

        Console.WriteLine($"  t1: {t1}");
        Console.WriteLine($"  t1.Equals(t2): {t1.Equals(t2)}"); // true — same Celsius
        Console.WriteLine($"  t1 == t2: {t1 == t2}");           // false — == checks reference by default (for classes)
        Console.WriteLine($"  HashCode match: {t1.GetHashCode() == t2.GetHashCode()}"); // true

        // object.GetType() — available on every object
        Console.WriteLine($"  t1.GetType(): {t1.GetType().Name}");
        Console.WriteLine($"  t1.GetType().BaseType: {t1.GetType().BaseType?.Name}"); // Object

        // KEY TAKEAWAYS:
        // sealed     = lock class/method from further inheritance/override
        // static     = utility container, no instances, extension methods live here
        // Generics   = type-safe reusable code — prefer over `object` / casting
        // record     = immutable data type, value equality, with-expressions, auto ToString
        // object     = everything inherits from it; override ToString/Equals/GetHashCode as needed
    }
}
