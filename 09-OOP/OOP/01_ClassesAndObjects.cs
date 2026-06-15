// ============================================================
// CONCEPT 1: CLASSES AND OBJECTS — The Foundation of OOP
// ============================================================
//
// ANALOGY: A CLASS is a blueprint (like a house plan).
//           An OBJECT is the actual house built from that blueprint.
//           You can build many houses from one blueprint.
//
// Class  = Definition / Template / Blueprint
// Object = Instance created from that class using `new`

namespace OOP;

// ----- Defining a Class -----
// A class groups related DATA (fields/properties) and BEHAVIOR (methods) together.
// This models a real-world "thing" — here, a Car.

public class Car
{
    // FIELDS — private raw storage variables (data the object holds)
    // Convention: prefix with underscore _ to distinguish from parameters
    private string _brand;
    private int _speed;

    // PROPERTY — a controlled way to expose fields (more on this in Encapsulation)
    // For now, think of it as a "smart field" with get/set
    public string Color { get; set; }

    // CONSTRUCTOR — special method called when `new Car(...)` is written
    // Its job: initialize the object's state
    public Car(string brand, string color)
    {
        _brand = brand;    // `this._brand` would also work; `this` refers to current instance
        Color = color;
        _speed = 0;        // default starting speed
    }

    // METHOD — defines what the object can DO (its behavior)
    public void Accelerate(int amount)
    {
        _speed += amount;
    }

    public void Brake(int amount)
    {
        // Math.Max prevents speed from going below 0
        _speed = Math.Max(0, _speed - amount);
    }

    // `this` keyword — refers to the current instance of the class
    // Useful when a parameter name shadows a field name
    public void SetBrand(string brand)
    {
        this._brand = brand; // `this._brand` = field, `brand` = parameter
    }

    // ToString override — what prints when you Console.WriteLine(car)
    public override string ToString()
    {
        return $"Car: {_brand} | Color: {Color} | Speed: {_speed} km/h";
    }
}

// ----- Static Runner -----
public static class ClassesAndObjectsDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 1. CLASSES & OBJECTS ==========");

        // Creating OBJECTS from the Car class using `new`
        // Each object has its own copy of fields — they are independent
        Car car1 = new Car("Toyota", "Red");
        Car car2 = new Car("BMW", "Black");

        car1.Accelerate(60);
        car1.Accelerate(20);
        car1.Brake(10);

        car2.Accelerate(100);

        // Each object has its own state
        Console.WriteLine(car1); // Car: Toyota | Color: Red | Speed: 70 km/h
        Console.WriteLine(car2); // Car: BMW | Color: Black | Speed: 100 km/h

        // Changing a property on one object does NOT affect the other
        car1.Color = "Blue";
        Console.WriteLine($"car1 color: {car1.Color}"); // Blue
        Console.WriteLine($"car2 color: {car2.Color}"); // Black (unchanged)

        // KEY POINT: `car1` and `car2` are REFERENCE types
        // They store a reference (pointer) to the object in memory, not the object itself
        Car car3 = car1; // car3 now points to the SAME object as car1
        car3.Color = "Green";
        Console.WriteLine($"car1 color after car3 change: {car1.Color}"); // Green — same object!
    }
}
