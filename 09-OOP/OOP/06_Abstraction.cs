// ============================================================
// CONCEPT 6: ABSTRACTION — Hiding Complexity, Showing Essentials
// ============================================================
//
// ANALOGY: When you drive a car, you use the steering wheel and pedals.
//          You don't know (or need to know) how the engine combustion works.
//          The car ABSTRACTS away the complexity.
//
// DEFINITION: Abstraction = exposing WHAT something does,
//             hiding HOW it does it.
//
// In C#, abstraction is achieved via:
//   1. Abstract Classes  (partial abstraction — can have some implementation)
//   2. Interfaces        (full abstraction — only contracts, no implementation) ← next file
//
// ABSTRACT CLASS RULES:
//  - Cannot be instantiated (new AbstractClass() = ERROR)
//  - Can have abstract methods (no body — MUST be overridden by derived class)
//  - Can have regular methods (with body — inherited as-is)
//  - Can have fields, properties, constructors
//  - A derived class MUST override ALL abstract methods (or also be abstract)

namespace OOP;

// ----- ABSTRACT BASE CLASS -----
// Models the concept of a "notification" without specifying HOW to send it.
public abstract class Notification
{
    // Regular property — concrete, shared by all notifications
    public string Recipient { get; protected set; }
    public string Message { get; protected set; }
    public DateTime SentAt { get; private set; }

    // Constructor in abstract class — derived classes chain to it with : base(...)
    protected Notification(string recipient, string message)
    {
        Recipient = recipient;
        Message = message;
    }

    // ABSTRACT METHOD — no body, no implementation here
    // Forces every derived class to provide its own "how to send" logic
    // Think of it as: "I know you CAN send, but HOW is up to you"
    public abstract void Send();

    // ABSTRACT PROPERTY — derived class must implement a getter
    public abstract string Channel { get; }

    // REGULAR (concrete) METHOD — shared logic all notifications need
    // Calls Send() internally — which resolves to the derived class's version (polymorphism!)
    public void SendWithLog()
    {
        Console.WriteLine($"  Preparing {Channel} notification for {Recipient}...");
        Send(); // polymorphic call — goes to the derived class
        SentAt = DateTime.Now;
        Console.WriteLine($"  Logged: sent at {SentAt:HH:mm:ss}");
    }

    // Virtual method — has a default, but CAN be overridden
    public virtual string GetSummary() => $"[{Channel}] → {Recipient}: \"{Message}\"";
}

// ----- CONCRETE DERIVED CLASSES -----
// Each one provides the "how" for its specific channel

public class EmailNotification : Notification
{
    public string Subject { get; private set; }

    public EmailNotification(string recipient, string subject, string message)
        : base(recipient, message)
    {
        Subject = subject;
    }

    // Must implement — this is the EmailNotification's specific "how"
    public override void Send()
    {
        Console.WriteLine($"  [EMAIL] To: {Recipient} | Subject: {Subject} | Body: {Message}");
        // Real code: smtpClient.Send(...)
    }

    public override string Channel => "Email";

    public override string GetSummary() => $"[Email] {Subject} → {Recipient}";
}

public class SmsNotification : Notification
{
    private string _phoneNumber;

    public SmsNotification(string phoneNumber, string message)
        : base(phoneNumber, message)
    {
        _phoneNumber = phoneNumber;
    }

    public override void Send()
    {
        // Truncate SMS to 160 chars (real-world constraint)
        string sms = Message.Length > 160 ? Message[..160] + "..." : Message;
        Console.WriteLine($"  [SMS] To: {_phoneNumber} | Text: {sms}");
        // Real code: twilioClient.Messages.Create(...)
    }

    public override string Channel => "SMS";
}

public class PushNotification : Notification
{
    private string _deviceToken;

    public PushNotification(string deviceToken, string message)
        : base(deviceToken, message)
    {
        _deviceToken = deviceToken;
    }

    public override void Send()
    {
        Console.WriteLine($"  [PUSH] Token: {_deviceToken[..8]}... | Alert: {Message}");
        // Real code: firebaseClient.Send(...)
    }

    public override string Channel => "Push";
}

// ----- ABSTRACT CLASS CAN HAVE ABSTRACT + CONCRETE MIXED -----
// Here's a template method pattern — a classic use of abstract classes
public abstract class DataProcessor
{
    // Template method — defines the ALGORITHM SKELETON
    // Derived classes fill in the blanks via abstract methods
    public void Process(string data)
    {
        string validated = Validate(data);   // step 1: abstract
        string transformed = Transform(validated); // step 2: abstract
        Save(transformed);                   // step 3: concrete (shared)
        Console.WriteLine($"  [{GetType().Name}] Processing complete.");
    }

    // Abstract steps — each processor defines its own logic
    protected abstract string Validate(string input);
    protected abstract string Transform(string input);

    // Concrete step — same for all processors
    private void Save(string data) => Console.WriteLine($"  Saving: {data}");
}

public class JsonProcessor : DataProcessor
{
    protected override string Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Empty JSON");
        return input.Trim();
    }
    protected override string Transform(string input) => $"{{\"data\": \"{input}\"}}";
}

public class CsvProcessor : DataProcessor
{
    protected override string Validate(string input)
    {
        if (!input.Contains(',')) throw new FormatException("Not valid CSV");
        return input;
    }
    protected override string Transform(string input) =>
        string.Join(" | ", input.Split(','));
}

public static class AbstractionDemo
{
    public static void Run()
    {
        Console.WriteLine("\n========== 6. ABSTRACTION ==========");

        // Cannot do: var n = new Notification(...); // ERROR — abstract class!

        Console.WriteLine("\n-- Notifications via abstract class:");
        List<Notification> notifications = new()
        {
            new EmailNotification("deepak@gmail.com", "Welcome!", "Hello Deepak, welcome aboard."),
            new SmsNotification("+91-9876543210", "Your OTP is 4521"),
            new PushNotification("device-token-abc123xyz", "You have a new message!")
        };

        // Call SendWithLog on each — the concrete Send() is called polymorphically
        // This code doesn't need to change when you add SlackNotification, WhatsAppNotification, etc.
        foreach (var notification in notifications)
        {
            notification.SendWithLog();
            Console.WriteLine($"  Summary: {notification.GetSummary()}");
            Console.WriteLine();
        }

        Console.WriteLine("-- Template Method Pattern (abstract algorithm skeleton):");
        DataProcessor json = new JsonProcessor();
        json.Process("hello world");

        DataProcessor csv = new CsvProcessor();
        csv.Process("Deepak,28,Engineer");

        // KEY TAKEAWAYS:
        // Abstract class = partial blueprint (defines structure, some implementation)
        // Abstract method = "you MUST do this, but do it YOUR way"
        // Cannot instantiate abstract class — it's a concept, not a thing
        // Perfect for: template method pattern, shared behavior with forced overrides
        //
        // WHEN TO USE ABSTRACT CLASS vs INTERFACE (preview):
        // Abstract class: when derived classes share code/state AND have an IS-A relationship
        // Interface:       when you need a contract across unrelated classes (covered next)
    }
}
