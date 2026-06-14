// ============================================================
// FILE: Ch06_Relationships.cs
// PURPOSE: Teach Composition, Aggregation, and Association —
//          the three "HAS-A" relationships, plus the crucial
//          principle: "Prefer Composition Over Inheritance."
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO: Document Editor built with Composition ─────────────

    // Each component is small, focused, and independently testable.
    // This is composition: Document OWNS (and creates) these components.

    internal interface ISpellChecker
    {
        List<string> Check(string text);
    }

    internal interface IAutoSaver
    {
        void Save(string content, string filePath);
    }

    internal interface IFormatter
    {
        string Format(string text);
    }

    // Concrete implementations — each does ONE thing
    internal class BasicSpellChecker : ISpellChecker
    {
        private static readonly HashSet<string> _badWords =
            new(StringComparer.OrdinalIgnoreCase) { "teh", "adn", "recieve", "occured" };

        public List<string> Check(string text) =>
            text.Split(' ').Where(w => _badWords.Contains(w)).ToList();
    }

    internal class LocalAutoSaver : IAutoSaver
    {
        public void Save(string content, string filePath)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"     💾 Auto-saved to {filePath} ({content.Length} chars)");
            Console.ResetColor();
        }
    }

    internal class MarkdownFormatter : IFormatter
    {
        public string Format(string text) => text.Replace("**", "").Replace("__", "");
    }

    // COMPOSITION: Document creates and OWNS its components.
    // If Document is destroyed, its components go with it.
    internal class Document
    {
        private string _content = "";
        private readonly string _filePath;

        // Components injected via constructor (Dependency Injection)
        // This makes Document testable — swap real components for fakes in tests
        private readonly ISpellChecker _spellChecker;
        private readonly IAutoSaver    _autoSaver;
        private readonly IFormatter    _formatter;

        public string Title { get; }

        public Document(string title, string filePath,
            ISpellChecker spellChecker, IAutoSaver autoSaver, IFormatter formatter)
        {
            Title        = title;
            _filePath    = filePath;
            _spellChecker = spellChecker;
            _autoSaver    = autoSaver;
            _formatter    = formatter;
        }

        public void Type(string text)
        {
            _content += text;

            // Ask the spell checker (we don't care HOW it works)
            var errors = _spellChecker.Check(text);
            if (errors.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"     ⚠  Spell errors: {string.Join(", ", errors)}");
                Console.ResetColor();
            }

            // Auto-save (we don't care WHERE it saves)
            _autoSaver.Save(_content, _filePath);
        }

        public string GetFormattedContent() => _formatter.Format(_content);
    }

    // AGGREGATION DEMO: A Department and its Employees.
    // Employees exist independently — they don't die when Department is dissolved.
    internal class DepartmentDemo
    {
        public string Name { get; }
        private readonly List<EmployeeRef> _employees = new(); // Aggregation

        public DepartmentDemo(string name) => Name = name;

        public void AddEmployee(EmployeeRef emp) => _employees.Add(emp);
        public void RemoveEmployee(int id) => _employees.RemoveAll(e => e.Id == id);
        public int Count => _employees.Count;
        public IReadOnlyList<EmployeeRef> Employees => _employees.AsReadOnly();
    }

    // Simple employee reference class for aggregation demo
    internal class EmployeeRef
    {
        public int    Id   { get; }
        public string Name { get; }
        public EmployeeRef(int id, string name) { Id = id; Name = name; }
        public override string ToString() => $"{Name} (ID:{Id})";
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch06_Relationships
    {
        public static void Run()
        {
            UI.Header("CHAPTER 6: RELATIONSHIPS — COMPOSITION VS AGGREGATION");

            UI.Concept(
                "Objects rarely exist in isolation. They relate to other objects. " +
                "Understanding the TYPE of relationship between objects is critical for " +
                "good design. The three relationships are:\n" +
                "  • Association  — A uses B (weakest)\n" +
                "  • Aggregation  — A contains B, but B can live without A\n" +
                "  • Composition  — A owns B, B cannot live without A (strongest)"
            );
            UI.Pause();

            Section1_ThreeRelationships();
            Section2_Association();
            Section3_Aggregation();
            Section4_Composition();
            Section5_CompositionOverInheritance();
            Section6_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_ThreeRelationships()
        {
            UI.SubHeader("The Three Object Relationships");

            UI.Diagram("Relationship Strength Spectrum",
                "  WEAKEST ◄────────────────────────────────────────► STRONGEST",
                "",
                "  ASSOCIATION          AGGREGATION              COMPOSITION",
                "  (uses / knows)       (has, weak)              (owns, strong)",
                "",
                "  Teacher uses Student  Department has Employee  Car owns Engine",
                "  Teacher → Student     Department ◇── Employee  Car ◆── Engine",
                "",
                "  Student exists        Employee exists          Engine CANNOT exist",
                "  independently         without Department       without a Car",
                "",
                "  UML symbols:",
                "  → simple arrow = association",
                "  ◇── open diamond = aggregation (whole/part, part is independent)",
                "  ◆── filled diamond = composition (whole/part, part depends on whole)"
            );

            UI.Analogy("Real World Examples of Each",
                "ASSOCIATION: A Driver uses a Car. Driver exists without Car, Car exists without Driver.\n" +
                "AGGREGATION: A Team has Players. Dissolve the team — players still exist, can join other teams.\n" +
                "COMPOSITION: A House has Rooms. Demolish the house — rooms are gone. Rooms are part of the house."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_Association()
        {
            UI.SubHeader("Association — 'Uses' Relationship");

            UI.Concept(
                "Association is the weakest relationship. Object A uses Object B, but neither " +
                "owns the other. B can exist without A, and A can exist without B. " +
                "This is often just a method parameter or temporary reference."
            );

            UI.Code("Association example — Teacher uses Student (not permanent)",
                "public class Teacher",
                "{",
                "    public string Name { get; }",
                "    public Teacher(string name) => Name = name;",
                "",
                "    // Association: Teacher USES Student temporarily (method parameter)",
                "    // Teacher doesn't HOLD a Student. Student doesn't KNOW about Teacher.",
                "    // Both exist independently.",
                "    public void Grade(Student student, int score)",
                "    {",
                "        Console.WriteLine($\"{Name} graded {student.Name}: {score}/100\");",
                "        student.RecordGrade(score);",
                "    }",
                "}",
                "",
                "// Usage:",
                "var teacher = new Teacher(\"Mr. Smith\");  // exists independently",
                "var student = new Student(\"Deepak\");     // exists independently",
                "teacher.Grade(student, 95);               // temporary interaction",
                "// teacher and student still exist and work independently after this call"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_Aggregation()
        {
            UI.SubHeader("Aggregation — 'Has' with Independent Lifetime");

            UI.Concept(
                "Aggregation is 'has-a' where the contained object has an INDEPENDENT lifetime. " +
                "The container doesn't create or destroy the contained object. " +
                "The contained object is passed in and can exist after the container is gone."
            );

            UI.Code("Aggregation — Department has Employees (employees aren't destroyed when dept is)",
                "public class Department",
                "{",
                "    public string Name { get; }",
                "    private List<Employee> _members = new();",
                "",
                "    // EMPLOYEES ARE CREATED OUTSIDE AND PASSED IN:",
                "    // Department does NOT create employees. They already exist.",
                "    public void AddMember(Employee employee)   => _members.Add(employee);",
                "    public void RemoveMember(Employee employee) => _members.Remove(employee);",
                "",
                "    // If Department is garbage collected: _members list is gone,",
                "    // but each EMPLOYEE object still lives in memory (other code may hold references).",
                "}",
                "",
                "// Usage shows the independence:",
                "var emp1 = new Employee(1, \"Deepak\", 80000);   // Employee created separately",
                "var emp2 = new Employee(2, \"Priya\",  75000);",
                "",
                "var engineering = new Department(\"Engineering\");",
                "engineering.AddMember(emp1);  // Pass existing employees in",
                "engineering.AddMember(emp2);",
                "",
                "engineering = null;            // Department is gone",
                "// emp1 and emp2 are STILL ALIVE. They can be added to another department.",
                "var hr = new Department(\"HR\");",
                "hr.AddMember(emp2);            // emp2 transferred — still the same object"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_Composition()
        {
            UI.SubHeader("Composition — 'Owns' with Dependent Lifetime");

            UI.Concept(
                "Composition is 'has-a' where the contained object's lifetime is TIED to the container. " +
                "The container CREATES the contained object and is responsible for it. " +
                "When the container is gone, the contained object goes with it."
            );

            UI.Code("Composition — Car owns its Engine (engine can't exist independently)",
                "public class Engine",
                "{",
                "    public int Horsepower { get; }",
                "    public Engine(int hp) => Horsepower = hp;",
                "    public void Start() => Console.WriteLine($\"{Horsepower}hp engine running\");",
                "}",
                "",
                "public class Car",
                "{",
                "    public string Model { get; }",
                "    // COMPOSITION: Car CREATES its own Engine.",
                "    // Engine is not passed in from outside — it's born with the Car.",
                "    private readonly Engine _engine;",
                "",
                "    public Car(string model, int horsepower)",
                "    {",
                "        Model   = model;",
                "        _engine = new Engine(horsepower); // Car creates the engine",
                "    }",
                "",
                "    public void Start() => _engine.Start();",
                "    // Engine lives and dies with the Car.",
                "    // External code never holds a direct reference to _engine.",
                "}",
                "",
                "var car = new Car(\"Tesla Model S\", 670);",
                "car.Start();",
                "car = null;  // Car AND its Engine are now eligible for garbage collection.",
                "// You can't separately use the engine — there's no reference to it."
            );

            UI.Diagram("Composition Ownership",
                "  Car ◆──────────── Engine",
                "   │                  │",
                "   │  Car CREATES     │",
                "   │  Engine in its   │",
                "   │  constructor     │",
                "   │                  │",
                "   └── Car destroyed → Engine destroyed together"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_CompositionOverInheritance()
        {
            UI.SubHeader("Prefer Composition Over Inheritance — The Critical Principle");

            UI.Concept(
                "This is one of the most important design principles in software. " +
                "When you have a choice between inheritance and composition for code reuse, " +
                "CHOOSE COMPOSITION. Here's why:"
            );

            UI.Diagram("Why Composition Wins",
                "  INHERITANCE problems:                   COMPOSITION advantages:",
                "  ┌─────────────────────────────┐         ┌─────────────────────────────────┐",
                "  │ • Tight coupling to base    │         │ • Loose coupling via interfaces  │",
                "  │ • Can't change at runtime   │         │ • Swap behavior at runtime       │",
                "  │ • Base changes break children│        │ • Components independent         │",
                "  │ • Deep hierarchies = fragile │        │ • Each component testable alone  │",
                "  │ • 'Is-A' often isn't true   │         │ • Mix and match freely           │",
                "  └─────────────────────────────┘         └─────────────────────────────────┘"
            );

            UI.Code("Classic anti-pattern: Button inheriting from Rectangle",
                "// ❌ INHERITANCE APPROACH — seems logical at first:",
                "class Rectangle { public double Width, Height; public double Area() => Width * Height; }",
                "class Button : Rectangle  // Button IS-A Rectangle?",
                "{",
                "    public string Text { get; set; }",
                "    public void Click() { Console.WriteLine(\"Clicked: \" + Text); }",
                "}",
                "// Problem: all Rectangle methods are exposed on Button.",
                "// Can you call button.Area()? button.Width = -5? Doesn't make sense for a UI button.",
                "// Button IS-A Rectangle is FALSE in most contexts.",
                "",
                "// ✓ COMPOSITION APPROACH — Button HAS a size, doesn't IS-A Rectangle:",
                "class Size  { public double Width, Height; }",
                "class Button",
                "{",
                "    public Size   Dimensions { get; }  // HAS a size",
                "    public string Text       { get; }",
                "    public void Click() { Console.WriteLine(\"Clicked: \" + Text); }",
                "    // Only Button-relevant behavior exposed. Clean API.",
                "}"
            );

            UI.Code("Composition enables runtime behavior swapping (inheritance can't do this)",
                "// With composition: swap the formatter behavior at runtime",
                "public class ReportGenerator",
                "{",
                "    private IFormatter _formatter; // composed, not inherited",
                "",
                "    public ReportGenerator(IFormatter formatter) => _formatter = formatter;",
                "",
                "    // ✓ Behavior can be SWAPPED at runtime:",
                "    public void SetFormatter(IFormatter f) => _formatter = f;",
                "",
                "    public string Generate(string data) => _formatter.Format(data);",
                "}",
                "",
                "var gen = new ReportGenerator(new MarkdownFormatter());",
                "gen.Generate(\"**Bold** data\");  // Markdown formatted",
                "",
                "gen.SetFormatter(new HtmlFormatter()); // Swap at runtime!",
                "gen.Generate(\"**Bold** data\");  // Now HTML formatted",
                "",
                "// Inheritance can't do this — the behavior is baked in at compile time."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_LiveDemo()
        {
            UI.SubHeader("Live Demo — Document Editor Built with Composition");

            UI.Print("The Document class (defined at top of this file) uses composition to wire");
            UI.Print("together a SpellChecker, AutoSaver, and Formatter. Let's watch it:\n");

            UI.LiveDemo("Composition in the document editor", () =>
            {
                // Wire up the components (could swap any of these with different implementations)
                var doc = new Document(
                    title: "My Report",
                    filePath: "C:/docs/report.txt",
                    spellChecker: new BasicSpellChecker(),
                    autoSaver:    new LocalAutoSaver(),
                    formatter:    new MarkdownFormatter()
                );

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     doc.Type(\"This is a great report adn it is recieve worthy.\");");
                Console.ResetColor();
                doc.Type("This is a great report adn it is recieve worthy.");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     doc.Type(\"The results are conclusive and the data is clean.\");");
                Console.ResetColor();
                doc.Type("\nThe results are conclusive and the data is clean.");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Key: Document doesn't care HOW spell checking or saving works.");
                Console.WriteLine("     Swap BasicSpellChecker for AzureSpellChecker — zero Document code changes.");
                Console.ResetColor();
            });

            UI.Print("");
            UI.Print("AGGREGATION DEMO:\n");

            UI.LiveDemo("Employees existing independently of Departments", () =>
            {
                var emp1 = new EmployeeRef(1, "Deepak");
                var emp2 = new EmployeeRef(2, "Priya");
                var emp3 = new EmployeeRef(3, "Carlos");

                var engineering = new DepartmentDemo("Engineering");
                var hr          = new DepartmentDemo("Human Resources");

                engineering.AddEmployee(emp1);
                engineering.AddEmployee(emp2);
                hr.AddEmployee(emp3);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     Engineering dept: {engineering.Count} employees");
                Console.WriteLine($"     HR dept: {hr.Count} employee");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n     Transferring Priya from Engineering to HR...");
                engineering.RemoveEmployee(emp2.Id);
                hr.AddEmployee(emp2);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     Engineering dept: {engineering.Count} employee");
                Console.WriteLine($"     HR dept: {hr.Count} employees");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     emp2 (Priya) still exists and was moved between departments.");
                Console.WriteLine("     The EMPLOYEE object is independent of the DEPARTMENT container.");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE:\n\n" +
                "You're designing a 'Computer' class.\n\n" +
                "Classify each component's relationship to Computer:\n" +
                "  • CPU (Central Processing Unit)\n" +
                "  • RAM (Memory sticks)\n" +
                "  • USB Keyboard (plugged in)\n" +
                "  • External Hard Drive (pluggable)\n" +
                "  • Operating System\n\n" +
                "Is each COMPOSITION, AGGREGATION, or ASSOCIATION?",

                "ANSWERS:\n\n" +
                "  CPU (soldered on motherboard)  → COMPOSITION\n" +
                "     Can't be removed and used elsewhere. Part of the computer.\n\n" +
                "  RAM (memory sticks)             → COMPOSITION (once installed)\n" +
                "     Typically specific to the board config. Borderline aggregation.\n\n" +
                "  USB Keyboard                   → AGGREGATION\n" +
                "     Plugged in — exists independently, works with any computer.\n\n" +
                "  External Hard Drive            → AGGREGATION\n" +
                "     Completely independent. Works on any computer, stores own data.\n\n" +
                "  Operating System (installed)   → COMPOSITION\n" +
                "     The OS is configured for THIS machine. Not easily transferred.\n\n" +
                "  User (person at keyboard)      → ASSOCIATION\n" +
                "     User uses the computer. Neither owns the other."
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Relationships", new QuizQuestion[]
            {
                new(
                    "What is the main difference between Composition and Aggregation?",
                    new[] {
                        "Composition uses classes; Aggregation uses interfaces",
                        "In Composition, the part's lifetime depends on the whole (owned/created by it). In Aggregation, the part exists independently.",
                        "Aggregation is stronger than Composition",
                        "They are the same — just different names for has-a relationship"
                    },
                    1,
                    "Composition: owner creates and is responsible for the part (Engine in Car). Aggregation: the part is passed in and exists independently (Employee in Department)."
                ),
                new(
                    "Why is 'Prefer Composition Over Inheritance' a design principle?",
                    new[] {
                        "Composition is faster at runtime than inheritance",
                        "Inheritance was deprecated in modern C#",
                        "Composition provides looser coupling, allows runtime behavior swapping, and avoids the fragile base class problem",
                        "Interfaces can't be used with inheritance"
                    },
                    2,
                    "Composition gives flexibility: swap components, test in isolation, change behavior at runtime. Inheritance creates tight coupling and the hierarchy changes affect all children."
                ),
                new(
                    "A Teacher has a list of Student references but doesn't create them. Students can be in multiple Teachers' lists. This is:",
                    new[] {
                        "Composition — Teacher owns the students",
                        "Aggregation — Teacher has students but they exist independently",
                        "Association — just a method parameter relationship",
                        "Inheritance — Teacher IS-A Student type"
                    },
                    1,
                    "Aggregation: Teacher holds references to existing Students, but doesn't own them. Students can exist in multiple lists and outlive any one Teacher."
                ),
                new(
                    "What is the 'Fragile Base Class Problem'?",
                    new[] {
                        "Base classes are more likely to have bugs than derived classes",
                        "Changing a base class can unexpectedly break derived classes that depend on specific inherited behavior",
                        "Base classes use more memory than derived classes",
                        "Abstract base classes can't be unit tested"
                    },
                    1,
                    "When the base class changes internal behavior, derived classes that inherited and depended on that behavior can break unexpectedly. This is why deep inheritance hierarchies are risky."
                ),
            });
        }
    }
}
