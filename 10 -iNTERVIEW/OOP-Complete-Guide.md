# Object-Oriented Programming: Complete Mastery Guide
> From Zero to Production-Ready — C# Focused, Interview-Ready

---

## How to Use This Guide

Each section builds on the previous one. Don't skip ahead. The goal is not memorization — it's building **mental models** that let you reason about design decisions naturally. Read, then close the guide and re-explain the concept to yourself. If you can't, re-read.

---

# PART 1: FOUNDATIONS — Why OOP Exists

---

## Chapter 1: The Problem OOP Solves

### Before OOP: Procedural Programming

In the early days of software, programs were written procedurally — a sequence of instructions executed top to bottom, with functions operating on shared data.

```c
// Procedural style (C-like pseudocode)
string employeeName = "Deepak";
int employeeAge = 30;
double employeeSalary = 80000;

void GiveRaise(ref double salary, double amount) { salary += amount; }
void PrintEmployee(string name, int age, double salary) { ... }
```

**The problems that emerged as software grew:**

| Problem | What Happened |
|---|---|
| Global state | Functions anywhere could modify any data — impossible to track bugs |
| No grouping | Data and functions that belong together were scattered |
| No reuse | Copy-pasting code everywhere; change in one place broke everything |
| No modeling | Software didn't map to how humans think about the real world |
| Spaghetti code | 10,000-line files with functions calling each other in tangled webs |

### The Insight Behind OOP

The key insight: **real-world problems involve *things* that have *properties* and *behaviors*.**

A bank account:
- Has a balance (property)
- Can be deposited into (behavior)
- Can be withdrawn from (behavior)
- Can print a statement (behavior)

OOP says: model your software the same way. Group data and behavior together into **objects**.

---

## Chapter 2: Classes and Objects — The Foundation

### What Is a Class?

A **class** is a **blueprint** — a template that describes what an object will look like and what it can do. It does not exist in memory by itself.

**Analogy:** A class is like an architectural blueprint for a house. The blueprint isn't a house — it's instructions for building a house.

### What Is an Object?

An **object** is a **concrete instance** of a class — it exists in memory and has actual values.

**Analogy:** The actual house built from the blueprint is the object. You can build 100 houses from the same blueprint; each is a separate object with its own state (paint color, furniture, etc.)

```csharp
// CLASS — the blueprint (no memory allocated for data yet)
public class BankAccount
{
    // Fields — the data each object will hold
    private string _owner;
    private decimal _balance;

    // Constructor — runs when an object is created
    public BankAccount(string owner, decimal initialBalance)
    {
        _owner = owner;
        _balance = initialBalance;
    }

    // Methods — the behaviors
    public void Deposit(decimal amount) => _balance += amount;
    public void Withdraw(decimal amount) => _balance -= amount;
    public decimal GetBalance() => _balance;
    public override string ToString() => $"{_owner}: ${_balance:F2}";
}

// OBJECTS — concrete instances (memory is now allocated)
BankAccount deepakAccount = new BankAccount("Deepak", 5000);
BankAccount johnAccount   = new BankAccount("John", 12000);

deepakAccount.Deposit(500);   // only deepakAccount's balance changes
Console.WriteLine(deepakAccount); // Deepak: $5500.00
Console.WriteLine(johnAccount);   // John: $12000.00
```

### Mental Model: Class vs Object

```
CLASS (Blueprint in code — exists at design time):
┌──────────────────────────────────┐
│          BankAccount             │
├──────────────────────────────────┤
│ - _owner: string                 │
│ - _balance: decimal              │
├──────────────────────────────────┤
│ + Deposit(amount)                │
│ + Withdraw(amount)               │
│ + GetBalance(): decimal          │
└──────────────────────────────────┘

OBJECTS (Instances in memory — exist at runtime):
┌───────────────────┐  ┌───────────────────┐
│  deepakAccount    │  │   johnAccount     │
├───────────────────┤  ├───────────────────┤
│ _owner = "Deepak" │  │ _owner = "John"   │
│ _balance = 5500   │  │ _balance = 12000  │
└───────────────────┘  └───────────────────┘
```

### What If Classes Didn't Exist?

You'd have 20 variables per "account" floating around globally. Every function would need to accept all those variables. Adding a new field means updating every function signature. Debugging becomes impossible.

### Fields vs Properties vs Methods

```csharp
public class Employee
{
    // FIELD — raw storage, usually private
    private string _name;
    private decimal _salary;

    // PROPERTY — controlled access to a field
    // Allows validation logic, computed values, read-only access
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            _name = value;
        }
    }

    // AUTO-PROPERTY — compiler generates backing field for you
    public int EmployeeId { get; private set; }
    public string Department { get; set; }

    // COMPUTED PROPERTY — no backing field, computed on demand
    public decimal AnnualSalary => _salary * 12;

    // METHOD — performs an action or computation
    public void GiveRaise(decimal percentage)
    {
        _salary *= (1 + percentage / 100);
    }
}
```

### Constructors

Constructors initialize an object to a valid state. You should never leave an object in an invalid state after construction.

```csharp
public class Order
{
    public int OrderId { get; }
    public DateTime CreatedAt { get; }
    public List<OrderItem> Items { get; }

    // Primary constructor
    public Order(int orderId)
    {
        if (orderId <= 0) throw new ArgumentException("Invalid order ID");
        OrderId = orderId;
        CreatedAt = DateTime.UtcNow;
        Items = new List<OrderItem>();
    }

    // Overloaded constructor — calls primary via this()
    public Order(int orderId, List<OrderItem> items) : this(orderId)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }
}
```

### Static vs Instance

```csharp
public class MathHelper
{
    // STATIC — belongs to the CLASS, not any instance
    // No object needed, shared across all usage
    public static double SquareRoot(double n) => Math.Sqrt(n);
    public static readonly double Pi = 3.14159;

    // INSTANCE — belongs to a specific object
    private double _value;
    public double DoubleMyValue() => _value * 2;
}

// Static: called on the class
double r = MathHelper.SquareRoot(16); // 4.0

// Instance: needs an object
var helper = new MathHelper();
helper.DoubleMyValue();
```

**When to use static:** Utility/helper methods with no state (Math operations, extension methods, factory methods). Don't overuse — static dependencies are hard to test and mock.

### Interview Questions — Classes & Objects

**Q: What is the difference between a class and an object?**
> A class is a compile-time blueprint that defines structure and behavior. An object is a runtime instance of that class, allocated in heap memory with its own state.

**Q: Can you have a class with no constructor?**
> Yes. C# automatically provides a parameterless default constructor if you define no constructors. If you define any constructor, the default is removed unless you explicitly add it back.

**Q: What is the difference between a field and a property?**
> A field is raw storage. A property wraps a field (or computes a value) with get/set accessors that can include validation logic, notifications, or computed values. Properties are the recommended way to expose data in C# classes.

**Q: Why would you make a field private?**
> To enforce encapsulation — prevent external code from putting the object into an invalid state. The object controls all mutations through its methods and properties.

---

# PART 2: THE FOUR PILLARS

---

## Chapter 3: Encapsulation — The First Pillar

### What Is Encapsulation?

Encapsulation means **bundling data and the methods that operate on that data together**, while **hiding the internal implementation details** from the outside world.

Two aspects:
1. **Grouping** — data + behavior live together in a class
2. **Information hiding** — internal details are private; only a controlled public interface is exposed

**Analogy:** A TV remote control. You press the "Volume Up" button — you don't know (or care) whether it's infrared, Bluetooth, how the signal is encoded, or how the TV's audio circuit processes it. The interface is simple; the implementation is hidden.

### Why Encapsulation Exists

Without it:
- Any code anywhere can corrupt an object's state
- Changing internal implementation breaks all code that touched the internals
- No invariants can be maintained (e.g., balance never negative)

### Access Modifiers in C#

| Modifier | Accessible From |
|---|---|
| `public` | Anywhere |
| `private` | Only within the same class |
| `protected` | Same class and derived classes |
| `internal` | Same assembly |
| `protected internal` | Same assembly OR derived classes |
| `private protected` | Same class or derived classes in same assembly |

### Encapsulation in Practice

```csharp
public class BankAccount
{
    private decimal _balance;       // PRIVATE — no one can set this directly
    private List<string> _history;  // PRIVATE — internal detail

    public string Owner { get; }    // PUBLIC read-only after construction
    public decimal Balance => _balance;  // READ-ONLY property

    public BankAccount(string owner, decimal initialBalance)
    {
        if (initialBalance < 0) throw new ArgumentException("Cannot start with negative balance");
        Owner = owner;
        _balance = initialBalance;
        _history = new List<string>();
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Deposit must be positive");
        _balance += amount;
        _history.Add($"Deposited {amount:C} on {DateTime.Now}");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Withdrawal must be positive");
        if (amount > _balance) throw new InvalidOperationException("Insufficient funds");
        _balance -= amount;
        _history.Add($"Withdrew {amount:C} on {DateTime.Now}");
    }

    // Return a COPY so caller can't mutate the internal list
    public IReadOnlyList<string> GetHistory() => _history.AsReadOnly();
}

// External code:
var account = new BankAccount("Deepak", 1000);
account.Deposit(500);
// account._balance = 999999;  // COMPILE ERROR — protected!
// account.Withdraw(-100);      // ArgumentException — business rule enforced!
```

### What Encapsulation Protects Against

```csharp
// WITHOUT encapsulation — disaster scenario
public class BadAccount
{
    public decimal Balance; // anyone can do: account.Balance = -999999;
    public List<string> History; // anyone can clear or corrupt history
}

// Somewhere in the codebase, accidentally:
badAccount.Balance = -50000;  // Now the object is in an invalid state
badAccount.History.Clear();   // Audit trail destroyed
```

### Common Mistakes

1. **Making everything public** — defeats the purpose entirely
2. **Exposing mutable collections** — return `IReadOnlyList<T>` or a copy
3. **Getters/setters that just read/write with no logic** — acceptable for DTOs, but think before you do it for domain objects
4. **Not validating in setters** — the setter is your guard

### Interview Questions — Encapsulation

**Q: Is encapsulation just about making fields private?**
> No. It's about maintaining invariants — ensuring the object is always in a valid, consistent state. Private fields are the mechanism; the invariant is the goal. A public property with proper validation also achieves encapsulation.

**Q: What is the difference between encapsulation and information hiding?**
> They're closely related. Information hiding is the principle (hide implementation details). Encapsulation is the mechanism (bundle data and behavior, restrict access). Encapsulation is how you achieve information hiding.

---

## Chapter 4: Abstraction — The Second Pillar

### What Is Abstraction?

Abstraction means **exposing only what is necessary** and **hiding complexity** behind a simple interface. You present a simplified model of a complex system.

**Analogy:** Driving a car. You interact with a steering wheel, pedals, and gear shift. You don't see the engine combustion, transmission gears, ABS calculations, or fuel injection timing. The car abstracts all that complexity.

**Key distinction from encapsulation:**
- **Encapsulation** — *how* you hide internal data (private fields)
- **Abstraction** — *what* you expose as the public interface (design decision)

Encapsulation is the implementation mechanism. Abstraction is the design principle.

### Abstraction in C#: Abstract Classes and Interfaces

```csharp
// ABSTRACT CLASS — partially implemented, cannot be instantiated
public abstract class Shape
{
    public string Color { get; set; }

    // Abstract method — must be implemented by subclasses
    public abstract double Area();
    public abstract double Perimeter();

    // Concrete method — shared behavior
    public void Describe()
    {
        Console.WriteLine($"I am a {GetType().Name} with area {Area():F2}");
    }
}

public class Circle : Shape
{
    private double _radius;
    public Circle(double radius) => _radius = radius;

    public override double Area() => Math.PI * _radius * _radius;
    public override double Perimeter() => 2 * Math.PI * _radius;
}

public class Rectangle : Shape
{
    private double _width, _height;
    public Rectangle(double w, double h) { _width = w; _height = h; }

    public override double Area() => _width * _height;
    public override double Perimeter() => 2 * (_width + _height);
}

// Caller only knows about Shape — doesn't care which type
List<Shape> shapes = new() { new Circle(5), new Rectangle(4, 6) };
foreach (var shape in shapes)
{
    shape.Describe(); // Works for ALL shapes — the abstraction in action
}
```

### Why Abstraction Matters

Without it, every caller knows exactly how everything is implemented. Change the implementation and every caller breaks. With abstraction, callers depend on the interface, not the implementation — you can change internals freely.

```csharp
// INTERFACE — pure abstraction, zero implementation
public interface ILogger
{
    void Log(string message);
    void LogError(string message, Exception ex);
}

// Multiple concrete implementations
public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
    public void LogError(string message, Exception ex) => Console.Error.WriteLine($"[ERR] {message}: {ex.Message}");
}

public class FileLogger : ILogger
{
    private readonly string _path;
    public FileLogger(string path) => _path = path;
    public void Log(string message) => File.AppendAllText(_path, $"[LOG] {message}\n");
    public void LogError(string message, Exception ex) => File.AppendAllText(_path, $"[ERR] {message}: {ex.Message}\n");
}

// Consumer only depends on ILogger — doesn't care which one
public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger) => _logger = logger;

    public void PlaceOrder(Order order)
    {
        _logger.Log($"Placing order {order.OrderId}");
        // ... business logic
    }
}

// In production:
var service = new OrderService(new FileLogger("/logs/orders.log"));
// In tests:
var service = new OrderService(new ConsoleLogger());
// Tomorrow: swap to DatabaseLogger — OrderService doesn't change
```

### Abstract Class vs Interface: When to Use Which

| | Abstract Class | Interface |
|---|---|---|
| Has implementation | Yes (partial) | No (C# 8+ allows default methods) |
| Constructor | Yes | No |
| Fields | Yes | No |
| Multiple inheritance | No (single) | Yes (multiple) |
| Use when | Sharing code + contract | Defining a contract only |
| IS-A relationship | Yes | Not necessarily (capability) |

**Rule of thumb:**
- Use **interface** when defining a *capability* (`IComparable`, `ILogger`, `ISerializable`)
- Use **abstract class** when sharing *implementation* across related types (`Shape`, `Animal`, `Vehicle`)

### Interview Questions — Abstraction

**Q: What is the difference between abstraction and encapsulation?**
> Abstraction is about *design* — what you expose in your public interface. Encapsulation is about *implementation* — hiding internal data. You use encapsulation (private fields) to implement abstraction (a clean public interface). A class can have good encapsulation but poor abstraction (if it exposes too many unnecessary details).

**Q: When would you choose an abstract class over an interface?**
> When you need to share implementation code among related classes, need constructors, need fields, or want to provide non-abstract methods alongside abstract ones. Interfaces are preferred when you just need a contract without any shared implementation, or when a class needs to satisfy multiple contracts.

---

## Chapter 5: Inheritance — The Third Pillar

### What Is Inheritance?

Inheritance allows a class (child/derived) to **inherit fields, properties, and methods** from another class (parent/base), enabling **code reuse** and establishing an **IS-A relationship**.

**Analogy:** You inherit traits from your parents — eye color, height tendency, some personality traits. But you also have your own unique characteristics. You're still a distinct person (object), but you share a base.

### Why Inheritance Exists

Without inheritance, you'd duplicate code across classes. An `Employee`, `Manager`, and `Director` all have name, ID, salary, hire date. Without inheritance, you copy those fields and methods 3 times. Change one? Update three places. Bugs guaranteed.

```csharp
// BASE CLASS
public class Employee
{
    public int Id { get; }
    public string Name { get; }
    public decimal Salary { get; protected set; }
    public DateTime HireDate { get; }

    public Employee(int id, string name, decimal salary)
    {
        Id = id;
        Name = name;
        Salary = salary;
        HireDate = DateTime.Today;
    }

    public virtual decimal CalculateBonus()
    {
        return Salary * 0.05m; // 5% base bonus
    }

    public virtual string GetTitle() => "Employee";

    public override string ToString() => $"[{GetTitle()}] {Name} (ID: {Id}) - ${Salary:N0}";
}

// DERIVED CLASS — inherits everything from Employee
public class Manager : Employee
{
    public List<Employee> DirectReports { get; } = new();

    public Manager(int id, string name, decimal salary) : base(id, name, salary) { }

    // OVERRIDE — changes inherited behavior
    public override decimal CalculateBonus()
    {
        decimal teamBonus = DirectReports.Sum(e => e.Salary) * 0.01m;
        return base.CalculateBonus() + teamBonus; // call base + add extra
    }

    public override string GetTitle() => "Manager";

    public void AddReport(Employee employee) => DirectReports.Add(employee);
}

// FURTHER DERIVED
public class Director : Manager
{
    public string Department { get; }

    public Director(int id, string name, decimal salary, string dept)
        : base(id, name, salary)
    {
        Department = dept;
    }

    public override decimal CalculateBonus() => Salary * 0.15m; // Flat 15%
    public override string GetTitle() => $"Director of {Department}";
}
```

### Inheritance Hierarchy Mental Model

```
                    Employee
                   /        \
              Manager      Contractor
             /       \
          Director   TeamLead
         /
    VPofEngineering
```

### The Fragile Base Class Problem

**The biggest danger with inheritance:** Changes to the base class break derived classes in unexpected ways.

```csharp
public class Base
{
    public virtual void DoWork()
    {
        Step1(); // Later you add Step3() here
        Step2();
    }
    protected virtual void Step1() { }
    protected virtual void Step2() { }
}

public class Derived : Base
{
    protected override void Step1()
    {
        // Assumed Step1 runs first — now Step3 is in between and breaks my logic
    }
}
```

This is why **"prefer composition over inheritance"** is a core principle.

### When to Use Inheritance

**Good use:** True IS-A relationship where derived classes are genuine specializations.
- `Dog IS-A Animal` ✓
- `Manager IS-A Employee` ✓
- `Circle IS-A Shape` ✓

**Bad use:** Just to reuse code without a true IS-A relationship.
- `Stack` shouldn't inherit from `List` just to reuse list operations
- `Logger` shouldn't inherit from `FileStream` just to reuse file writing

### sealed Classes and Methods

```csharp
// sealed CLASS — cannot be inherited
public sealed class Singleton { ... }

// sealed METHOD — cannot be overridden further
public class Vehicle
{
    public virtual void Start() { }
}
public class Car : Vehicle
{
    public sealed override void Start() { } // No further overriding
}
```

### Interview Questions — Inheritance

**Q: What is the difference between method overriding and method hiding?**
> Overriding (`override` keyword) changes the behavior of a virtual method polymorphically — the derived class version is called even when referenced as the base type. Hiding (`new` keyword) replaces the method only when called through the derived type reference; calling through the base type still uses the base method.

**Q: Why is multiple inheritance not allowed in C#?**
> The "diamond problem": if A inherits from B and C, both of which have a `DoWork()` method, A doesn't know which version to use. C# avoids this by allowing single class inheritance but multiple interface implementation.

**Q: What does `base` do?**
> It refers to the parent class. `base.Method()` calls the parent's version of an overridden method. `base(args)` in a constructor calls the parent constructor.

---

## Chapter 6: Polymorphism — The Fourth Pillar

### What Is Polymorphism?

Polymorphism means **"many forms"** — the ability of objects of different types to be treated as objects of a common type, with each object responding to the same message in its own way.

**Analogy:** You say "speak" to a dog, cat, and parrot. Each responds — but the dog barks, the cat meows, the parrot talks. The *command* is uniform; the *response* is type-specific.

### Two Types of Polymorphism

**1. Compile-time (Static) Polymorphism — Method Overloading**
Same method name, different parameter signatures. Resolved at compile time.

```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public int Add(int a, int b, int c) => a + b + c;
    public string Add(string a, string b) => a + b; // concatenation
}

var calc = new Calculator();
calc.Add(1, 2);         // calls int version
calc.Add(1.5, 2.3);    // calls double version
calc.Add("hi", "bye"); // calls string version
```

**2. Runtime (Dynamic) Polymorphism — Method Overriding**
The version of a method that runs is determined at **runtime** based on the actual object type, not the reference type.

```csharp
public abstract class Notification
{
    public abstract void Send(string message);
}

public class EmailNotification : Notification
{
    private string _email;
    public EmailNotification(string email) => _email = email;
    public override void Send(string message)
        => Console.WriteLine($"Email to {_email}: {message}");
}

public class SmsNotification : Notification
{
    private string _phone;
    public SmsNotification(string phone) => _phone = phone;
    public override void Send(string message)
        => Console.WriteLine($"SMS to {_phone}: {message}");
}

public class PushNotification : Notification
{
    private string _deviceId;
    public PushNotification(string deviceId) => _deviceId = deviceId;
    public override void Send(string message)
        => Console.WriteLine($"Push to {_deviceId}: {message}");
}

// THE POWER OF POLYMORPHISM:
List<Notification> notifications = new()
{
    new EmailNotification("deepak@example.com"),
    new SmsNotification("+1-555-0100"),
    new PushNotification("device-abc-123")
};

// One loop, handles ALL types — add new type tomorrow, this code doesn't change
foreach (var notification in notifications)
{
    notification.Send("Your order has shipped!");
}
```

### The Open/Closed Principle Connection

Polymorphism enables the Open/Closed Principle: **open for extension, closed for modification**. When you add `WhatsAppNotification`, you don't touch the loop above — you just add a new class.

### Virtual Dispatch: How It Works Under the Hood

```
Object in memory:
┌─────────────────────────────────────┐
│  EmailNotification object           │
│  ┌────────────────────────────────┐ │
│  │ VTable pointer ─────────────┐ │ │
│  │ _email = "deepak@..."       │ │ │
│  └────────────────────────────────┘ │
└─────────────────────────────────────┘
                          │
                          ▼
                    VTable for EmailNotification:
                    ┌────────────────────────────┐
                    │ Send → EmailNotification.Send │
                    └────────────────────────────┘

When you call notification.Send():
1. Runtime looks at the actual object
2. Finds its VTable
3. Calls the method pointer in the VTable
→ Always calls the RIGHT type's method
```

### Polymorphism with Interfaces

```csharp
public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount, string currency);
    void Refund(string transactionId);
}

public class StripeProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount, string currency)
    {
        Console.WriteLine($"Stripe: processing {amount} {currency}");
        return true;
    }
    public void Refund(string txId) => Console.WriteLine($"Stripe refund: {txId}");
}

public class PayPalProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount, string currency)
    {
        Console.WriteLine($"PayPal: processing {amount} {currency}");
        return true;
    }
    public void Refund(string txId) => Console.WriteLine($"PayPal refund: {txId}");
}

// Checkout doesn't know or care which processor it is
public class CheckoutService
{
    private readonly IPaymentProcessor _processor;
    public CheckoutService(IPaymentProcessor processor) => _processor = processor;

    public void Checkout(decimal amount)
    {
        bool success = _processor.ProcessPayment(amount, "USD");
        if (!success) throw new Exception("Payment failed");
    }
}

// Swap payment providers with zero code change in CheckoutService:
var service = new CheckoutService(new StripeProcessor());
// or:
var service = new CheckoutService(new PayPalProcessor());
```

### Pattern Matching and Type Checking

```csharp
// Modern C# polymorphism with pattern matching
void ProcessShape(Shape shape)
{
    // Type switch — explicit polymorphism
    double result = shape switch
    {
        Circle c    => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t  => 0.5 * t.Base * t.Height,
        _           => throw new ArgumentException("Unknown shape")
    };
    Console.WriteLine($"Area: {result:F2}");
}
```

**When to use pattern matching vs virtual dispatch:**
- Virtual dispatch: when all types are known and part of a shared hierarchy (preferred)
- Pattern matching: when working with types you don't own, or when behavior is external to the class

### Interview Questions — Polymorphism

**Q: What is the difference between overloading and overriding?**
> Overloading is compile-time polymorphism — same method name, different signatures in the same class. The compiler chooses which to call. Overriding is runtime polymorphism — a derived class changes the behavior of a virtual/abstract method from the base class. The runtime chooses which to call based on the actual object type.

**Q: What would happen if you removed the `virtual` keyword from a method and a subclass had `override` on it?**
> The `override` keyword would cause a compile error (cannot override a non-virtual member). If you used `new` instead, method hiding would occur — calling through a base reference would invoke the base method, not the derived one.

**Q: Can you achieve polymorphism without inheritance?**
> Yes, through interfaces. Two unrelated classes can implement the same interface and be treated polymorphically through that interface type, with no inheritance relationship between them.

---

# PART 3: RELATIONSHIPS BETWEEN CLASSES

---

## Chapter 7: Composition vs Aggregation vs Association

### Three Kinds of Relationships

```
ASSOCIATION: "Uses" — A knows about B, weakest relationship
AGGREGATION: "Has-A" — A contains B, B can exist without A  
COMPOSITION: "Owns" — A contains B, B cannot exist without A
INHERITANCE: "Is-A" — A is a specialized form of B
```

### Association

Objects know about each other but neither owns the other.

```csharp
public class Student
{
    public string Name { get; set; }
}

public class Teacher
{
    public string Name { get; set; }

    // Teacher knows about students but doesn't own them
    public void Teach(Student student)
    {
        Console.WriteLine($"{Name} is teaching {student.Name}");
    }
}
```

### Aggregation (Has-A, weak ownership)

```csharp
// AGGREGATION: Department has Employees, but Employees exist independently
public class Department
{
    public string Name { get; }
    private List<Employee> _employees = new();

    public Department(string name) => Name = name;

    public void AddEmployee(Employee e) => _employees.Add(e);
    public void RemoveEmployee(Employee e) => _employees.Remove(e);

    // If Department is destroyed, Employees still exist
}

// Employee is created outside Department — passed in
var emp = new Employee(1, "Deepak", 80000);
var dept = new Department("Engineering");
dept.AddEmployee(emp);
// dept = null → emp still lives
```

### Composition (Owns, strong ownership)

```csharp
// COMPOSITION: Car owns its Engine — Engine doesn't exist without Car
public class Engine
{
    public int Horsepower { get; }
    public Engine(int hp) => Horsepower = hp;
    public void Start() => Console.WriteLine($"Engine starting ({Horsepower}hp)");
}

public class Car
{
    // Engine is created by Car and lives/dies with it
    private readonly Engine _engine;
    public string Model { get; }

    public Car(string model, int horsepower)
    {
        Model = model;
        _engine = new Engine(horsepower); // Car creates its Engine
    }

    public void Start() => _engine.Start();
    // When Car is disposed/GC'd, Engine is too
}
```

### Prefer Composition Over Inheritance

This is one of the most important OOP principles. **Inheritance creates tight coupling.** Composition is more flexible.

```csharp
// BAD: Using inheritance to reuse behavior
public class Stack<T> : List<T>  // Stack IS-A List? No. Wrong.
{
    public void Push(T item) => Add(item);
    public T Pop() { var item = this[Count - 1]; RemoveAt(Count - 1); return item; }
    // Problem: All List<T> methods are exposed — caller can call Insert, Remove, etc.
    // Stack's invariant (LIFO) is broken
}

// GOOD: Using composition
public class Stack<T>
{
    private readonly List<T> _items = new(); // CONTAINS a list, doesn't extend it

    public void Push(T item) => _items.Add(item);
    public T Pop()
    {
        if (_items.Count == 0) throw new InvalidOperationException("Stack is empty");
        var item = _items[^1];
        _items.RemoveAt(_items.Count - 1);
        return item;
    }
    public T Peek() => _items.Count > 0 ? _items[^1] : throw new InvalidOperationException("Empty");
    public int Count => _items.Count;
    // Only LIFO operations are exposed — invariant maintained
}
```

### Real-World Composition Example: Building a Document Editor

```csharp
// Each component is independent, composable, replaceable
public interface ISpellChecker { List<string> Check(string text); }
public interface IAutoSaver { void Save(Document doc); }
public interface IFormatter { string Format(string text); }

public class Document
{
    private string _content;
    private readonly ISpellChecker _spellChecker;
    private readonly IAutoSaver _autoSaver;
    private readonly IFormatter _formatter;

    public Document(ISpellChecker spellChecker, IAutoSaver autoSaver, IFormatter formatter)
    {
        _spellChecker = spellChecker;
        _autoSaver    = autoSaver;
        _formatter    = formatter;
    }

    public void Type(string text)
    {
        _content += text;
        var errors = _spellChecker.Check(text);
        if (errors.Any()) Console.WriteLine($"Spell errors: {string.Join(", ", errors)}");
        _autoSaver.Save(this);
    }
}
// Swap any component independently. No inheritance needed.
```

### Interview Questions — Relationships

**Q: What is the difference between composition and aggregation?**
> Both are "has-a" relationships. In composition, the child object's lifetime is tied to the parent — if the parent is destroyed, so is the child (e.g., a Heart in a Person). In aggregation, the child can exist independently (e.g., Employees in a Department — fire the department, the employees still exist).

**Q: Why prefer composition over inheritance?**
> Inheritance creates tight coupling and the fragile base class problem. Composition is more flexible — you can change behavior at runtime by swapping components, you avoid deep hierarchies, and you don't expose all of the base class's methods. It also better supports the Single Responsibility Principle.

---

# PART 4: INTERFACES DEEP DIVE

---

## Chapter 8: Interfaces — Contracts Without Implementation

### What Is an Interface?

An interface is a **pure contract** — it defines *what* a type can do, with zero *how*. It's a promise: "any class implementing me guarantees these methods exist."

```csharp
public interface IRepository<T>
{
    T GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    bool Exists(int id);
}
```

### Multiple Interface Implementation

```csharp
public interface ILoggable
{
    void Log(string message);
}

public interface ISerializable
{
    string Serialize();
    void Deserialize(string data);
}

public interface IValidatable
{
    bool Validate();
    IEnumerable<string> GetValidationErrors();
}

// A class can implement ALL of them
public class Order : ILoggable, ISerializable, IValidatable
{
    public void Log(string message) => Console.WriteLine($"[Order] {message}");

    public string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
    public void Deserialize(string data) { /* ... */ }

    public bool Validate() => !GetValidationErrors().Any();
    public IEnumerable<string> GetValidationErrors()
    {
        if (OrderId <= 0) yield return "Invalid order ID";
        if (!Items.Any()) yield return "Order must have items";
    }

    public int OrderId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
```

### Interface Segregation (Preview of SOLID-I)

Don't force classes to implement methods they don't need. Break large interfaces into small, focused ones.

```csharp
// BAD: Fat interface forces implementers to stub methods they don't use
public interface IWorker
{
    void Work();
    void Eat();  // Robots don't eat
    void Sleep(); // Robots don't sleep
}

// GOOD: Segregated interfaces
public interface IWorkable { void Work(); }
public interface IFeedable { void Eat(); }
public interface ISleepable { void Sleep(); }

public class HumanWorker : IWorkable, IFeedable, ISleepable
{
    public void Work() => Console.WriteLine("Working...");
    public void Eat() => Console.WriteLine("Eating...");
    public void Sleep() => Console.WriteLine("Sleeping...");
}

public class Robot : IWorkable
{
    public void Work() => Console.WriteLine("Computing...");
    // No Eat() or Sleep() — because robots don't need them
}
```

### Default Interface Methods (C# 8+)

```csharp
public interface ILogger
{
    void Log(string message);

    // Default implementation — implementing classes don't have to override
    void LogWarning(string message) => Log($"[WARN] {message}");
    void LogError(string message) => Log($"[ERROR] {message}");
}

// Minimal implementation — gets default LogWarning and LogError for free
public class SimpleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
    // LogWarning and LogError work automatically
}
```

**Use sparingly** — default methods can blur the line between interface and abstract class. Prefer abstract classes when you need shared implementation.

---

# PART 5: SOLID PRINCIPLES

---

## Chapter 9: SOLID — The Five Principles of Good OOP Design

SOLID is an acronym for five design principles that make OOP code maintainable, extensible, and testable.

---

### S — Single Responsibility Principle (SRP)

**"A class should have only one reason to change."**

More practically: **a class should do one thing and do it well.**

```csharp
// VIOLATION: This class does three things
public class UserService
{
    public User GetUser(int id) { /* ... */ }
    public void SaveToDatabase(User user) { /* ... */ } // Data access concern
    public void SendWelcomeEmail(User user) { /* ... */ } // Email concern
    public string FormatUserReport(User user) { /* ... */ } // Reporting concern
}
// Change in email template → modify UserService
// Change in DB schema → modify UserService
// Change in report format → modify UserService
// 3 reasons to change = 3 responsibilities

// FIX: Split into focused classes
public class UserRepository      // Database concern
{
    public User GetById(int id) { /* ... */ }
    public void Save(User user) { /* ... */ }
}

public class UserEmailService    // Email concern
{
    public void SendWelcomeEmail(User user) { /* ... */ }
}

public class UserReportFormatter // Reporting concern
{
    public string Format(User user) { /* ... */ }
}

public class UserService         // Orchestration only
{
    private readonly UserRepository _repo;
    private readonly UserEmailService _emailService;

    public UserService(UserRepository repo, UserEmailService emailService)
    {
        _repo = repo;
        _emailService = emailService;
    }

    public void RegisterUser(User user)
    {
        _repo.Save(user);
        _emailService.SendWelcomeEmail(user);
    }
}
```

**Benefits:** Each class is easier to test, understand, and modify. Changes are localized.

---

### O — Open/Closed Principle (OCP)

**"Software entities should be open for extension but closed for modification."**

Add new behavior by adding new code, not by changing existing code.

```csharp
// VIOLATION: Adding a new discount type requires modifying existing code
public class DiscountService
{
    public decimal GetDiscount(string customerType, decimal price)
    {
        if (customerType == "Regular") return price * 0.05m;
        if (customerType == "Premium") return price * 0.10m;
        if (customerType == "VIP") return price * 0.20m;
        // Adding "Corporate" means modifying this method — dangerous
        return 0;
    }
}

// FIX: Open for extension (add new class), closed for modification
public interface IDiscountStrategy
{
    decimal Calculate(decimal price);
}

public class RegularDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal price) => price * 0.05m;
}

public class PremiumDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal price) => price * 0.10m;
}

public class VIPDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal price) => price * 0.20m;
}

// Adding Corporate? Create CorporateDiscount class — zero changes to existing code
public class CorporateDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal price) => price * 0.25m;
}

public class DiscountService
{
    private readonly IDiscountStrategy _strategy;
    public DiscountService(IDiscountStrategy strategy) => _strategy = strategy;
    public decimal GetDiscount(decimal price) => _strategy.Calculate(price);
}
```

---

### L — Liskov Substitution Principle (LSP)

**"Objects of a derived class must be substitutable for objects of the base class without breaking correctness."**

If `S` is a subtype of `T`, then objects of type `T` may be replaced with objects of type `S` without altering any desirable property of the program.

```csharp
// VIOLATION: Classic rectangle/square problem
public class Rectangle
{
    public virtual double Width { get; set; }
    public virtual double Height { get; set; }
    public double Area => Width * Height;
}

public class Square : Rectangle
{
    // Square enforces equal sides
    public override double Width { set { base.Width = value; base.Height = value; } }
    public override double Height { set { base.Width = value; base.Height = value; } }
}

// This code breaks with Square:
void TestRectangle(Rectangle r)
{
    r.Width = 5;
    r.Height = 3;
    Console.WriteLine(r.Area); // Expected: 15. For Square: 9. BROKEN!
}

// FIX: Don't make Square inherit from Rectangle if they can't be substituted
public abstract class Shape
{
    public abstract double Area { get; }
}

public class Rectangle : Shape
{
    public double Width { get; }
    public double Height { get; }
    public Rectangle(double w, double h) { Width = w; Height = h; }
    public override double Area => Width * Height;
}

public class Square : Shape
{
    public double Side { get; }
    public Square(double side) => Side = side;
    public override double Area => Side * Side;
}
```

**LSP checklist for derived classes:**
- Don't throw exceptions the base doesn't throw
- Don't strengthen preconditions (accept at least what base accepts)
- Don't weaken postconditions (guarantee at least what base guarantees)
- Don't violate invariants the base establishes

---

### I — Interface Segregation Principle (ISP)

**"Clients should not be forced to depend on interfaces they do not use."**

(Covered in Chapter 8. Fat interfaces create unnecessary coupling.)

```csharp
// BAD
public interface IPrinter
{
    void Print(Document d);
    void Scan(Document d);
    void Fax(Document d);
    void Staple(Document d);
}
// A simple printer has to implement Scan, Fax, Staple it can't do → throws NotImplementedException

// GOOD
public interface IPrintable { void Print(Document d); }
public interface IScannable { void Scan(Document d); }
public interface IFaxable   { void Fax(Document d); }

public class SimplePrinter : IPrintable
{
    public void Print(Document d) => Console.WriteLine("Printing...");
    // No Scan, Fax, Staple — only what it supports
}

public class AllInOnePrinter : IPrintable, IScannable, IFaxable
{
    public void Print(Document d) => Console.WriteLine("Printing...");
    public void Scan(Document d) => Console.WriteLine("Scanning...");
    public void Fax(Document d) => Console.WriteLine("Faxing...");
}
```

---

### D — Dependency Inversion Principle (DIP)

**"High-level modules should not depend on low-level modules. Both should depend on abstractions."**

```csharp
// VIOLATION: High-level OrderService depends on low-level SqlOrderRepository
public class OrderService
{
    private SqlOrderRepository _repository = new SqlOrderRepository(); // concrete dependency
    // If you switch to MongoDB, you have to modify OrderService
}

// FIX: Both depend on IOrderRepository abstraction
public interface IOrderRepository
{
    Order GetById(int id);
    void Save(Order order);
}

public class SqlOrderRepository : IOrderRepository
{
    public Order GetById(int id) { /* SQL query */ }
    public void Save(Order order) { /* SQL insert/update */ }
}

public class MongoOrderRepository : IOrderRepository
{
    public Order GetById(int id) { /* MongoDB query */ }
    public void Save(Order order) { /* MongoDB upsert */ }
}

// High-level module depends on abstraction, not implementation
public class OrderService
{
    private readonly IOrderRepository _repository;

    // Dependency INJECTED from outside — not created inside
    public OrderService(IOrderRepository repository) => _repository = repository;

    public void ProcessOrder(int orderId)
    {
        var order = _repository.GetById(orderId);
        // business logic
        _repository.Save(order);
    }
}

// At composition root (Program.cs / Startup):
var service = new OrderService(new SqlOrderRepository());
// or: new OrderService(new MongoOrderRepository());
// or: new OrderService(new FakeOrderRepository()); // for tests
```

**Dependency Injection** is the mechanism that enables DIP. The dependency is "injected" (provided from outside) rather than created internally.

### SOLID Summary Mental Model

```
S — One class, one job
O — Add features by adding code, not editing code
L — Subclasses must be plug-in replacements for base classes
I — Small, focused interfaces — don't force unused methods
D — Depend on interfaces, not concrete classes; inject dependencies
```

### Interview Questions — SOLID

**Q: What is Dependency Injection and how does it relate to SOLID?**
> Dependency Injection is a technique where an object's dependencies are provided from outside rather than created internally. It's the practical implementation of the Dependency Inversion Principle (D in SOLID). It makes code testable (you can inject mocks), loosely coupled, and configurable.

**Q: How does OCP not mean "never change existing code"?**
> OCP means new *features* should be added through extension (new classes) rather than modification of existing, tested code. Bug fixes and refactoring of existing code are still appropriate. The key is: if you find yourself modifying a class every time a new business requirement arrives, that class violates OCP.

---

# PART 6: DESIGN PATTERNS

---

## Chapter 10: The 23 Gang of Four Patterns

Design patterns are proven, reusable solutions to commonly occurring problems in software design. They fall into three categories:

- **Creational** — how objects are created
- **Structural** — how objects are composed/assembled
- **Behavioral** — how objects communicate and distribute responsibility

### CREATIONAL PATTERNS

#### 1. Singleton — "Only One"

Ensures a class has only one instance and provides a global point of access.

```csharp
public sealed class AppConfiguration
{
    private static AppConfiguration _instance;
    private static readonly object _lock = new object();

    // Properties
    public string DatabaseConnectionString { get; private set; }
    public string ApiKey { get; private set; }

    private AppConfiguration()
    {
        DatabaseConnectionString = Environment.GetEnvironmentVariable("DB_CONN");
        ApiKey = Environment.GetEnvironmentVariable("API_KEY");
    }

    public static AppConfiguration Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null) // double-checked locking
                        _instance = new AppConfiguration();
                }
            }
            return _instance;
        }
    }
}

// Thread-safe alternative using Lazy<T>
public sealed class AppConfig
{
    private static readonly Lazy<AppConfig> _lazy =
        new Lazy<AppConfig>(() => new AppConfig());

    public static AppConfig Instance => _lazy.Value;
    private AppConfig() { }
}
```

**When to use:** Configuration, logging, thread pools, cache.
**Caution:** Singletons are global state — they make testing harder. In modern .NET, prefer registered as singleton in DI container rather than static singletons.

#### 2. Factory Method — "Let Subclasses Decide"

Defines an interface for creating objects, but lets subclasses decide which class to instantiate.

```csharp
public abstract class NotificationFactory
{
    // Factory method — subclasses provide the concrete type
    public abstract INotification CreateNotification();

    // Template method that uses the factory
    public void SendAlert(string message)
    {
        var notification = CreateNotification();
        notification.Send(message);
    }
}

public class EmailFactory : NotificationFactory
{
    private readonly string _email;
    public EmailFactory(string email) => _email = email;
    public override INotification CreateNotification() => new EmailNotification(_email);
}

public class SmsFactory : NotificationFactory
{
    private readonly string _phone;
    public SmsFactory(string phone) => _phone = phone;
    public override INotification CreateNotification() => new SmsNotification(_phone);
}
```

#### 3. Abstract Factory — "Factory of Factories"

Creates families of related objects without specifying concrete classes.

```csharp
// A UI toolkit that can be Windows or Mac style
public interface IButton  { void Render(); }
public interface ICheckbox { void Render(); }

public class WindowsButton  : IButton   { public void Render() => Console.WriteLine("Windows Button"); }
public class MacButton      : IButton   { public void Render() => Console.WriteLine("Mac Button"); }
public class WindowsCheckbox : ICheckbox { public void Render() => Console.WriteLine("Windows Checkbox"); }
public class MacCheckbox    : ICheckbox { public void Render() => Console.WriteLine("Mac Checkbox"); }

public interface IGUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}

public class WindowsFactory : IGUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
}

public class MacFactory : IGUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}

// Application gets a factory injected — never knows which OS
public class Application
{
    private readonly IButton _button;
    private readonly ICheckbox _checkbox;

    public Application(IGUIFactory factory)
    {
        _button = factory.CreateButton();
        _checkbox = factory.CreateCheckbox();
    }

    public void Render() { _button.Render(); _checkbox.Render(); }
}
```

#### 4. Builder — "Step-by-Step Construction"

Constructs complex objects step by step.

```csharp
public class QueryBuilder
{
    private string _table;
    private List<string> _columns = new();
    private List<string> _conditions = new();
    private string _orderBy;
    private int? _limit;

    public QueryBuilder From(string table) { _table = table; return this; }
    public QueryBuilder Select(params string[] cols) { _columns.AddRange(cols); return this; }
    public QueryBuilder Where(string condition) { _conditions.Add(condition); return this; }
    public QueryBuilder OrderBy(string col) { _orderBy = col; return this; }
    public QueryBuilder Limit(int n) { _limit = n; return this; }

    public string Build()
    {
        var cols = _columns.Any() ? string.Join(", ", _columns) : "*";
        var sql = $"SELECT {cols} FROM {_table}";
        if (_conditions.Any()) sql += $" WHERE {string.Join(" AND ", _conditions)}";
        if (_orderBy != null) sql += $" ORDER BY {_orderBy}";
        if (_limit.HasValue) sql += $" LIMIT {_limit}";
        return sql;
    }
}

// Fluent, readable construction
string query = new QueryBuilder()
    .From("Orders")
    .Select("OrderId", "CustomerName", "Total")
    .Where("Status = 'Active'")
    .Where("Total > 100")
    .OrderBy("CreatedAt DESC")
    .Limit(50)
    .Build();
```

---

### STRUCTURAL PATTERNS

#### 5. Decorator — "Wrap to Add Behavior"

Adds behavior to objects dynamically without modifying the class.

```csharp
public interface IDataProcessor
{
    string Process(string data);
}

public class BaseDataProcessor : IDataProcessor
{
    public string Process(string data) => data;
}

// Each decorator wraps another processor
public class EncryptionDecorator : IDataProcessor
{
    private readonly IDataProcessor _inner;
    public EncryptionDecorator(IDataProcessor inner) => _inner = inner;
    public string Process(string data)
    {
        var processed = _inner.Process(data);
        return $"ENCRYPTED({processed})";
    }
}

public class CompressionDecorator : IDataProcessor
{
    private readonly IDataProcessor _inner;
    public CompressionDecorator(IDataProcessor inner) => _inner = inner;
    public string Process(string data)
    {
        var processed = _inner.Process(data);
        return $"COMPRESSED({processed})";
    }
}

public class LoggingDecorator : IDataProcessor
{
    private readonly IDataProcessor _inner;
    public LoggingDecorator(IDataProcessor inner) => _inner = inner;
    public string Process(string data)
    {
        Console.WriteLine($"Processing: {data}");
        var result = _inner.Process(data);
        Console.WriteLine($"Result: {result}");
        return result;
    }
}

// Compose any combination at runtime:
IDataProcessor processor = new LoggingDecorator(
                           new EncryptionDecorator(
                           new CompressionDecorator(
                           new BaseDataProcessor())));

processor.Process("sensitive data");
```

**Real-world:** ASP.NET Core middleware is the Decorator pattern applied to request handling.

#### 6. Adapter — "Make Incompatible Things Work Together"

Converts the interface of a class into another interface that clients expect.

```csharp
// External library you can't modify
public class LegacyPaymentSystem
{
    public bool MakePayment(string accountNum, float amount, string currency)
    {
        Console.WriteLine($"Legacy payment: {amount} {currency} from {accountNum}");
        return true;
    }
}

// Your system's interface
public interface IPaymentGateway
{
    PaymentResult ProcessPayment(PaymentRequest request);
}

public record PaymentRequest(string CardNumber, decimal Amount, string Currency);
public record PaymentResult(bool Success, string TransactionId);

// ADAPTER bridges the gap
public class LegacyPaymentAdapter : IPaymentGateway
{
    private readonly LegacyPaymentSystem _legacy = new LegacyPaymentSystem();

    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        // Convert modern request to legacy format
        bool success = _legacy.MakePayment(
            request.CardNumber,
            (float)request.Amount,   // decimal → float
            request.Currency
        );
        return new PaymentResult(success, Guid.NewGuid().ToString());
    }
}

// Your code works with the new interface — no legacy code exposed
IPaymentGateway gateway = new LegacyPaymentAdapter();
var result = gateway.ProcessPayment(new PaymentRequest("4111...", 99.99m, "USD"));
```

#### 7. Facade — "Simple Interface to Complex Subsystem"

Provides a simplified interface to a complex body of code.

```csharp
// Complex subsystem (you don't simplify these — you hide them)
class VideoDecoder { public byte[] Decode(string file) => new byte[0]; }
class AudioDecoder { public byte[] Decode(string file) => new byte[0]; }
class SubtitleLoader { public string[] Load(string file) => new string[0]; }
class VideoPlayer { public void Play(byte[] video, byte[] audio, string[] subs) => Console.WriteLine("Playing"); }
class BufferManager { public void Prebuffer(byte[] data) { } }

// FACADE: Simple interface hiding all the complexity
public class VideoPlayerFacade
{
    private readonly VideoDecoder _video = new();
    private readonly AudioDecoder _audio = new();
    private readonly SubtitleLoader _subs = new();
    private readonly VideoPlayer _player = new();
    private readonly BufferManager _buffer = new();

    public void Play(string filename, bool withSubtitles = false)
    {
        var videoData = _video.Decode(filename);
        var audioData = _audio.Decode(filename);
        _buffer.Prebuffer(videoData);

        string[] subtitles = withSubtitles ? _subs.Load(filename) : Array.Empty<string>();
        _player.Play(videoData, audioData, subtitles);
    }
}

// Client only sees one method:
var player = new VideoPlayerFacade();
player.Play("movie.mp4", withSubtitles: true);
```

---

### BEHAVIORAL PATTERNS

#### 8. Observer — "Subscribe and Be Notified"

Defines a one-to-many dependency so when one object changes state, all dependents are notified automatically.

```csharp
// Built-in C# events are the Observer pattern
public class StockMarket
{
    public event EventHandler<StockPriceChangedArgs> PriceChanged;

    private decimal _price;
    public string Symbol { get; }

    public StockMarket(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void UpdatePrice(decimal newPrice)
    {
        var old = _price;
        _price = newPrice;
        PriceChanged?.Invoke(this, new StockPriceChangedArgs(Symbol, old, newPrice));
    }
}

public class StockPriceChangedArgs : EventArgs
{
    public string Symbol { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public StockPriceChangedArgs(string sym, decimal old, decimal newP)
    { Symbol = sym; OldPrice = old; NewPrice = newP; }
}

// Observers
class TradingBot
{
    public void OnPriceChanged(object sender, StockPriceChangedArgs e)
    {
        if (e.NewPrice < e.OldPrice * 0.95m)
            Console.WriteLine($"Bot: BUY {e.Symbol} at {e.NewPrice}");
    }
}

class AlertService
{
    public void OnPriceChanged(object sender, StockPriceChangedArgs e)
    {
        Console.WriteLine($"Alert: {e.Symbol} moved from {e.OldPrice} to {e.NewPrice}");
    }
}

// Wire them up
var market = new StockMarket("AAPL", 150m);
var bot = new TradingBot();
var alerts = new AlertService();

market.PriceChanged += bot.OnPriceChanged;
market.PriceChanged += alerts.OnPriceChanged;

market.UpdatePrice(140m); // Both observers notified
```

#### 9. Strategy — "Swap Algorithms at Runtime"

Defines a family of algorithms, encapsulates each one, and makes them interchangeable.

```csharp
public interface ISortStrategy<T>
{
    void Sort(List<T> data);
}

public class QuickSort<T> : ISortStrategy<T> where T : IComparable<T>
{
    public void Sort(List<T> data) => data.Sort(); // simplified
}

public class BubbleSort<T> : ISortStrategy<T> where T : IComparable<T>
{
    public void Sort(List<T> data)
    {
        for (int i = 0; i < data.Count; i++)
            for (int j = 0; j < data.Count - i - 1; j++)
                if (data[j].CompareTo(data[j+1]) > 0)
                    (data[j], data[j+1]) = (data[j+1], data[j]);
    }
}

public class DataSorter<T> where T : IComparable<T>
{
    private ISortStrategy<T> _strategy;

    public DataSorter(ISortStrategy<T> strategy) => _strategy = strategy;

    // Strategy can be swapped at runtime
    public void SetStrategy(ISortStrategy<T> strategy) => _strategy = strategy;

    public List<T> Sort(List<T> data)
    {
        _strategy.Sort(data);
        return data;
    }
}

var sorter = new DataSorter<int>(new QuickSort<int>());
sorter.Sort(new List<int> { 3, 1, 4, 1, 5 });
```

#### 10. Command — "Encapsulate Operations as Objects"

Encapsulates a request as an object, allowing parameterization, queuing, logging, and undo.

```csharp
public interface ICommand
{
    void Execute();
    void Undo();
}

public class TextEditor
{
    private string _content = "";
    public string Content => _content;
    public void InsertText(string text, int pos) => _content = _content.Insert(pos, text);
    public void DeleteText(int pos, int length) => _content = _content.Remove(pos, length);
}

public class InsertCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly string _text;
    private readonly int _position;

    public InsertCommand(TextEditor editor, string text, int position)
    { _editor = editor; _text = text; _position = position; }

    public void Execute() => _editor.InsertText(_text, _position);
    public void Undo() => _editor.DeleteText(_position, _text.Length);
}

public class CommandHistory
{
    private readonly Stack<ICommand> _history = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _history.Push(command);
    }

    public void Undo()
    {
        if (_history.TryPop(out var command))
            command.Undo();
    }
}

var editor = new TextEditor();
var history = new CommandHistory();

history.ExecuteCommand(new InsertCommand(editor, "Hello", 0));
history.ExecuteCommand(new InsertCommand(editor, " World", 5));
Console.WriteLine(editor.Content); // "Hello World"

history.Undo();
Console.WriteLine(editor.Content); // "Hello"
```

#### 11. Repository Pattern (Enterprise Pattern)

Abstracts data access behind a collection-like interface.

```csharp
public interface IOrderRepository
{
    Order GetById(int id);
    IEnumerable<Order> GetByCustomer(int customerId);
    IEnumerable<Order> GetAll();
    void Add(Order order);
    void Update(Order order);
    void Delete(int id);
}

// In-memory implementation (for testing or simple apps)
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public Order GetById(int id) => _orders.FirstOrDefault(o => o.OrderId == id);
    public IEnumerable<Order> GetByCustomer(int customerId)
        => _orders.Where(o => o.CustomerId == customerId);
    public IEnumerable<Order> GetAll() => _orders.AsReadOnly();
    public void Add(Order order) { order.OrderId = _nextId++; _orders.Add(order); }
    public void Update(Order order) { /* find and replace */ }
    public void Delete(int id) => _orders.RemoveAll(o => o.OrderId == id);
}

// EF Core SQL implementation
public class SqlOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public SqlOrderRepository(AppDbContext db) => _db = db;

    public Order GetById(int id) => _db.Orders.Find(id);
    public IEnumerable<Order> GetByCustomer(int customerId)
        => _db.Orders.Where(o => o.CustomerId == customerId).ToList();
    public IEnumerable<Order> GetAll() => _db.Orders.ToList();
    public void Add(Order order) { _db.Orders.Add(order); _db.SaveChanges(); }
    public void Update(Order order) { _db.Orders.Update(order); _db.SaveChanges(); }
    public void Delete(int id) { var o = _db.Orders.Find(id); _db.Orders.Remove(o); _db.SaveChanges(); }
}
```

---

# PART 7: COMPLETE REAL-WORLD PROJECT

---

## Chapter 11: Building an E-Commerce Order Management System

### System Requirements

Build an order management system for an online store that:
- Manages products and inventory
- Handles customer orders through the full lifecycle
- Supports multiple payment methods
- Sends notifications (email, SMS)
- Applies discount strategies
- Provides order history and reporting
- Handles concurrent order placement

### Architecture Decision: Why Each OOP Concept Was Chosen

```
┌─────────────────────────────────────────────────────────────────────┐
│                     ORDER MANAGEMENT SYSTEM                          │
├─────────────────────────────────────────────────────────────────────┤
│  UI / API Layer                                                      │
│     ↓                                                                │
│  Application Services (Orchestration)                                │
│     ↓                                                                │
│  Domain Model (OOP Core — our focus)                                 │
│     ↓                                                                │
│  Infrastructure (Repositories, Payment Gateways, Notification)       │
└─────────────────────────────────────────────────────────────────────┘
```

### Step 1: Domain Entities (Encapsulation + SRP)

```csharp
// VALUE OBJECTS — immutable, no identity
public record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return this with { Amount = Amount + other.Amount };
    }

    public Money Multiply(int quantity) => this with { Amount = Amount * quantity };
    public override string ToString() => $"{Currency} {Amount:F2}";
}

public record Address(string Street, string City, string State, string PostalCode, string Country);

// ENTITY — has identity (Id)
public class Product
{
    public int ProductId { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    private int _stockQuantity;
    public int StockQuantity => _stockQuantity;

    public Product(int id, string name, string description, Money price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name required");
        if (price.Amount <= 0) throw new ArgumentException("Price must be positive");
        if (stock < 0) throw new ArgumentException("Stock cannot be negative");

        ProductId = id;
        Name = name;
        Description = description;
        UnitPrice = price;
        _stockQuantity = stock;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        if (quantity > _stockQuantity)
            throw new InvalidOperationException($"Insufficient stock for {Name}. Available: {_stockQuantity}");
        _stockQuantity -= quantity;
    }

    public void RestoreStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        _stockQuantity += quantity;
    }

    public bool IsInStock(int quantity) => _stockQuantity >= quantity;
}
```

### Step 2: Order Aggregate (Encapsulation, Composition)

```csharp
public class OrderItem
{
    public int ProductId { get; }
    public string ProductName { get; }
    public Money UnitPrice { get; }
    public int Quantity { get; private set; }
    public Money Subtotal => UnitPrice.Multiply(Quantity);

    public OrderItem(int productId, string productName, Money unitPrice, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}

public enum OrderStatus
{
    Draft, Submitted, PaymentPending, PaymentConfirmed,
    Processing, Shipped, Delivered, Cancelled, Refunded
}

public class Order
{
    public int OrderId { get; }
    public int CustomerId { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; }

    private readonly List<OrderItem> _items = new();
    private readonly List<string> _statusHistory = new();

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public Money Subtotal => _items.Aggregate(
        new Money(0, "USD"), (sum, item) => sum.Add(item.Subtotal));

    // Will be set by discount service
    public Money DiscountAmount { get; private set; } = new Money(0, "USD");
    public Money Total => new Money(Subtotal.Amount - DiscountAmount.Amount, "USD");

    public Order(int orderId, int customerId, Address shippingAddress)
    {
        OrderId = orderId;
        CustomerId = customerId;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        AddStatusHistory("Order created");
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a submitted order");
        if (!product.IsInStock(quantity))
            throw new InvalidOperationException($"Insufficient stock for {product.Name}");

        var existing = _items.FirstOrDefault(i => i.ProductId == product.ProductId);
        if (existing != null)
            throw new InvalidOperationException("Item already in order. Update quantity instead.");

        product.ReserveStock(quantity);
        _items.Add(new OrderItem(product.ProductId, product.Name, product.UnitPrice, quantity));
        SetUpdated();
    }

    public void ApplyDiscount(Money discountAmount)
    {
        if (discountAmount.Amount < 0) throw new ArgumentException("Discount cannot be negative");
        if (discountAmount.Amount > Subtotal.Amount) throw new ArgumentException("Discount exceeds subtotal");
        DiscountAmount = discountAmount;
        SetUpdated();
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order already submitted");
        if (!_items.Any())
            throw new InvalidOperationException("Cannot submit empty order");

        TransitionStatus(OrderStatus.Submitted);
    }

    public void MarkPaymentPending()   => TransitionStatus(OrderStatus.PaymentPending);
    public void ConfirmPayment()       => TransitionStatus(OrderStatus.PaymentConfirmed);
    public void StartProcessing()      => TransitionStatus(OrderStatus.Processing);
    public void MarkShipped(string trackingNum) { TransitionStatus(OrderStatus.Shipped); AddStatusHistory($"Tracking: {trackingNum}"); }
    public void MarkDelivered()        => TransitionStatus(OrderStatus.Delivered);

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel shipped/delivered order");

        // Restore stock for all items
        // (In real system, restore from product service)
        TransitionStatus(OrderStatus.Cancelled);
        AddStatusHistory($"Cancelled: {reason}");
    }

    private void TransitionStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        AddStatusHistory($"Status changed to {newStatus}");
        SetUpdated();
    }

    private void AddStatusHistory(string message) =>
        _statusHistory.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}");

    private void SetUpdated() => UpdatedAt = DateTime.UtcNow;

    public IReadOnlyList<string> GetStatusHistory() => _statusHistory.AsReadOnly();
}
```

### Step 3: Discount Strategies (Strategy Pattern + OCP)

```csharp
public interface IDiscountStrategy
{
    Money CalculateDiscount(Order order);
    string Description { get; }
}

public class NoDiscount : IDiscountStrategy
{
    public string Description => "No discount";
    public Money CalculateDiscount(Order order) => new Money(0, "USD");
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percentage;
    public string Description => $"{_percentage}% off";

    public PercentageDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 100) throw new ArgumentException("Invalid percentage");
        _percentage = percentage;
    }

    public Money CalculateDiscount(Order order) =>
        new Money(order.Subtotal.Amount * _percentage / 100, "USD");
}

public class FixedAmountDiscount : IDiscountStrategy
{
    private readonly decimal _amount;
    public string Description => $"${_amount:F2} off";

    public FixedAmountDiscount(decimal amount)
    {
        if (amount < 0) throw new ArgumentException("Discount amount cannot be negative");
        _amount = amount;
    }

    public Money CalculateDiscount(Order order)
    {
        var discount = Math.Min(_amount, order.Subtotal.Amount);
        return new Money(discount, "USD");
    }
}

public class MinimumOrderDiscount : IDiscountStrategy
{
    private readonly decimal _minimumOrder;
    private readonly IDiscountStrategy _inner;
    public string Description => $"{_inner.Description} (min order ${_minimumOrder:F2})";

    public MinimumOrderDiscount(decimal minimumOrder, IDiscountStrategy inner)
    {
        _minimumOrder = minimumOrder;
        _inner = inner;
    }

    public Money CalculateDiscount(Order order) =>
        order.Subtotal.Amount >= _minimumOrder
            ? _inner.CalculateDiscount(order)
            : new Money(0, "USD");
}

// Composable: apply multiple discounts, take the best
public class BestOfDiscountStrategy : IDiscountStrategy
{
    private readonly IEnumerable<IDiscountStrategy> _strategies;
    public string Description => "Best available discount";

    public BestOfDiscountStrategy(IEnumerable<IDiscountStrategy> strategies) =>
        _strategies = strategies;

    public Money CalculateDiscount(Order order) =>
        _strategies
            .Select(s => s.CalculateDiscount(order))
            .MaxBy(d => d.Amount) ?? new Money(0, "USD");
}
```

### Step 4: Payment Processing (Strategy + Template Method)

```csharp
public record PaymentRequest(decimal Amount, string Currency, string CustomerId);
public record PaymentResult(bool Success, string TransactionId, string ErrorMessage = null);

public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(PaymentRequest request);
    Task<bool> RefundAsync(string transactionId, decimal amount);
    string ProviderName { get; }
}

public class StripePaymentProcessor : IPaymentProcessor
{
    public string ProviderName => "Stripe";

    public async Task<PaymentResult> ProcessAsync(PaymentRequest request)
    {
        // In reality: call Stripe API
        await Task.Delay(100); // simulate network
        Console.WriteLine($"Stripe: Charging ${request.Amount} for customer {request.CustomerId}");
        return new PaymentResult(true, $"stripe_txn_{Guid.NewGuid():N}");
    }

    public async Task<bool> RefundAsync(string transactionId, decimal amount)
    {
        await Task.Delay(50);
        Console.WriteLine($"Stripe: Refunding ${amount} for {transactionId}");
        return true;
    }
}

public class PaymentService
{
    private readonly IPaymentProcessor _processor;
    private readonly ILogger _logger;

    public PaymentService(IPaymentProcessor processor, ILogger logger)
    {
        _processor = processor;
        _logger = logger;
    }

    public async Task<PaymentResult> ChargeOrderAsync(Order order)
    {
        _logger.Log($"Processing payment for order {order.OrderId} via {_processor.ProviderName}");

        var request = new PaymentRequest(order.Total.Amount, order.Total.Currency, order.CustomerId.ToString());
        var result = await _processor.ProcessAsync(request);

        if (result.Success)
        {
            _logger.Log($"Payment successful: {result.TransactionId}");
            order.ConfirmPayment();
        }
        else
        {
            _logger.LogError($"Payment failed: {result.ErrorMessage}", null);
        }

        return result;
    }
}
```

### Step 5: Notification System (Observer + Strategy)

```csharp
public interface IOrderNotifier
{
    Task NotifyAsync(Order order, string eventType, string message);
}

public class EmailOrderNotifier : IOrderNotifier
{
    private readonly string _smtpServer;
    public EmailOrderNotifier(string smtpServer) => _smtpServer = smtpServer;

    public async Task NotifyAsync(Order order, string eventType, string message)
    {
        await Task.Delay(10); // simulate sending
        Console.WriteLine($"EMAIL: Order #{order.OrderId} {eventType} - {message}");
    }
}

public class SmsOrderNotifier : IOrderNotifier
{
    public async Task NotifyAsync(Order order, string eventType, string message)
    {
        await Task.Delay(10);
        Console.WriteLine($"SMS: Order #{order.OrderId} - {message}");
    }
}

// Composite notifier — sends to all channels
public class CompositeOrderNotifier : IOrderNotifier
{
    private readonly List<IOrderNotifier> _notifiers;

    public CompositeOrderNotifier(params IOrderNotifier[] notifiers) =>
        _notifiers = new List<IOrderNotifier>(notifiers);

    public async Task NotifyAsync(Order order, string eventType, string message)
    {
        var tasks = _notifiers.Select(n => n.NotifyAsync(order, eventType, message));
        await Task.WhenAll(tasks);
    }
}
```

### Step 6: Repository Interfaces (DIP + Repository Pattern)

```csharp
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
    Task<int> AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
}

public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task UpdateStockAsync(int productId, int newQuantity);
}
```

### Step 7: Order Service — Orchestration (SRP + DIP)

```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IDiscountStrategy _discountStrategy;
    private readonly PaymentService _paymentService;
    private readonly IOrderNotifier _notifier;
    private readonly ILogger _logger;

    public OrderService(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IDiscountStrategy discountStrategy,
        PaymentService paymentService,
        IOrderNotifier notifier,
        ILogger logger)
    {
        _orderRepo        = orderRepo;
        _productRepo      = productRepo;
        _discountStrategy = discountStrategy;
        _paymentService   = paymentService;
        _notifier         = notifier;
        _logger           = logger;
    }

    public async Task<Order> CreateOrderAsync(int customerId, Address shippingAddress)
    {
        var order = new Order(0, customerId, shippingAddress);
        var orderId = await _orderRepo.AddAsync(order);
        _logger.Log($"Created order {orderId} for customer {customerId}");
        return order;
    }

    public async Task AddItemToOrderAsync(int orderId, int productId, int quantity)
    {
        var order   = await _orderRepo.GetByIdAsync(orderId) ?? throw new KeyNotFoundException($"Order {orderId} not found");
        var product = await _productRepo.GetByIdAsync(productId) ?? throw new KeyNotFoundException($"Product {productId} not found");

        order.AddItem(product, quantity);
        await _orderRepo.UpdateAsync(order);
    }

    public async Task<PaymentResult> SubmitAndPayOrderAsync(int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId) ?? throw new KeyNotFoundException();

        // Apply discount
        var discount = _discountStrategy.CalculateDiscount(order);
        order.ApplyDiscount(discount);
        if (discount.Amount > 0)
            _logger.Log($"Applied discount: {discount} ({_discountStrategy.Description})");

        // Submit order
        order.Submit();
        await _orderRepo.UpdateAsync(order);
        await _notifier.NotifyAsync(order, "Submitted", $"Your order total is {order.Total}");

        // Process payment
        order.MarkPaymentPending();
        await _orderRepo.UpdateAsync(order);

        var paymentResult = await _paymentService.ChargeOrderAsync(order);

        if (paymentResult.Success)
        {
            order.StartProcessing();
            await _orderRepo.UpdateAsync(order);
            await _notifier.NotifyAsync(order, "Confirmed", "Payment confirmed, your order is being processed");
        }
        else
        {
            order.Cancel($"Payment failed: {paymentResult.ErrorMessage}");
            await _orderRepo.UpdateAsync(order);
            await _notifier.NotifyAsync(order, "Cancelled", "Payment failed — your order has been cancelled");
        }

        return paymentResult;
    }

    public async Task ShipOrderAsync(int orderId, string trackingNumber)
    {
        var order = await _orderRepo.GetByIdAsync(orderId) ?? throw new KeyNotFoundException();
        order.MarkShipped(trackingNumber);
        await _orderRepo.UpdateAsync(order);
        await _notifier.NotifyAsync(order, "Shipped", $"Your order has shipped! Tracking: {trackingNumber}");
    }
}
```

### Step 8: Composition Root (Wiring Everything Together)

```csharp
// Program.cs / Startup.cs — this is where you decide WHICH implementations to use
var logger = new ConsoleLogger();

var orderRepo   = new InMemoryOrderRepository(); // or: new SqlOrderRepository(dbContext)
var productRepo = new InMemoryProductRepository();

var paymentProcessor = new StripePaymentProcessor(); // or: new PayPalProcessor()
var paymentService   = new PaymentService(paymentProcessor, logger);

var notifier = new CompositeOrderNotifier(
    new EmailOrderNotifier("smtp.gmail.com"),
    new SmsOrderNotifier()
);

var discountStrategy = new BestOfDiscountStrategy(new IDiscountStrategy[]
{
    new MinimumOrderDiscount(100, new PercentageDiscount(10)),   // 10% off orders over $100
    new MinimumOrderDiscount(200, new FixedAmountDiscount(30)),  // $30 off orders over $200
});

var orderService = new OrderService(
    orderRepo, productRepo, discountStrategy,
    paymentService, notifier, logger
);

// USE THE SYSTEM
var address  = new Address("123 Main St", "San Francisco", "CA", "94105", "USA");
var order    = await orderService.CreateOrderAsync(customerId: 1, address);
await orderService.AddItemToOrderAsync(order.OrderId, productId: 101, quantity: 2);
await orderService.AddItemToOrderAsync(order.OrderId, productId: 205, quantity: 1);
var result = await orderService.SubmitAndPayOrderAsync(order.OrderId);
```

### Design Decisions and Trade-offs

| Decision | Why | Trade-off |
|---|---|---|
| Interfaces for all external dependencies | Testable, swappable | More classes, more setup |
| Repository pattern | Decouple domain from data access | Extra layer, can be over-engineering for small apps |
| Strategy for discounts | OCP — add discounts without changing code | Requires knowing which strategy to inject |
| Composite notifier | Send to all channels transparently | All-or-nothing if one fails (needs resilience) |
| Order as Aggregate | Enforces business invariants centrally | Large objects, more coordination needed |
| Value objects (Money, Address) | Immutability, no accidental sharing | Extra allocations vs simple primitives |

### How This Scales in Production

```
SCALING CONSIDERATIONS:

1. CONCURRENCY: Two customers ordering the last item
   → Use optimistic locking (EF Core concurrency tokens)
   → Order.AddItem calls Product.ReserveStock (DB transaction)

2. PAYMENT FAILURES: Stripe times out mid-transaction
   → Implement saga pattern / outbox pattern
   → Record intent before calling external service
   → Idempotency keys for retries

3. HIGH ORDER VOLUME:
   → Order submission → Message queue (RabbitMQ/Azure Service Bus)
   → Payment service is a separate microservice
   → Notifications sent asynchronously after payment confirmed

4. REPORTING:
   → CQRS: separate read models for reporting
   → Don't query the same Order aggregate for analytics
   → Event sourcing: store state changes as events
```

---

# PART 8: INTERVIEW MASTERY

---

## Chapter 12: Top Interview Questions with Model Answers

### OOP Fundamentals

**Q: What are the four pillars of OOP?**
> Encapsulation (bundling data + behavior, hiding internals), Abstraction (exposing only necessary interface), Inheritance (reusing behavior through parent-child class relationships), and Polymorphism (objects of different types responding to the same interface in type-specific ways).

**Q: What is the difference between abstract class and interface in C#?**
> An abstract class can have fields, constructors, and partial implementation. A class can only inherit one abstract class. Use it when sharing implementation among closely related types. An interface is a pure contract with no fields or constructors. A class can implement multiple interfaces. Use it when defining capabilities across unrelated types.

**Q: Explain the SOLID principles.**
> S — Single Responsibility: one class, one reason to change. O — Open/Closed: extend by adding new code, not modifying existing. L — Liskov Substitution: derived types must be usable wherever base types are. I — Interface Segregation: many small interfaces beat one large one. D — Dependency Inversion: depend on abstractions (interfaces), not concrete classes; inject dependencies from outside.

**Q: What is Dependency Injection?**
> A design pattern where a class receives its dependencies from external code rather than creating them internally. Enables loose coupling, testability, and flexibility. Supported in .NET via built-in DI container (IServiceCollection), where you register implementations against interfaces and the framework injects them via constructors.

**Q: What is the difference between IS-A and HAS-A?**
> IS-A is inheritance: a Car IS-A Vehicle. HAS-A is composition: a Car HAS-A Engine. Prefer HAS-A (composition) when you just want to reuse behavior — it's more flexible and avoids tight coupling. Use IS-A when there's a true specialization relationship.

**Q: What is method hiding vs method overriding?**
> Overriding (`virtual`/`override`) replaces a method polymorphically — the derived version is called even through a base reference. Hiding (`new` keyword) replaces the method only when called through a derived reference; the base reference still calls the base method.

**Q: What is a sealed class? When would you use it?**
> A sealed class cannot be inherited. Use it when: the class's implementation relies on assumptions that inheritance would break, for performance (JIT can devoptimize), or to prevent accidental misuse. `string` in .NET is sealed. The Singleton pattern's class is typically sealed.

### Design Patterns

**Q: What design patterns have you used in production?**
> Model answer: "In my WPF applications I regularly used the MVVM pattern (Command pattern for user actions, Observer pattern via INotifyPropertyChanged for data binding). For data access I used the Repository pattern to decouple business logic from EF Core. For cross-cutting concerns like logging I used the Decorator pattern. I've also used the Singleton pattern for shared configuration objects, though I now prefer DI containers for managing lifetime."

**Q: What is the difference between Factory Method and Abstract Factory?**
> Factory Method: one abstract method for creating one product type; subclasses decide the concrete type. Abstract Factory: an interface with multiple factory methods for creating *families* of related products. Use Abstract Factory when you need to ensure the created objects work together (e.g., UI controls must all be Windows-style or all Mac-style).

### Performance and Best Practices

**Q: When does OOP hurt performance?**
> Virtual dispatch has a small cost compared to direct method calls (VTable lookup). Excessive object creation (lots of small short-lived objects) increases GC pressure. Deep inheritance hierarchies can hurt CPU cache performance. In performance-critical paths, consider: structs instead of classes (value type, stack-allocated), sealing frequently-called classes, avoiding virtual dispatch in tight loops.

**Q: How do you make OOP code testable?**
> 1) Depend on interfaces, not concrete classes. 2) Inject dependencies via constructor. 3) Avoid static methods with side effects or global state. 4) Keep classes small (SRP). 5) Use the Repository pattern to abstract data access. With these in place, you can inject mock/fake implementations in tests.

---

# PART 9: ADVANCED TOPICS

---

## Chapter 13: Advanced OOP Concepts

### Covariance and Contravariance

```csharp
// COVARIANCE (out): return type can be more derived
public interface IProducer<out T>
{
    T Produce();
}

// CONTRAVARIANCE (in): parameter type can be more general
public interface IConsumer<in T>
{
    void Consume(T item);
}

// Usage:
IProducer<Dog> dogProducer = new DogFactory();
IProducer<Animal> animalProducer = dogProducer; // Valid — Dog IS-A Animal (covariant)

IConsumer<Animal> animalConsumer = new AnimalProcessor();
IConsumer<Dog> dogConsumer = animalConsumer; // Valid — consumer of Animal can handle Dog (contravariant)
```

### Generics and Type Constraints

```csharp
// Type constraints enforce what T can be
public class Repository<T, TId>
    where T : class, IEntity<TId>    // T must be a class implementing IEntity
    where TId : struct, IComparable  // TId must be a value type (int, Guid...)
{
    private readonly Dictionary<TId, T> _store = new();

    public T GetById(TId id) => _store.TryGetValue(id, out var entity) ? entity : null;
    public void Save(T entity) => _store[entity.Id] = entity;
}

public interface IEntity<TId>
{
    TId Id { get; }
}
```

### Immutability and Records

```csharp
// C# record — immutable by default, value semantics, with-expressions
public record CustomerProfile(int Id, string Name, string Email, Address Address);

var customer = new CustomerProfile(1, "Deepak", "deepak@example.com",
    new Address("123 Main", "SF", "CA", "94105", "USA"));

// Non-destructive mutation — creates a new instance
var updatedCustomer = customer with { Email = "newemail@example.com" };

// Records have structural equality (not reference equality)
var c1 = new CustomerProfile(1, "Deepak", "d@e.com", ...);
var c2 = new CustomerProfile(1, "Deepak", "d@e.com", ...);
Console.WriteLine(c1 == c2); // True — same values, even though different instances
```

### Extension Methods

```csharp
// Add methods to existing types without inheritance
public static class StringExtensions
{
    public static bool IsValidEmail(this string str) =>
        System.Text.RegularExpressions.Regex.IsMatch(str, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static string Truncate(this string str, int maxLength, string suffix = "...") =>
        str.Length <= maxLength ? str : str[..(maxLength - suffix.Length)] + suffix;
}

// Usage — reads like a method ON string
"deepak@example.com".IsValidEmail()          // true
"A very long title that needs cutting".Truncate(20) // "A very long title..."
```

### Object Lifecycle and IDisposable

```csharp
// IDisposable — for deterministic cleanup of unmanaged resources
public class DatabaseConnection : IDisposable
{
    private SqlConnection _connection;
    private bool _disposed = false;

    public DatabaseConnection(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
    }

    public void ExecuteQuery(string sql) { /* ... */ }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Close();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }

    // Finalizer as safety net (only if you have unmanaged resources)
    ~DatabaseConnection() => Dispose(false);
}

// Always use with using statement:
using var conn = new DatabaseConnection(connectionString);
conn.ExecuteQuery("SELECT * FROM Orders");
// conn.Dispose() called automatically at end of scope
```

---

## Chapter 14: Common Pitfalls and How to Avoid Them

### Pitfall 1: God Class

```csharp
// ANTI-PATTERN: One class that does everything
public class OrderManager // 3000 lines long
{
    public Order CreateOrder() { }
    public void SendEmail() { }
    public void WriteToDatabase() { }
    public void CalculateTax() { }
    public void GeneratePDF() { }
    public void UpdateInventory() { }
    // ... 50 more methods
}

// FIX: Split by responsibility (see Chapter 11 project)
```

### Pitfall 2: Anemic Domain Model

```csharp
// ANTI-PATTERN: Classes with only getters/setters, no behavior
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal Total { get; set; }    // Should be computed
    public string Status { get; set; }    // Should be controlled state machine
}
// All business logic ends up in service classes — not OOP, just data bags

// FIX: Domain objects enforce their own rules (see Order class in Chapter 11)
```

### Pitfall 3: Deep Inheritance Hierarchies

```csharp
// ANTI-PATTERN
Animal → Mammal → Canine → Dog → GoldenRetriever → GuideGoldenRetriever → TrainedGuideGoldenRetriever

// FIX: Flatten with composition and interfaces
public class Animal
{
    public IMovementBehavior Movement { get; set; }
    public IFeedingBehavior Feeding { get; set; }
    public ITrainingBehavior Training { get; set; }
}
// Compose behavior instead of inheriting it
```

### Pitfall 4: Breaking Encapsulation Through Return Types

```csharp
// BAD: Returning internal mutable collection
public List<OrderItem> GetItems() => _items; // Caller can add/remove items!

// GOOD: Return read-only view or copy
public IReadOnlyList<OrderItem> GetItems() => _items.AsReadOnly();
```

### Pitfall 5: Overusing Inheritance for Code Reuse

```csharp
// BAD: Inheriting just to reuse methods
public class EmailSender { public void Send(string to, string msg) { } }
public class UserService : EmailSender { } // UserService IS-A EmailSender? No!

// GOOD: Inject the dependency
public class UserService
{
    private readonly IEmailSender _emailSender; // Has-A, not Is-A
    public UserService(IEmailSender sender) => _emailSender = sender;
}
```

---

## Quick Reference: OOP Decision Guide

```
WHEN TO USE WHAT:

┌─ Need to share code across related types?
│   └─ Abstract class (with implementation)
│
├─ Need to define a contract across unrelated types?
│   └─ Interface
│
├─ IS-A relationship with genuine specialization?
│   └─ Inheritance (but prefer composition if in doubt)
│
├─ HAS-A relationship or code reuse without IS-A?
│   └─ Composition
│
├─ Need to swap algorithms or behaviors at runtime?
│   └─ Strategy pattern
│
├─ Need to add behavior without modifying existing class?
│   └─ Decorator pattern
│
├─ Need to notify multiple objects of state changes?
│   └─ Observer pattern / C# events
│
├─ Need to hide complex object creation?
│   └─ Factory Method / Abstract Factory / Builder
│
├─ Need undo/redo or operation queuing?
│   └─ Command pattern
│
└─ Depending on concrete classes?
    └─ Invert the dependency — create an interface and inject it
```

---

## Learning Roadmap

| Week | Focus | Goal |
|---|---|---|
| 1 | Classes, Objects, Encapsulation | Understand data hiding and valid state |
| 2 | Abstraction, Interfaces | Separate contract from implementation |
| 3 | Inheritance, Polymorphism | Understand when and when NOT to inherit |
| 4 | Composition, Relationships | Prefer composition — build the project |
| 5 | SOLID Principles | Refactor the project through SOLID lens |
| 6 | Creational Patterns | Factory, Builder, Singleton |
| 7 | Structural Patterns | Decorator, Adapter, Facade |
| 8 | Behavioral Patterns | Observer, Strategy, Command |
| 9 | Advanced Topics | Generics, covariance, IDisposable |
| 10 | Interview Prep | Mock interviews, explain concepts aloud |

---

*Practice rule: for every concept, build something small with it within 24 hours. You'll retain 80% more than reading alone.*
