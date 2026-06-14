// ============================================================
// FILE: Ch01_ClassesObjects.cs
// PURPOSE: Teach Classes and Objects from absolute scratch.
//          This is the atom of all OOP. Everything else builds here.
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO CLASSES (actual C# that runs inside the app) ───────
    // These are INSIDE the chapter file — you can see the code AND
    // watch it execute right here in the app. That's intentional.

    // A simple BankAccount class demonstrating the core concepts
    internal class BankAccount
    {
        // FIELDS: private storage for this object's data.
        // 'private' = only code INSIDE this class can read/write these.
        // '_' prefix is a C# naming convention for private backing fields.
        private string _owner;
        private decimal _balance;

        // PROPERTY: a controlled window into a private field.
        // 'get' returns the value. No 'set' = read-only from outside.
        public string Owner => _owner;
        public decimal Balance => _balance;

        // CONSTRUCTOR: runs once, the moment a new object is created.
        // Its job: set the object into a VALID initial state.
        // If you can't construct a valid object, throw immediately.
        public BankAccount(string owner, decimal initialBalance)
        {
            // Guard clause: reject invalid input BEFORE storing it.
            // Never let an object exist in an invalid state.
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner name cannot be empty.");
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.");

            _owner   = owner;
            _balance = initialBalance;
        }

        // METHOD: an action this object can perform.
        // Methods are verbs; properties and fields are nouns.
        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Deposit amount must be positive.");
            _balance += amount;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Withdrawal amount must be positive.");
            if (amount > _balance) return false; // Insufficient funds — signal failure, don't throw
            _balance -= amount;
            return true;
        }

        // override ToString: every class inherits this from 'object'.
        // Overriding it gives a meaningful text representation.
        public override string ToString() => $"[{_owner}] Balance: ${_balance:F2}";
    }

    // A static class demonstrating CLASS-LEVEL (vs instance-level) members
    internal static class MathUtils
    {
        // STATIC FIELD: shared by all — not per-object
        public static int CallCount { get; private set; } = 0;

        // STATIC METHOD: belongs to the class, not any object.
        // No 'new MathUtils()' needed — call it as MathUtils.Square(5)
        public static double Square(double n)
        {
            CallCount++;
            return n * n;
        }
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch01_ClassesObjects
    {
        public static void Run()
        {
            UI.Header("CHAPTER 1: CLASSES & OBJECTS — THE BLUEPRINT AND THE THING");

            UI.Concept(
                "Classes and Objects are the FOUNDATION of all OOP. Every other concept — " +
                "encapsulation, inheritance, polymorphism — sits on top of this. " +
                "Get this right and everything else will make sense."
            );

            UI.Pause();
            Section1_WhatAndWhy();
            Section2_ClassAnatomy();
            Section3_ObjectsInMemory();
            Section4_ConstructorDeep();
            Section5_PropertiesVsFields();
            Section6_StaticVsInstance();
            Section7_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_WhatAndWhy()
        {
            UI.SubHeader("What is a Class? What is an Object?");

            UI.Analogy("Cookie Cutter and Cookies",
                "A CLASS is the cookie cutter. It defines the SHAPE of what you'll make. " +
                "It lives in your drawer — it takes up no space for actual cookies. " +
                "An OBJECT is an actual cookie you stamped out using that cutter. " +
                "You can make 100 cookies (objects) from one cutter (class). " +
                "Each cookie is separate — if you eat one, the others are fine. " +
                "Each cookie can have different toppings (data), but the same shape (structure)."
            );

            UI.Diagram("Class (Blueprint)  vs  Objects (Instances in Memory)",
                "  CLASS BankAccount (exists at DESIGN TIME — just a definition):",
                "  ┌────────────────────────────────────────┐",
                "  │  Fields:   _owner, _balance            │",
                "  │  Methods:  Deposit(), Withdraw()       │",
                "  └────────────────────────────────────────┘",
                "",
                "  OBJECTS (exist at RUNTIME — memory allocated, have real values):",
                "  ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐",
                "  │ deepakAccount    │   │ johnAccount      │   │ maryAccount      │",
                "  │ _owner='Deepak'  │   │ _owner='John'    │   │ _owner='Mary'    │",
                "  │ _balance=5500    │   │ _balance=12000   │   │ _balance=250     │",
                "  └──────────────────┘   └──────────────────┘   └──────────────────┘"
            );

            UI.Print("WHY DO CLASSES EXIST?\n");
            UI.Print("Before OOP, code and data were separate. You'd have:");
            UI.Code("Procedural style — data and behavior scattered everywhere",
                "string owner = \"Deepak\";",
                "decimal balance = 5000;",
                "",
                "// Anyone anywhere could do this — no protection:",
                "balance = -999999;  // Object now in invalid state!",
                "",
                "// Every function needed all data passed in explicitly:",
                "void PrintAccount(string owner, decimal balance) { ... }",
                "void Deposit(ref decimal balance, decimal amount) { ... }",
                "void Withdraw(ref decimal balance, decimal amount) { ... }",
                "",
                "// Now add a SECOND account... all the variables need new names.",
                "string owner2 = \"John\";",
                "decimal balance2 = 12000;",
                "// This doesn't scale. 100 accounts = 200 loose variables. Chaos."
            );

            UI.Concept(
                "OOP solves this by GROUPING the data (_owner, _balance) and the " +
                "behavior (Deposit, Withdraw) that operates on that data INTO ONE UNIT: a class. " +
                "Now the account OWNS its data and controls how it changes."
            );

            UI.KeyPoint("A class bundles related data (what an object IS) with related behavior (what an object DOES).");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_ClassAnatomy()
        {
            UI.SubHeader("Anatomy of a Class — Every Part Explained");

            UI.Code("Complete BankAccount class — every element labelled",
                "// 'public'  = accessible from any other code",
                "// 'class'   = this defines a class (a blueprint)",
                "// 'BankAccount' = the class name (PascalCase by convention)",
                "public class BankAccount",
                "{",
                "    // ── FIELDS ──────────────────────────────────────────",
                "    // Fields store the OBJECT'S DATA (state).",
                "    // Private: no external code can directly read or write these.",
                "    // Prefix '_' tells readers: this is a private backing field.",
                "    private string _owner;    // Who owns this account",
                "    private decimal _balance; // Current balance (decimal = exact money math)",
                "",
                "    // ── PROPERTIES ──────────────────────────────────────",
                "    // Properties are controlled windows into private fields.",
                "    // 'get' block: returns the value when someone reads this property.",
                "    // No 'set' block: this property is READ-ONLY from outside the class.",
                "    public string  Owner   => _owner;   // Arrow syntax: shortcut for { get { return _owner; } }",
                "    public decimal Balance => _balance;",
                "",
                "    // ── CONSTRUCTOR ─────────────────────────────────────",
                "    // Special method: runs ONCE when 'new BankAccount(...)' is called.",
                "    // Name must EXACTLY match the class name.",
                "    // No return type (not even void).",
                "    // Job: initialize the object to a VALID state.",
                "    public BankAccount(string owner, decimal initialBalance)",
                "    {",
                "        // Guard clauses: reject bad input BEFORE storing anything.",
                "        if (string.IsNullOrWhiteSpace(owner))",
                "            throw new ArgumentException(\"Name required.\");",
                "        if (initialBalance < 0)",
                "            throw new ArgumentException(\"Balance can't start negative.\");",
                "",
                "        _owner   = owner;          // Store in private field",
                "        _balance = initialBalance;",
                "    }",
                "",
                "    // ── METHODS ─────────────────────────────────────────",
                "    // Methods define what actions an object can perform.",
                "    // 'public' = callable by outside code.",
                "    // 'void' = returns nothing.",
                "    public void Deposit(decimal amount)",
                "    {",
                "        if (amount <= 0) throw new ArgumentException(\"Must be positive.\");",
                "        _balance += amount; // Modify private field through controlled method",
                "    }",
                "",
                "    // 'bool' return type: tells the caller if the operation succeeded.",
                "    // Better than throwing an exception for a business rule like 'not enough funds'.",
                "    public bool Withdraw(decimal amount)",
                "    {",
                "        if (amount <= 0 || amount > _balance) return false;",
                "        _balance -= amount;",
                "        return true;",
                "    }",
                "}"
            );

            UI.Print("KEY VOCABULARY:\n");
            var vocab = new[]
            {
                ("Class",       "The blueprint. Exists in source code. No memory for data."),
                ("Object",      "An instance of a class. Exists at runtime. Has real data in memory."),
                ("Field",       "A variable stored inside an object. Private by default."),
                ("Property",    "A controlled accessor for a field. Can have get/set with logic."),
                ("Constructor", "Special method that runs once at object creation. Initializes state."),
                ("Method",      "A function that belongs to a class. Can read and modify the object's fields."),
                ("Instance",    "Another word for 'object'. 'Create an instance' = 'new ClassName()'"),
            };

            foreach (var (term, def) in vocab)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  {term,-16}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(def);
            }
            Console.ResetColor();
            Console.WriteLine();
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_ObjectsInMemory()
        {
            UI.SubHeader("Objects in Memory — The Heap, the Stack, and References");

            UI.Concept(
                "When you write 'new BankAccount(...)', the C# runtime does two things:\n" +
                "  1. Allocates a block of memory on the HEAP for the object's data.\n" +
                "  2. Returns a REFERENCE (memory address) that you store in a variable.\n" +
                "Your variable doesn't HOLD the object — it POINTS TO it."
            );

            UI.Diagram("Memory Layout",
                "  STACK (where local variables live):         HEAP (where objects live):",
                "  ┌────────────────────────┐                ┌──────────────────────────────┐",
                "  │ deepakAccount ─────────┼──────────────▶ │  BankAccount object          │",
                "  │ (holds memory address) │                │  _owner   = \"Deepak\"        │",
                "  └────────────────────────┘                │  _balance = 5500             │",
                "                                            └──────────────────────────────┘",
                "",
                "  BankAccount copy = deepakAccount;  // Both point to SAME object!",
                "  ┌────────────────────────┐",
                "  │ deepakAccount ─────────┼──────────────▶ ┌──────────────────────────┐",
                "  │ copy          ─────────┼──────────────▶ │  SAME BankAccount object │",
                "  └────────────────────────┘                └──────────────────────────┘",
                "  // copy.Deposit(100) ALSO changes deepakAccount's balance!"
            );

            UI.LiveDemo("Two variables pointing to the same object", () =>
            {
                var acc1 = new BankAccount("Deepak", 1000);
                var acc2 = acc1;  // acc2 points to the SAME object in memory

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     acc1 = {acc1}");
                Console.WriteLine($"     acc2 = {acc2}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("     acc2.Deposit(500);  // Depositing through acc2...");
                Console.ResetColor();

                acc2.Deposit(500);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"     acc1 after acc2.Deposit: {acc1}");
                Console.WriteLine("     ↑  acc1 ALSO changed! Both variables point to the same object.");
                Console.ResetColor();
            });

            UI.Mistake(
                "Thinking that assigning an object to a new variable creates a COPY of it. " +
                "'BankAccount copy = original' makes TWO references to ONE object.",
                "To truly copy an object, you must implement a copy constructor or Clone method. " +
                "For value types (struct, int, decimal), assignment DOES copy the value."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_ConstructorDeep()
        {
            UI.SubHeader("Constructors — Deep Dive");

            UI.Concept(
                "The constructor is a guarantee: once it finishes, the object is READY TO USE. " +
                "If you can't create a valid object from the given input, throw an exception. " +
                "NEVER let an object exist in an invalid or incomplete state."
            );

            UI.Code("Three constructor patterns you'll use constantly",
                "public class Order",
                "{",
                "    public int OrderId { get; }",
                "    public DateTime CreatedAt { get; }",
                "    public string Status { get; private set; }",
                "",
                "    // ── PATTERN 1: Standard constructor ─────────────────",
                "    // Validates input and sets all required fields.",
                "    public Order(int orderId)",
                "    {",
                "        if (orderId <= 0) throw new ArgumentException(\"Invalid ID\");",
                "        OrderId   = orderId;",
                "        CreatedAt = DateTime.UtcNow;  // Always use UTC for server-side dates",
                "        Status    = \"Draft\";",
                "    }",
                "",
                "    // ── PATTERN 2: Constructor chaining with 'this()' ────",
                "    // Calls the primary constructor, then adds extra setup.",
                "    // Avoids code duplication between constructors.",
                "    public Order(int orderId, string initialStatus) : this(orderId)",
                "    {",
                "        // 'this(orderId)' ran first — OrderId, CreatedAt, Status all set.",
                "        // Now we override Status with the provided value.",
                "        Status = initialStatus;",
                "    }",
                "",
                "    // ── PATTERN 3: Object initializer syntax (no extra code needed) ─",
                "    // If you have public setters, caller can do:",
                "    // var order = new Order(1) { Status = \"Submitted\" };",
                "    // But: use this only for DTO-style objects, not domain objects.",
                "    // Domain objects should ONLY change state through methods.",
                "}"
            );

            UI.Code("Guard clauses — validating input in constructors",
                "// GUARD CLAUSE pattern: check the error condition and return/throw EARLY.",
                "// This keeps the 'happy path' code un-indented and easy to read.",
                "",
                "// ❌ BAD — nested if blocks, hard to read:",
                "public Employee(string name, decimal salary)",
                "{",
                "    if (!string.IsNullOrEmpty(name))",
                "    {",
                "        if (salary > 0)",
                "        {",
                "            _name   = name;    // Happy path buried in nesting",
                "            _salary = salary;",
                "        }",
                "    }",
                "}",
                "",
                "// ✓ GOOD — guard clauses at top, happy path flows naturally:",
                "public Employee(string name, decimal salary)",
                "{",
                "    if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(\"Name required\");",
                "    if (salary <= 0)  throw new ArgumentException(\"Salary must be positive\");",
                "    // All validation done. Happy path is unindented and clean:",
                "    _name   = name;",
                "    _salary = salary;",
                "}"
            );

            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_PropertiesVsFields()
        {
            UI.SubHeader("Properties vs Fields — Why Properties Win");

            UI.Concept(
                "A FIELD is raw storage. Anyone can read or write it (if public). " +
                "A PROPERTY adds a layer of control: you can validate in the setter, " +
                "compute a value in the getter, or make it read-only. " +
                "In C#, always expose data through properties, not public fields."
            );

            UI.Code("Property patterns — from simple to powerful",
                "public class Employee",
                "{",
                "    private string _name;",
                "    private decimal _salaryPerMonth;",
                "",
                "    // ── PATTERN 1: Auto-property (compiler generates backing field) ──",
                "    // Use when you just need get/set with no logic.",
                "    public int EmployeeId { get; private set; }  // read publicly, set only inside class",
                "    public string Department { get; set; }        // fully public read/write",
                "",
                "    // ── PATTERN 2: Property with validation in setter ─────────────",
                "    // The ONLY way to set _name is through this property.",
                "    // The setter validates before storing. External code can't bypass this.",
                "    public string Name",
                "    {",
                "        get => _name;   // Arrow syntax for one-liner getter",
                "        set",
                "        {",
                "            if (string.IsNullOrWhiteSpace(value))",
                "                throw new ArgumentException(\"Name cannot be blank.\");",
                "            _name = value;  // 'value' is the keyword for the incoming data",
                "        }",
                "    }",
                "",
                "    // ── PATTERN 3: Computed (derived) property — no backing field ──",
                "    // Annual salary is ALWAYS salary * 12. No need to store it separately.",
                "    // Computed properties prevent data from getting 'out of sync'.",
                "    public decimal AnnualSalary => _salaryPerMonth * 12;",
                "",
                "    // ── PATTERN 4: Init-only property (C# 9+) ───────────────────",
                "    // Can be set during construction only. Immutable after that.",
                "    public DateTime HireDate { get; init; }",
                "}"
            );

            UI.Mistake(
                "Making all properties public with both get AND set: 'public string Name { get; set; }'. " +
                "This is fine for DTOs (data transfer objects), but domain objects that represent " +
                "business concepts should control their own state — use private set or no setter.",
                "Ask: 'Should external code be able to directly change this value?' " +
                "If yes, public set is fine. If there's any business rule around changing it, " +
                "use private set and a dedicated method like 'employee.ChangeName(\"NewName\")'."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_StaticVsInstance()
        {
            UI.SubHeader("Static vs Instance — Class-Level vs Object-Level");

            UI.Concept(
                "INSTANCE members (no 'static' keyword): belong to a specific object. " +
                "Each object gets its own copy of instance fields. " +
                "STATIC members (with 'static' keyword): belong to the CLASS itself. " +
                "They're shared across ALL objects. There's only one copy, ever."
            );

            UI.Diagram("Visualizing Static vs Instance",
                "  STATIC field: ONE copy shared by ALL BankAccount objects",
                "  ┌─────────────────────────────────────────────────────────┐",
                "  │  BankAccount class: static int _totalAccounts = 3       │",
                "  └──────────────────────────┬──────────────────────────────┘",
                "            ┌─────────────────┼─────────────────┐",
                "            ▼                 ▼                 ▼",
                "  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐",
                "  │ deepak obj   │  │ john obj     │  │ mary obj     │",
                "  │ _balance=5000│  │ _balance=1000│  │ _balance=250 │",
                "  └──────────────┘  └──────────────┘  └──────────────┘",
                "  INSTANCE fields: each object has its OWN separate copy"
            );

            UI.Code("Static vs Instance in practice",
                "public class BankAccount",
                "{",
                "    // STATIC: belongs to the class — tracks total count across ALL accounts",
                "    private static int _totalAccounts = 0;",
                "    public  static int TotalAccounts => _totalAccounts;",
                "",
                "    // INSTANCE: belongs to this specific account object",
                "    private string  _owner;",
                "    private decimal _balance;",
                "",
                "    public BankAccount(string owner, decimal balance)",
                "    {",
                "        _owner   = owner;",
                "        _balance = balance;",
                "        _totalAccounts++;  // Increment the SHARED counter for the whole class",
                "    }",
                "}",
                "",
                "// Usage:",
                "var acc1 = new BankAccount(\"Deepak\", 5000);  // _totalAccounts = 1",
                "var acc2 = new BankAccount(\"John\", 1000);    // _totalAccounts = 2",
                "Console.WriteLine(BankAccount.TotalAccounts); // Prints 2",
                "// Note: called on the CLASS (BankAccount.), not an instance (acc1.)"
            );

            UI.Print("\nWHEN TO USE STATIC:\n");
            UI.Good("Use static for", "Utility/helper methods (MathHelper.Round, StringHelper.Truncate), " +
                "constants (Math.PI), factory methods, configuration that doesn't change per-object.");
            UI.Bad("Avoid static for", "Anything that holds mutable state shared across the app. " +
                "Static mutable state is global state — it makes testing hard and creates hidden dependencies.");

            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section7_LiveDemo()
        {
            UI.SubHeader("Live Demo — Watch BankAccount Objects Work");

            UI.Print("The BankAccount class is defined at the top of this file. Let's use it:\n");

            UI.LiveDemo("Creating and using BankAccount objects", () =>
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     // Creating two separate account objects:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("     var deepak = new BankAccount(\"Deepak\", 5000);");
                Console.WriteLine("     var john   = new BankAccount(\"John\",   1200);");
                Console.ResetColor();

                var deepak = new BankAccount("Deepak", 5000);
                var john   = new BankAccount("John",   1200);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n     deepak: {deepak}");
                Console.WriteLine($"     john:   {john}");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     // Deposit into Deepak's account only:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("     deepak.Deposit(500);");
                Console.ResetColor();

                deepak.Deposit(500);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n     deepak: {deepak}  ← changed");
                Console.WriteLine($"     john:   {john}    ← unchanged");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     // Try to overdraw John:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("     bool success = john.Withdraw(5000);");
                Console.ResetColor();

                bool success = john.Withdraw(5000);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n     Withdrawal success: {success}  ← false (insufficient funds)");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"     john: {john}  ← balance unchanged");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE: You're building a 'Student' class for a university system.\n" +
                "What fields, properties, and methods would it have?\n" +
                "Think about: what data identifies a student? What can a student DO?\n" +
                "What rules should the system enforce (constraints)?",

                "Suggested design:\n\n" +
                "  Fields:      _studentId, _firstName, _lastName, _gpa, _enrolledCourses (List)\n\n" +
                "  Properties:  StudentId (read-only), FullName (computed: first + last),\n" +
                "               Gpa (get; private set — validation: 0.0 to 4.0),\n" +
                "               IsOnDeansList (computed: GPA >= 3.5)\n\n" +
                "  Methods:     EnrollIn(Course course) — validates capacity, no duplicate enrollment\n" +
                "               Drop(string courseCode) — only if not past drop deadline\n" +
                "               GetTranscript() — returns read-only list of courses and grades\n\n" +
                "  Constructor: Student(int id, string firstName, string lastName)\n" +
                "               — id must be > 0, names must not be empty"
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Classes and Objects", new QuizQuestion[]
            {
                new(
                    "What is the difference between a class and an object?",
                    new[] {
                        "They are the same thing — 'class' and 'object' are interchangeable",
                        "A class is the blueprint (design time). An object is a concrete instance (runtime) with real data",
                        "A class exists in memory. An object is just a type definition",
                        "An object is a collection of classes"
                    },
                    1,
                    "Class = blueprint (no runtime memory for data). Object = instance created from the class, allocated on the heap with real field values."
                ),
                new(
                    "Why should fields typically be private?",
                    new[] {
                        "To make the code compile faster",
                        "Private fields are stored more efficiently in memory",
                        "To prevent external code from putting the object into an invalid state",
                        "Because public fields don't work in C#"
                    },
                    2,
                    "Private fields enforce encapsulation. External code must go through your properties/methods, which can validate input."
                ),
                new(
                    "You write: BankAccount copy = original; Then copy.Deposit(100). What happens?",
                    new[] {
                        "Only 'copy' changes — it's a separate account",
                        "Both 'copy' and 'original' change — they point to the SAME object",
                        "A compile error — you can't assign objects this way",
                        "The Deposit call fails because 'copy' is just a reference"
                    },
                    1,
                    "For class (reference type) objects, assignment copies the REFERENCE, not the object. Both variables point to the same heap memory."
                ),
                new(
                    "When does a constructor run?",
                    new[] {
                        "Every time you call any method on the object",
                        "Once per class, when the program starts",
                        "Once per object, immediately when 'new ClassName(...)' is called",
                        "When the garbage collector cleans up the object"
                    },
                    2,
                    "The constructor runs exactly once per object — at creation. Its job is to initialize the object to a valid, usable state."
                ),
                new(
                    "What is a static method?",
                    new[] {
                        "A method that never changes its return value",
                        "A method that belongs to the CLASS, not an instance. No 'new' needed to call it.",
                        "A method that can only be called once",
                        "A method with no parameters"
                    },
                    1,
                    "Static members belong to the class itself. Math.Sqrt() is static — you don't create a 'new Math()' to call it."
                ),
            });
        }
    }
}
