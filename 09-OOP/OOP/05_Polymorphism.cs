// ============================================================
// CONCEPT 5: POLYMORPHISM — One Interface, Many Forms
// ============================================================
//
// POLY = many, MORPH = form
//
// ANALOGY: A "+" operator works differently for int (adds numbers)
//          and string (concatenates). Same symbol, different behavior.
//
// TWO TYPES:
//  1. Compile-time (Static)  — Method Overloading
//  2. Runtime (Dynamic)      — Method Overriding via virtual/override
//
// This is the most powerful OOP concept — it lets you write code
// that works on a BASE TYPE but automatically calls the right
// DERIVED TYPE's implementation at runtime.

namespace OOP;

// ----- RUNTIME POLYMORPHISM SETUP -----

public class Shape
{
    public string Color { get; set; } = "White";

    // `virtual` = this method CAN be overridden in derived classes
    public virtual double Area() => 0;
    public virtual double Perimeter() => 0;

    // non-virtual method — same for all shapes, not overridable
    public void PrintInfo()
    {
        // At runtime, C# calls the CORRECT Area() based on actual object type
        // Even though `this` is typed as Shape, it calls the derived override!
        Console.WriteLine($"[{GetType().Name}] Color={Color} | Area={Area():F2} | Perimeter={Perimeter():F2}");
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }
    public Circle(double radius) { Radius = radius; Color = "Red"; }

    public override double Area() => Math.PI * Radius * Radius;
    public override double Perimeter() => 2 * Math.PI * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
    public Rectangle(double w, double h) { Width = w; Height = h; Color = "Blue"; }

    public override double Area() => Width * Height;
    public override double Perimeter() => 2 * (Width + Height);
}

public class Triangle : Shape
{
    private double _a, _b, _c;
    public Triangle(double a, double b, double c) { _a = a; _b = b; _c = c; }

    public override double Perimeter() => _a + _b + _c;
    public override double Area()
    {
        double s = Perimeter() / 2; // Heron's formula
        return Math.Sqrt(s * (s - _a) * (s - _b) * (s - _c));
    }
}

// ----- METHOD HIDING with `new` (NOT polymorphism — common trap!) -----
// `new` on a method HIDES the base method rather than overriding it.
// The method called depends on the COMPILE-TIME type, not runtime type.
// This breaks polymorphism — avoid unless you have a specific reason.
public class SpecialShape : Shape
{
    // `new` hides Shape.Area() — does NOT override it
    public new double Area()
    {
        Console.WriteLine("  [SpecialShape.Area called via new hiding]");
        return -1;
    }
}

// ----- COMPILE-TIME POLYMORPHISM: Method Overloading -----
// Same method name, different parameter signatures.
// The compiler picks the right one based on arguments at compile time.
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public int Add(int a, int b, int c) => a + b + c;
    public string Add(string a, string b) => a + b; // "concatenation" for strings

    // Optional parameters — another way to handle varying args (not true overloading)
    public double Multiply(double a, double b, double factor = 1.0)
        => a * b * factor;
}

public static class PolymorphismDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 5. POLYMORPHISM ==========");

        // ---- RUNTIME POLYMORPHISM ----
        // Store different types in a list of the BASE type
        // This is the heart of OOP — write once, works for all derived types
        Console.WriteLine("\n-- Runtime Polymorphism (virtual/override):");
        List<Shape> shapes = new List<Shape>
        {
            new Circle(5),
            new Rectangle(4, 6),
            new Triangle(3, 4, 5)
        };

        // Loop calls PrintInfo on Shape, but Area()/Perimeter() dispatch
        // to the ACTUAL type at runtime — Circle, Rectangle, Triangle
        foreach (Shape shape in shapes)
        {
            shape.PrintInfo(); // correct Area() called even though type is Shape
        }

        // Total area — works with any Shape, including future ones you haven't written yet!
        double totalArea = shapes.Sum(s => s.Area());
        Console.WriteLine($"Total area of all shapes: {totalArea:F2}");

        // ---- METHOD HIDING TRAP ----
        Console.WriteLine("\n-- Method Hiding (new keyword) vs Override — CRITICAL DIFFERENCE:");
        SpecialShape special = new SpecialShape();
        Shape upcast = special; // upcast to Shape reference

        Console.WriteLine($"Via SpecialShape reference: {special.Area()}");  // calls SpecialShape.Area (new)
        Console.WriteLine($"Via Shape reference:        {upcast.Area()}");   // calls Shape.Area (0) — hiding, NOT override!
        // With `override`, BOTH lines would call SpecialShape.Area.
        // With `new`, it depends on the REFERENCE TYPE, not the object type.
        // LESSON: Prefer `override` + `virtual`. Only use `new` to intentionally break polymorphism.

        // ---- COMPILE-TIME POLYMORPHISM ----
        Console.WriteLine("\n-- Compile-time Polymorphism (Method Overloading):");
        var calc = new Calculator();
        Console.WriteLine(calc.Add(1, 2));           // int version
        Console.WriteLine(calc.Add(1.5, 2.5));       // double version
        Console.WriteLine(calc.Add(1, 2, 3));         // 3-param version
        Console.WriteLine(calc.Add("Hello, ", "World!")); // string version
        Console.WriteLine(calc.Multiply(3, 4));       // factor defaults to 1
        Console.WriteLine(calc.Multiply(3, 4, 2.0));  // factor = 2

        // ---- POLYMORPHISM WITH METHOD PARAMETER -----
        Console.WriteLine("\n-- Polymorphism through method parameter:");
        PrintShapeDetails(new Circle(3));
        PrintShapeDetails(new Rectangle(2, 5));
        // This method doesn't need to change when you add new Shape subtypes!
    }

    // Accepts ANY Shape — circle, rectangle, triangle, future shapes too
    private static void PrintShapeDetails(Shape shape)
    {
        Console.WriteLine($"  {shape.GetType().Name}: area = {shape.Area():F2}");
    }
}
