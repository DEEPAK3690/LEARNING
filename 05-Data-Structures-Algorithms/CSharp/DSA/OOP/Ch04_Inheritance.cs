// ============================================================
// FILE: Ch04_Inheritance.cs
// PURPOSE: Teach Inheritance — IS-A relationships, code reuse,
//          the base keyword, virtual/override, and when NOT to use it.
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO: Employee Hierarchy ────────────────────────────────

    // BASE CLASS: the common ancestor. Contains SHARED behavior and state.
    internal class Employee
    {
        // Protected: derived classes can access these directly.
        // Private would be MORE encapsulated, but protected is common for fields
        // that subclasses genuinely need to work with.
        public int    Id     { get; }
        public string Name   { get; }
        protected decimal _salary; // protected: subclasses can read/write it

        public decimal Salary => _salary;

        public Employee(int id, string name, decimal salary)
        {
            if (id <= 0)    throw new ArgumentException("ID must be positive.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.");
            if (salary <= 0) throw new ArgumentException("Salary must be positive.");
            Id = id; Name = name; _salary = salary;
        }

        // 'virtual' = this method CAN be overridden in derived classes.
        // Without 'virtual', derived classes cannot change this method's behavior.
        public virtual decimal CalculateBonus() => _salary * 0.05m; // 5% default

        public virtual string GetTitle() => "Employee";

        // 'virtual' here allows subclasses to add extra info to ToString
        public override string ToString() =>
            $"[{GetTitle()}] {Name} (ID:{Id}) Salary:${_salary:N0} Bonus:${CalculateBonus():N0}";
    }

    // DERIVED CLASS: inherits everything from Employee, adds its own behavior
    internal class Manager : Employee
    {
        // Manager-specific state — not in Employee because regular employees don't manage teams
        public List<Employee> DirectReports { get; } = new();

        // ': base(id, name, salary)' calls the PARENT'S constructor.
        // REQUIRED: parent class has no parameterless constructor.
        // The parent constructor runs FIRST, then this constructor's body.
        public Manager(int id, string name, decimal salary) : base(id, name, salary) { }

        // 'override' = replace the parent's implementation with this one.
        // The 'override' keyword + the parent method's 'virtual' keyword work as a pair.
        public override decimal CalculateBonus()
        {
            // 'base.CalculateBonus()' calls the PARENT's version first.
            // Then we ADD the team bonus on top.
            decimal personalBonus = base.CalculateBonus();      // 5% of own salary
            decimal teamBonus     = DirectReports.Sum(e => e.Salary) * 0.01m; // 1% of team salaries
            return personalBonus + teamBonus;
        }

        public override string GetTitle() => "Manager";

        public void AddReport(Employee e) => DirectReports.Add(e);
    }

    // A class derived from a derived class — multi-level inheritance
    internal class Director : Manager
    {
        public string Department { get; }

        public Director(int id, string name, decimal salary, string dept)
            : base(id, name, salary) // calls Manager's constructor
        {
            Department = dept;
        }

        // Director overrides bonus again — their own formula
        public override decimal CalculateBonus() => _salary * 0.20m; // Flat 20% for directors

        public override string GetTitle() => $"Director of {Department}";
    }

    // SEALED CLASS: 'sealed' prevents further inheritance.
    // Use when the class is complete and should not be specialized further.
    internal sealed class CEO : Director
    {
        public CEO(int id, string name, decimal salary)
            : base(id, name, salary, "Company") { }

        public override decimal CalculateBonus() => _salary * 0.50m; // 50% bonus
        public override string GetTitle() => "CEO";
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch04_Inheritance
    {
        public static void Run()
        {
            UI.Header("CHAPTER 4: INHERITANCE — REUSE WITHOUT REWRITING");

            UI.Concept(
                "Inheritance lets a class (the child) acquire the fields, properties, and methods " +
                "of another class (the parent), then extend or change them. " +
                "It models the IS-A relationship: a Manager IS-A Employee. " +
                "A Circle IS-A Shape. A Dog IS-A Animal."
            );
            UI.Pause();

            Section1_WhyInheritanceExists();
            Section2_HowItWorks();
            Section3_VirtualAndOverride();
            Section4_BaseKeyword();
            Section5_ProtectedMembers();
            Section6_WhenNOTToInherit();
            Section7_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_WhyInheritanceExists()
        {
            UI.SubHeader("Why Does Inheritance Exist?");

            UI.Analogy("Family Traits",
                "Children inherit traits from parents — eye color, height tendency, some behaviors. " +
                "But they're still distinct individuals with their own characteristics. " +
                "In code: Manager inherits Name, Salary, and basic work behavior from Employee. " +
                "But Manager has a team, a bigger bonus, and different responsibilities. " +
                "Inheritance captures the 'family resemblance' while allowing specialization."
            );

            UI.Print("THE PROBLEM WITHOUT INHERITANCE:\n");

            UI.Code("Duplication without inheritance",
                "// WITHOUT inheritance, every employee type duplicates fields and methods:",
                "class RegularEmployee",
                "{",
                "    public int    Id     { get; }",
                "    public string Name   { get; }",
                "    public decimal Salary { get; }",
                "    public void   PrintPayStub() { ... }",
                "    public void   SubmitTimesheet() { ... }",
                "}",
                "",
                "class ManagerEmployee  // 90% duplicate code!",
                "{",
                "    public int    Id     { get; }  // ← same",
                "    public string Name   { get; }  // ← same",
                "    public decimal Salary { get; } // ← same",
                "    public void   PrintPayStub() { ... }    // ← same",
                "    public void   SubmitTimesheet() { ... } // ← same",
                "    // Only NEW stuff:",
                "    public List<Employee> DirectReports { get; }",
                "    public void RunPerformanceReview() { ... }",
                "}",
                "",
                "// Bug in PrintPayStub? Fix it in 2 places. Then 3. Then 10.",
                "// Add a new method? Copy it to every class. NIGHTMARE."
            );

            UI.Good("Inheritance solution", "Define all shared behavior ONCE in Employee. " +
                "Manager inherits it and only adds/changes what's different. " +
                "Fix PrintPayStub once → fixed everywhere.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_HowItWorks()
        {
            UI.SubHeader("How Inheritance Works — The Mechanics");

            UI.Code("Inheritance syntax and what the derived class gets",
                "// Syntax: DerivedClass : BaseClass",
                "// The colon means 'inherits from'",
                "public class Manager : Employee",
                "{",
                "    // Manager automatically HAS (inherited from Employee):",
                "    //   Id, Name, Salary  (public/protected properties)",
                "    //   CalculateBonus()  (if virtual, can be overridden)",
                "    //   ToString()        (can be overridden)",
                "",
                "    // Manager ADDS:",
                "    public List<Employee> DirectReports { get; } = new();",
                "",
                "    // Constructor: MUST call base constructor using ':base(...)'",
                "    // Employee has no default (no-arg) constructor, so we MUST pass args",
                "    public Manager(int id, string name, decimal salary)",
                "        : base(id, name, salary)  // Employee's constructor runs FIRST",
                "    {",
                "        // By the time we get here, Employee's constructor already ran.",
                "        // Id, Name, Salary are already initialized.",
                "        // We only add Manager-specific initialization here.",
                "    }",
                "}"
            );

            UI.Diagram("Inheritance Chain Memory Model",
                "  When you create: var mgr = new Manager(1, \"Alice\", 90000);",
                "",
                "  Memory (heap):  ONE object, but contains fields from ALL ancestors:",
                "  ┌──────────────────────────────────────────────────────────┐",
                "  │  Manager object                                          │",
                "  │  ┌─── FROM Employee: ─────────────────────────────┐     │",
                "  │  │  Id = 1, Name = \"Alice\", _salary = 90000       │     │",
                "  │  └────────────────────────────────────────────────┘     │",
                "  │  ┌─── FROM Manager: ──────────────────────────────┐     │",
                "  │  │  DirectReports = []                            │     │",
                "  │  └────────────────────────────────────────────────┘     │",
                "  └──────────────────────────────────────────────────────────┘",
                "",
                "  mgr.Name          → inherited from Employee (works!)",
                "  mgr.DirectReports → defined in Manager (works!)",
                "  mgr.CalculateBonus() → Manager's override is called (not Employee's)"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_VirtualAndOverride()
        {
            UI.SubHeader("virtual and override — The Core of Runtime Polymorphism");

            UI.Concept(
                "The 'virtual' and 'override' keywords work as a PAIR:\n" +
                "  • The BASE class marks a method 'virtual' → 'I can be changed'\n" +
                "  • A DERIVED class marks its version 'override' → 'I'm changing it'\n\n" +
                "Without 'virtual', a subclass CANNOT change inherited behavior. " +
                "The compiler enforces this as a safety feature."
            );

            UI.Code("virtual/override — the mechanics",
                "public class Employee",
                "{",
                "    // 'virtual' = subclasses MAY override this",
                "    public virtual decimal CalculateBonus() => _salary * 0.05m;",
                "}",
                "",
                "public class Manager : Employee",
                "{",
                "    // 'override' = I'm replacing the parent's implementation",
                "    // MUST match the parent method's signature exactly",
                "    public override decimal CalculateBonus()",
                "    {",
                "        return base.CalculateBonus() + TeamBonus();  // Calls parent's version too",
                "    }",
                "}",
                "",
                "// ❌ Method HIDING (the wrong way, avoid this):",
                "public class BadManager : Employee",
                "{",
                "    // 'new' hides the parent method, but does NOT override it polymorphically",
                "    public new decimal CalculateBonus() => 99999; // BAD PRACTICE",
                "    // Problem: if called through Employee reference, Employee's method runs!",
                "    // This is confusing and should almost never be used.",
                "}"
            );

            UI.Code("Why the distinction matters — polymorphism in action",
                "Employee emp = new Manager(1, \"Alice\", 80000);",
                "// 'emp' is declared as Employee type, but actually holds a Manager object",
                "",
                "// WITH 'virtual/override':",
                "emp.CalculateBonus();  // Calls MANAGER's CalculateBonus() — correct!",
                "// Runtime knows the actual type is Manager → calls Manager's method",
                "",
                "// WITH 'new' (method hiding):",
                "emp.CalculateBonus();  // Would call EMPLOYEE's version — WRONG!",
                "// Because: 'new' only takes effect when called through Manager reference"
            );

            UI.KeyPoint("Always use 'virtual' in the base class and 'override' in derived classes. Avoid 'new' for hiding.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_BaseKeyword()
        {
            UI.SubHeader("The 'base' Keyword — Calling Parent Code");

            UI.Concept(
                "The 'base' keyword lets a derived class access the PARENT's members:\n" +
                "  • 'base.Method()' — calls the parent's version of an overridden method\n" +
                "  • 'base(args)' in a constructor — calls the parent's constructor\n\n" +
                "This lets you EXTEND behavior rather than completely replace it."
            );

            UI.Code("Two uses of 'base'",
                "public class Director : Manager",
                "{",
                "    public string Department { get; }",
                "",
                "    // USE 1: base() in constructor — calls Manager's constructor",
                "    // Manager's constructor in turn calls Employee's constructor",
                "    // Constructor chain: Director → Manager → Employee",
                "    public Director(int id, string name, decimal salary, string dept)",
                "        : base(id, name, salary)  // calls Manager(id, name, salary)",
                "    {",
                "        Department = dept;",
                "    }",
                "",
                "    public override decimal CalculateBonus()",
                "    {",
                "        // USE 2: base.CalculateBonus() — calls Manager's version",
                "        // Then we add a Director-level multiplier on top",
                "        decimal managerBonus = base.CalculateBonus();",
                "        return managerBonus * 1.5m;  // Directors get 50% more than managers",
                "    }",
                "}"
            );

            UI.Diagram("Constructor Call Chain",
                "  new Director(1, \"Bob\", 150000, \"Engineering\")  called",
                "       ↓",
                "  Director constructor body runs first? NO. Chain first:",
                "       ↓  : base(1, \"Bob\", 150000)  →  Manager(1, \"Bob\", 150000)",
                "       ↓  : base(1, \"Bob\", 150000)  →  Employee(1, \"Bob\", 150000)",
                "       ↓  Employee constructor body runs  (sets Id, Name, Salary)",
                "       ↓  Manager constructor body runs   (sets DirectReports = [])",
                "       ↓  Director constructor body runs  (sets Department = \"Engineering\")",
                "  Object is now fully initialized."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_ProtectedMembers()
        {
            UI.SubHeader("protected — Sharing With Derived Classes Only");

            UI.Concept(
                "'protected' is the access modifier for inherited code sharing. " +
                "Protected members are:\n" +
                "  • Accessible inside the class that defines them\n" +
                "  • Accessible in all derived classes (anywhere in the hierarchy)\n" +
                "  • NOT accessible from external code (unlike 'public')\n\n" +
                "Use 'protected' for behavior or data that derived classes need " +
                "but external code should not touch."
            );

            UI.Code("Protected vs Private in inheritance",
                "public class Employee",
                "{",
                "    private string _ssn;        // private: subclasses CANNOT access this",
                "    protected decimal _salary;  // protected: subclasses CAN access this",
                "",
                "    protected virtual void OnSalaryChanged(decimal newSalary)",
                "    {",
                "        // Hook method: derived classes can override this to react to salary changes",
                "        // External code can't call this (it's protected)",
                "    }",
                "}",
                "",
                "public class Manager : Employee",
                "{",
                "    public void GiveTeamRaise(decimal percent)",
                "    {",
                "        _salary *= (1 + percent);   // ✓ Can access protected _salary",
                "        // _ssn = \"xxx\";           // ✗ Compile error — private is private",
                "        OnSalaryChanged(_salary);   // ✓ Can call protected method",
                "    }",
                "}"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_WhenNOTToInherit()
        {
            UI.SubHeader("When NOT to Use Inheritance — The Most Important Lesson");

            UI.Concept(
                "Inheritance is powerful but DANGEROUS when misused. " +
                "The rule is: ONLY use inheritance when you have a genuine IS-A relationship " +
                "AND the derived class is truly a specialization of the base class. " +
                "Using inheritance just to reuse code is a design smell."
            );

            UI.Print("THE IS-A TEST — Ask these questions before inheriting:\n");
            var questions = new[]
            {
                "Is 'DerivedClass IS-A BaseClass' always true in every context?",
                "Can I use a DerivedClass object anywhere a BaseClass is expected, without surprises?",
                "Does the derived class need ALL of the base class's behavior?",
                "Will the derived class behave consistently with every method the base class has?",
            };

            foreach (var q in questions)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  ? {q}");
            }
            Console.ResetColor();
            Console.WriteLine();

            UI.Code("Famous anti-pattern: Stack inheriting from List",
                "// ❌ BAD: Stack IS-A List? NO. A Stack has LIFO behavior only.",
                "public class Stack<T> : List<T>",
                "{",
                "    public void Push(T item) => Add(item);   // Use List.Add",
                "    public T Pop()  { var t = this[^1]; RemoveAt(Count - 1); return t; }",
                "    // PROBLEM: all List<T> methods are now exposed on Stack!",
                "    // Caller can do: myStack.Insert(0, item);  // BREAKS LIFO rule!",
                "    // Caller can do: myStack.RemoveAt(5);       // BREAKS LIFO rule!",
                "    // Stack's invariant (LIFO only) is completely broken",
                "}",
                "",
                "// ✓ GOOD: Stack USES a list internally (composition) — doesn't expose it",
                "public class Stack<T>",
                "{",
                "    private List<T> _items = new();    // HAS-A list, doesn't IS-A list",
                "    public void Push(T item)  => _items.Add(item);",
                "    public T    Pop()  { var t = _items[^1]; _items.RemoveAt(_items.Count - 1); return t; }",
                "    public T    Peek() => _items[^1];",
                "    public int  Count  => _items.Count;",
                "    // ONLY LIFO operations exposed. Invariant protected.",
                "}"
            );

            UI.Mistake(
                "Using inheritance to reuse code without a true IS-A relationship. " +
                "This leads to the 'Fragile Base Class Problem': any change to the base class " +
                "can break subclasses in unexpected ways.",
                "Ask: 'Am I inheriting because this IS-A that, or just to reuse code?' " +
                "If it's just for code reuse, use composition instead (has-a relationship). " +
                "We'll cover this in Chapter 6."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section7_LiveDemo()
        {
            UI.SubHeader("Live Demo — Employee Hierarchy in Action");

            UI.LiveDemo("Inheritance hierarchy running", () =>
            {
                var emp1 = new Employee(1, "Priya",   60000);
                var emp2 = new Employee(2, "Carlos",  65000);
                var mgr  = new Manager(3,  "Deepak",  90000);
                var dir  = new Director(4, "Sandra", 150000, "Engineering");
                var ceo  = new CEO(5,      "Raj",    300000);

                mgr.AddReport(emp1);
                mgr.AddReport(emp2);
                dir.AddReport(mgr);

                // Use a list of Employee — all objects can be treated as Employee
                var everyone = new List<Employee> { emp1, emp2, mgr, dir, ceo };

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     Payroll Report:");
                Console.WriteLine("     " + new string('─', 65));
                Console.ResetColor();

                decimal totalPayroll = 0;
                decimal totalBonuses = 0;

                foreach (var person in everyone)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"     {person}");
                    totalPayroll += person.Salary;
                    totalBonuses += person.CalculateBonus();
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n     Total Payroll: ${totalPayroll:N0}");
                Console.WriteLine($"     Total Bonuses: ${totalBonuses:N0}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Notice: foreach uses 'Employee' type, but each object");
                Console.WriteLine("     calls ITS OWN CalculateBonus() method. That's polymorphism.");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE: You're building a vehicle management system.\n\n" +
                "You have: Vehicle, Car, ElectricCar, Truck, Motorcycle, ElectricMotorcycle\n\n" +
                "1. Draw the inheritance hierarchy. Which IS-A relationships are valid?\n" +
                "2. What should 'Vehicle' have as shared behavior?\n" +
                "3. Should 'ElectricCar' inherit from 'Car' or from something else?\n" +
                "4. Is there a risk of the 'Fragile Base Class' problem here?",

                "SUGGESTED HIERARCHY:\n" +
                "  Vehicle (abstract)\n" +
                "    ├── Car\n" +
                "    │    └── ElectricCar      (IS-A Car that also IS-A Vehicle)\n" +
                "    ├── Truck\n" +
                "    └── Motorcycle\n" +
                "         └── ElectricMotorcycle\n\n" +
                "VEHICLE should have: MaxSpeed, FuelCapacity, RegisteredOwner,\n" +
                "  Start(), Stop(), GetServiceStatus(), RegisterPlate()\n\n" +
                "ELECTRIC vehicles: add IElectric interface with:\n" +
                "  BatteryLevel, Charge(), GetRange()\n" +
                "  Don't put battery in Vehicle — non-electric vehicles don't have one.\n\n" +
                "FRAGILE BASE CLASS RISK: If Vehicle.Start() changes behavior,\n" +
                "  ElectricCar.Start() might break. Mitigation: keep base methods small\n" +
                "  and focused. Use virtual/override properly."
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Inheritance", new QuizQuestion[]
            {
                new(
                    "What does 'public class Manager : Employee' mean?",
                    new[] {
                        "Manager IS-A Employee and inherits its public and protected members",
                        "Manager CONTAINS an Employee object",
                        "Employee inherits from Manager",
                        "Manager and Employee share a namespace"
                    },
                    0,
                    "The colon (:) denotes inheritance. Manager IS-A Employee and gets all its public and protected members."
                ),
                new(
                    "Why must you use 'virtual' on a base class method before a subclass can 'override' it?",
                    new[] {
                        "For performance — virtual methods are faster to dispatch",
                        "It's just a syntax requirement with no practical effect",
                        "It explicitly allows subclasses to change the behavior. Without it, the method is locked.",
                        "Virtual methods are automatically public"
                    },
                    2,
                    "'virtual' is a deliberate design decision: the base class author says 'this method is designed to be specialized.' It prevents accidental overriding."
                ),
                new(
                    "In a constructor chain (Director : Manager : Employee), what runs first?",
                    new[] {
                        "Director's constructor body runs first",
                        "The most derived class runs first, then works up to the base",
                        "Employee's constructor runs first, then Manager's, then Director's",
                        "All constructors run in parallel"
                    },
                    2,
                    "Constructor chain runs base-first. Employee initializes, then Manager, then Director. This ensures the object is in a valid state at every level."
                ),
                new(
                    "What is wrong with making Stack<T> inherit from List<T>?",
                    new[] {
                        "Nothing — inheritance is always the right approach for code reuse",
                        "Stack IS NOT-A List. Inheriting exposes all List methods on Stack, breaking the LIFO invariant",
                        "C# doesn't allow inheriting from generic classes",
                        "List<T> is sealed, so you can't inherit from it"
                    },
                    1,
                    "Stack IS-A List is false. Stack should CONTAIN a list (composition). Inheriting exposes Insert(), RemoveAt() etc., destroying Stack's LIFO guarantee."
                ),
                new(
                    "What does 'base.CalculateBonus()' do inside an overriding method?",
                    new[] {
                        "Creates a new instance of the base class",
                        "Calls the base class's version of CalculateBonus(), not the overriding one",
                        "Prevents the override from executing",
                        "Calls all overrides in the hierarchy"
                    },
                    1,
                    "'base.Method()' explicitly calls the parent's implementation. Useful to extend (add to) parent behavior rather than completely replacing it."
                ),
            });
        }
    }
}
