// ============================================================
// CONCEPT 7: INTERFACES — Pure Contracts
// ============================================================
//
// ANALOGY: A power socket is an interface. It defines: "any plug that
//          fits this socket shape can receive electricity." The socket
//          doesn't care if the plug powers a phone or a laptop.
//
// DEFINITION: An interface is a CONTRACT — a list of members a class
//             PROMISES to implement. No implementation, just the "what".
//
// KEY DIFFERENCES vs ABSTRACT CLASS:
//  ┌─────────────────────┬──────────────────┬────────────────────┐
//  │ Feature             │ Abstract Class   │ Interface          │
//  ├─────────────────────┼──────────────────┼────────────────────┤
//  │ Instantiate?        │ No               │ No                 │
//  │ Multiple?           │ No (one parent)  │ Yes (many!)        │
//  │ Has fields?         │ Yes              │ No                 │
//  │ Constructors?       │ Yes              │ No                 │
//  │ Default impl?       │ Yes              │ Yes (C# 8+)        │
//  │ Access modifiers?   │ Yes              │ All public by def. │
//  │ Use for?            │ IS-A + shared    │ CAN-DO contracts   │
//  └─────────────────────┴──────────────────┴────────────────────┘
//
//  Rule of thumb:
//  - Abstract class = shared code + forced override  (IS-A)
//  - Interface      = capability contract             (CAN-DO)
//  Example: A Bird IS-A Animal (abstract) AND CAN-DO IFlyable (interface)

namespace OOP;

// ----- DEFINING INTERFACES -----
// Convention: name starts with I
// All members are public by default — no access modifier needed

public interface ISaveable
{
    void Save();       // method contract
    bool Load(int id); // method contract
    string StorageKey { get; } // property contract
}

public interface IPrintable
{
    void Print();
    string GetPrintPreview();
}

public interface IValidatable
{
    bool IsValid();
    IEnumerable<string> GetValidationErrors();
}

// C# 8+ DEFAULT INTERFACE METHODS
// Provide a fallback implementation — implementing classes can optionally override
public interface ILoggable
{
    string GetLogMessage(); // must implement

    // Default method — optional to override
    void Log() => Console.WriteLine($"  [LOG] {GetLogMessage()}"); // default impl
}

// ----- A CLASS CAN IMPLEMENT MULTIPLE INTERFACES -----
// This is C#'s way of achieving "multiple inheritance of behavior"
public class Invoice : ISaveable, IPrintable, IValidatable, ILoggable
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    // --- ISaveable ---
    public string StorageKey => $"Invoice_{Id}"; // property from interface

    public void Save()
    {
        Console.WriteLine($"  [Invoice] Saving to DB: {StorageKey}");
        // Real code: dbContext.Invoices.Add(this); dbContext.SaveChanges();
    }

    public bool Load(int id)
    {
        Console.WriteLine($"  [Invoice] Loading invoice #{id} from DB");
        return true; // simulate success
    }

    // --- IPrintable ---
    public void Print()
    {
        Console.WriteLine($"  [Invoice] Printing Invoice #{Id} for {CustomerName}");
        Console.WriteLine($"  Amount: {Amount:C} | Date: {Date:d}");
    }

    public string GetPrintPreview() =>
        $"Invoice #{Id} | {CustomerName} | {Amount:C}";

    // --- IValidatable ---
    public bool IsValid() => !GetValidationErrors().Any();

    public IEnumerable<string> GetValidationErrors()
    {
        if (string.IsNullOrWhiteSpace(CustomerName)) yield return "CustomerName is required";
        if (Amount <= 0) yield return "Amount must be positive";
        if (Id <= 0) yield return "Id must be a positive number";
    }

    // --- ILoggable ---
    public string GetLogMessage() => $"Invoice #{Id} for {CustomerName} ({Amount:C})";
    // Note: Log() from ILoggable's default implementation is used automatically
    // unless we override it here
}

// ----- EXPLICIT INTERFACE IMPLEMENTATION -----
// When two interfaces have the same method name/signature,
// you must implement them explicitly to avoid ambiguity.
// Explicit implementations are accessible ONLY via the interface type.

public interface IReader
{
    string Read(); // returns content
}

public interface IWriter
{
    string Write(); // coincidentally same name+return
}

public class FileHandler : IReader, IWriter
{
    private string _content = "file content";

    // Explicit: `IReader.Read` — only callable via IReader reference
    string IReader.Read()
    {
        Console.WriteLine("  [IReader.Read] Reading file...");
        return _content;
    }

    // Explicit: `IWriter.Write` — only callable via IWriter reference
    string IWriter.Write()
    {
        Console.WriteLine("  [IWriter.Write] Writing file...");
        return "write result";
    }
}

// ----- INTERFACE AS A TYPE -----
// You can use an interface as a variable/parameter type
// — this is what makes code flexible and testable

public class NotificationService
{
    // Depends on the ISaveable contract, not a concrete class
    // Could be Invoice, Receipt, Report — anything that implements ISaveable
    public void SaveAll(IEnumerable<ISaveable> items)
    {
        foreach (var item in items)
        {
            if (item is IValidatable v)
            {
                if (!v.IsValid())
                {
                    Console.WriteLine($"  Skipping invalid item: {item.StorageKey}");
                    continue;

                }
            }
            item.Save();
        }
    }
}

public static class InterfacesDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 7. INTERFACES ==========");

        var invoice = new Invoice { Id = 101, CustomerName = "Deepak", Amount = 1500m };

        Console.WriteLine("\n-- Implementing ISaveable:");
        invoice.Save();
        invoice.Load(101);

        Console.WriteLine("\n-- Implementing IPrintable:");
        invoice.Print();
        Console.WriteLine($"  Preview: {invoice.GetPrintPreview()}");

        Console.WriteLine("\n-- Implementing IValidatable:");
        Console.WriteLine($"  Is valid: {invoice.IsValid()}");

        var badInvoice = new Invoice { Id = -1, Amount = -50 };
        Console.WriteLine($"  Bad invoice valid: {badInvoice.IsValid()}");
        foreach (var err in badInvoice.GetValidationErrors())
            Console.WriteLine($"  Error: {err}");

        Console.WriteLine("\n-- Default interface method (ILoggable.Log):");
        // Default interface methods are only accessible via the interface type
        ((ILoggable)invoice).Log(); // cast to ILoggable to call the default impl

        Console.WriteLine("\n-- Interface as type (polymorphism across unrelated classes):");
        var service = new NotificationService();
        var items = new List<ISaveable> { invoice, badInvoice };
        service.SaveAll(items); // skips invalid, saves valid

        Console.WriteLine("\n-- Explicit interface implementation:");
        var handler = new FileHandler();
        // handler.Read() // COMPILE ERROR — explicit impls not accessible on class type

        IReader reader = handler; // must cast to interface
        IWriter writer = handler;
        Console.WriteLine($"  Reader result: {reader.Read()}");
        Console.WriteLine($"  Writer result: {writer.Write()}");

        Console.WriteLine("\n-- `is` check for interface:");
        object obj = invoice;
        if (obj is IPrintable printable)
        {
            Console.WriteLine($"  obj implements IPrintable — preview: {printable.GetPrintPreview()}");
        }

        // KEY TAKEAWAYS:
        // Interface = contract (what), not implementation (how)
        // A class CAN implement many interfaces (solves multiple inheritance)
        // Use interface for parameters/return types → loose coupling, testability
        // Explicit impl = resolves name conflicts, hides members unless cast to interface
        // C# 8+ default methods = add new members without breaking existing implementors
    }
}
