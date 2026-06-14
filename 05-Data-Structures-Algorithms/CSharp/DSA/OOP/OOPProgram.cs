// ============================================================
// FILE: OOPProgram.cs
// PURPOSE: Main entry point and navigation hub for the OOP
//          Learning Application.
//
// This is the "orchestrator" — it doesn't do the teaching itself,
// it delegates to each chapter class. This is the Single
// Responsibility Principle: this class is only responsible for
// navigation, not for content. Each chapter handles its own content.
// ============================================================

namespace DSA.OOP
{
    internal static class OOPProgram
    {
        // This is the array of chapter titles and their corresponding Run() methods.
        // Using a tuple array (string, Action) — elegant and readable.
        // 'Action' is a delegate (a reference to a method with no params, no return).
        // This pattern is called the "Command" pattern — we'll learn it in Chapter 9!
        private static readonly (string Title, Action Run)[] Chapters =
        {
            ("Developer Mindset — How to Think Like a Software Engineer", Ch00_Mindset.Run),
            ("Chapter 1:  Classes & Objects — The Blueprint and the Thing", Ch01_ClassesObjects.Run),
            ("Chapter 2:  Encapsulation — Guard Your Object's State", Ch02_Encapsulation.Run),
            ("Chapter 3:  Abstraction — Show the Wheel, Hide the Engine", Ch03_Abstraction.Run),
            ("Chapter 4:  Inheritance — Reuse Without Rewriting", Ch04_Inheritance.Run),
            ("Chapter 5:  Polymorphism — One Interface, Many Behaviors", Ch05_Polymorphism.Run),
            ("Chapter 6:  Relationships — Composition vs Aggregation", Ch06_Relationships.Run),
            ("Chapter 7:  Interfaces — Contracts Without Implementation", Ch07_Interfaces.Run),
            ("Chapter 8:  SOLID Principles — The 5 Laws of Good Design", Ch08_SOLID.Run),
            ("Chapter 9:  Design Patterns — Proven Solutions to Common Problems", Ch09_Patterns.Run),
            ("Chapter 10: Real-World Project — E-Commerce Order System", Ch10_Project.Run),
        };

        // Entry point called from Program.cs
        public static void Run()
        {
            ShowWelcome();

            while (true)
            {
                ShowMenu();

                var key = Console.ReadKey(intercept: true).KeyChar;
                Console.WriteLine();

                // '0' exits the app
                if (key == '0') break;

                // Convert '1'–'9' and 'A'–'B' to chapter index
                int index = GetChapterIndex(key);

                if (index >= 0 && index < Chapters.Length)
                {
                    UI.Clear();
                    // Call the chapter's Run() method via the stored delegate
                    Chapters[index].Run();
                    UI.Pause("Chapter complete! Press any key to return to the main menu...");
                }
                else
                {
                    UI.Print("  Invalid choice. Please press a key shown in the menu.");
                    System.Threading.Thread.Sleep(800);
                }
            }

            ShowGoodbye();
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────

        private static void ShowWelcome()
        {
            UI.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(@"   ██████╗  ██████╗ ██████╗     ███╗   ███╗ █████╗ ███████╗████████╗███████╗██████╗ ██╗   ██╗");
            Console.WriteLine(@"  ██╔═══██╗██╔═══██╗██╔══██╗    ████╗ ████║██╔══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗╚██╗ ██╔╝");
            Console.WriteLine(@"  ██║   ██║██║   ██║██████╔╝    ██╔████╔██║███████║███████╗   ██║   █████╗  ██████╔╝ ╚████╔╝ ");
            Console.WriteLine(@"  ██║   ██║██║   ██║██╔═══╝     ██║╚██╔╝██║██╔══██║╚════██║   ██║   ██╔══╝  ██╔══██╗  ╚██╔╝  ");
            Console.WriteLine(@"  ╚██████╔╝╚██████╔╝██║         ██║ ╚═╝ ██║██║  ██║███████║   ██║   ███████╗██║  ██║   ██║   ");
            Console.WriteLine(@"   ╚═════╝  ╚═════╝ ╚═╝         ╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝   ╚═╝  ");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  Object-Oriented Programming — Complete Interactive Learning Guide");
            Console.WriteLine("  From Zero to Production-Ready, one concept at a time.");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ─────────────────────────────────────────────────────────────────────────────");
            Console.WriteLine("  HOW TO USE THIS APP:");
            Console.WriteLine("   • Work through chapters in ORDER — each builds on the previous one.");
            Console.WriteLine("   • Don't rush. Read every line. Pause to think before answers.");
            Console.WriteLine("   • After each chapter: close the app and re-explain the concept aloud.");
            Console.WriteLine("   • Return to chapters you scored below 70% on the quiz.");
            Console.WriteLine("  ─────────────────────────────────────────────────────────────────────────────");
            Console.ResetColor();
            UI.Pause("Ready? Press any key to see the chapter menu...");
        }

        private static void ShowMenu()
        {
            UI.Clear();
            UI.Header("OOP MASTERY GUIDE — CHAPTER MENU");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  Select a chapter to study (start from [0] if you're new):");
            Console.WriteLine();
            Console.ResetColor();

            // Print chapters 0–9 with number keys
            // Chapters 10 uses 'A', 'B' (since keyboard only has single chars)
            string[] keys = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B" };

            for (int i = 0; i < Chapters.Length; i++)
                UI.MenuItem(keys[i + 1], Chapters[i].Title);

            Console.WriteLine();
            UI.MenuItem("0", "Exit", "Close the learning app");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  Your choice: ");
            Console.ResetColor();
        }

        private static void ShowGoodbye()
        {
            UI.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║   Well done for putting in the work. Keep building!         ║");
            Console.WriteLine("  ║                                                              ║");
            Console.WriteLine("  ║   Reminder: Read code. Write code. Break things. Fix them.  ║");
            Console.WriteLine("  ║   That's the loop that turns a learner into an engineer.    ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Convert the pressed key character to a chapter array index
        private static int GetChapterIndex(char key)
        {
            char upper = char.ToUpper(key);
            if (upper >= '1' && upper <= '9') return upper - '1';      // '1'→0, '9'→8
            if (upper == 'A') return 9;
            if (upper == 'B') return 10;
            return -1; // invalid
        }
    }
}
