// ============================================================
// CONCEPT 4: INHERITANCE — Reusing and Extending Classes
// ============================================================
//
// ANALOGY: A Dog IS AN Animal. It inherits all animal traits
//          (breathe, eat) and adds its own (bark, fetch).
//
// DEFINITION: A derived (child) class inherits fields, properties,
//             and methods from a base (parent) class.
//             Use it to model "IS-A" relationships.
//
// SYNTAX:  class Child : Parent { }
//
// C# supports SINGLE inheritance for classes (one parent only).
// But a class can implement multiple INTERFACES (covered later).
//
// Everything in C# ultimately inherits from `object` (System.Object).

namespace OOP;

// ----- BASE CLASS -----
// `protected` members are accessible in this class AND all derived classes
// but NOT from outside code. Think of it as "family-only access".
public class Animal
{
    public string Name { get; protected set; }
    public int Age { get; protected set; }
    protected string _sound; // derived classes can read/write this

    // Base constructor — derived classes MUST chain to this (or another base ctor)
    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
        _sound = "...";
        Console.WriteLine($"  [Animal constructor: {name}]");
    }

    // Regular method — inherited as-is by derived classes
    public void Breathe() => Console.WriteLine($"{Name} is breathing.");

    public void Eat(string food) => Console.WriteLine($"{Name} eats {food}.");

    // `virtual` — marks this method as OVERRIDABLE by derived classes
    // Without `virtual`, derived classes cannot override it (only hide it with `new`)
    public virtual void MakeSound()
    {
        Console.WriteLine($"{Name} says: {_sound}");
    }

    public virtual string Describe() => $"{Name} ({Age} yrs old)";

    public override string ToString() => Describe();
}

// ----- DERIVED CLASS -----
// Dog IS-A Animal. It inherits everything from Animal.
public class Dog : Animal
{
    public string Breed { get; private set; }

    // `: base(name, age)` — calls the Animal constructor before Dog's body runs
    // You MUST call a base constructor if the base has no parameterless constructor
    public Dog(string name, int age, string breed) : base(name, age)
    {
        Breed = breed;
        _sound = "Woof!"; // can access protected field from base
        Console.WriteLine($"  [Dog constructor: {breed}]");
    }

    // `override` — replaces the base class virtual method with new behavior
    // Requires the base method to be marked `virtual` or `abstract`
    public override void MakeSound()
    {
        // `base.MakeSound()` — optionally call the parent's version first
        base.MakeSound();
        Console.WriteLine($"{Name} also wags its tail.");
    }

    public override string Describe() => $"Dog: {Name} ({Breed}), {Age} yrs";

    // Dog-specific method — not in Animal
    public void Fetch(string item) => Console.WriteLine($"{Name} fetches the {item}!");
}

// ----- SECOND DERIVED CLASS -----
public class Cat : Animal
{
    public bool IsIndoor { get; private set; }

    public Cat(string name, int age, bool isIndoor) : base(name, age)
    {
        _sound = "Meow~";
        IsIndoor = isIndoor;
        Console.WriteLine($"  [Cat constructor: indoor={isIndoor}]");
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name} says: {_sound} (softly)");
    }

    public override string Describe() => $"Cat: {Name}, {Age} yrs, Indoor: {IsIndoor}";
}

// ----- MULTI-LEVEL INHERITANCE -----
// GuideDog IS-A Dog IS-A Animal  (chain of 3 levels)
public class GuideDog : Dog
{
    public string Handler { get; private set; }

    public GuideDog(string name, int age, string handler)
        : base(name, age, "Labrador") // calls Dog's constructor
    {
        Handler = handler;
    }

    public override string Describe() => $"Guide Dog: {Name}, Handler: {Handler}";
}

public static class InheritanceDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 4. INHERITANCE ==========");

        Console.WriteLine("\n-- Constructor chain (Animal → Dog):");
        var dog = new Dog("Rex", 3, "German Shepherd");
        Console.WriteLine(dog);

        Console.WriteLine("\n-- Inherited methods:");
        dog.Breathe();  // inherited from Animal
        dog.Eat("bone"); // inherited from Animal
        dog.Fetch("ball"); // Dog-specific

        Console.WriteLine("\n-- Overridden method:");
        dog.MakeSound(); // Dog's override (calls base first, then adds tail wag)

        var cat = new Cat("Whiskers", 5, true);
        cat.MakeSound(); // Cat's override

        // ----- IS-A RELATIONSHIP (upcasting) -----
        // A Dog can be stored in an Animal variable — it IS AN Animal
        Animal animal = new Dog("Buddy", 2, "Poodle"); // upcast — always safe
        animal.MakeSound(); // calls DOG's MakeSound (runtime polymorphism — covered in Polymorphism)
        // animal.Fetch("ball"); // COMPILE ERROR — Animal doesn't know about Fetch

        // ----- TYPE CHECKING -----
        Console.WriteLine("\n-- Type checking:");
        Console.WriteLine($"animal is Dog: {animal is Dog}");
        Console.WriteLine($"animal is Cat: {animal is Cat}");
        Console.WriteLine($"animal is Animal: {animal is Animal}"); // always true

        // ----- CASTING -----
        // `as` — safe cast: returns null if cast fails (no exception)
        Dog? castedDog = animal as Dog;
        if (castedDog != null)
        {
            castedDog.Fetch("stick");
        }

        // `is` with pattern — modern C# way (cleaner than as + null check)
        if (animal is Dog d)
        {
            Console.WriteLine($"Pattern match — breed: {d.Breed}");
        }

        // Direct cast (Dog)animal — throws InvalidCastException if wrong type
        // Only use when you are 100% certain of the type

        Console.WriteLine("\n-- Multi-level inheritance:");
        var guide = new GuideDog("Max", 4, "Sarah");
        guide.Breathe(); // from Animal (2 levels up)
        guide.Fetch("toy"); // from Dog (1 level up)
        guide.MakeSound(); // from Dog (overridden, but still works)
        Console.WriteLine(guide); // uses GuideDog.Describe()

        // KEY TAKEAWAYS:
        // `protected`  = base + derived access only
        // `: base(...)`= chain constructor up the hierarchy
        // `virtual`    = allows override in derived class
        // `override`   = replaces parent's virtual method
        // `base.Method()` = call the parent's version
        // `is` / `as`  = safe runtime type checking and casting
        // Upcast is always safe. Downcast can fail — always check first.
    }
}
