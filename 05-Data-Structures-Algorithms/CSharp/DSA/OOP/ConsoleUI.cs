// ============================================================
// FILE: ConsoleUI.cs
// PURPOSE: All display utilities for the OOP Learning App.
//
// Think of this as the "view layer" of our application.
// Every chapter uses these helpers so the visual style stays
// consistent. If you ever want to change colors or formatting,
// you only change this ONE file — that's the Single Responsibility
// Principle (SRP) in action, which we'll learn in Chapter 8!
// ============================================================

namespace DSA.OOP
{
    // --- DATA STRUCTURE: represents one multiple-choice question ---
    // 'record' is a modern C# feature (C# 9+). It creates an immutable
    // data container. Perfect for quiz questions that should never change
    // after they're created.
    internal record QuizQuestion(
        string Question,        // The question text shown to the user
        string[] Options,       // Array of answer choices (A, B, C, D)
        int CorrectIndex,       // 0=A, 1=B, 2=C, 3=D
        string Explanation      // Why the correct answer is correct
    );

    // 'static' means you never need to create an object of this class.
    // You just call UI.Header("text") directly.
    // This makes sense here because UI is a utility — it has no state of its own.
    internal static class UI
    {
        // ── PRIVATE CONSTANTS ──────────────────────────────────────────
        // Box-drawing characters. These create the nice bordered boxes.
        // We define them ONCE here instead of repeating the string everywhere.
        // This is DRY: Don't Repeat Yourself — a core developer habit.
        private const char TL = '╔', TR = '╗', BL = '╚', BR = '╝';
        private const char H  = '═', V  = '║';
        private const char ML = '╠', MR = '╣';

        // Console width we'll use for formatting
        private const int Width = 76;

        // ── PUBLIC COLOR-CODED PRINT METHODS ──────────────────────────

        // Prints a full-width chapter header box in cyan
        // Example output:
        // ╔══════════════════════════════════╗
        // ║   CHAPTER 1: CLASSES & OBJECTS   ║
        // ╚══════════════════════════════════╝
        public static void Header(string title)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.Cyan);
            string top    = TL + new string(H, Width) + TR;
            string middle = V + title.PadLeft((Width + title.Length) / 2).PadRight(Width) + V;
            string bottom = BL + new string(H, Width) + BR;
            Console.WriteLine(top);
            Console.WriteLine(middle);
            Console.WriteLine(bottom);
            ResetColor();
            Console.WriteLine();
        }

        // Prints a section sub-header in dark cyan
        public static void SubHeader(string title)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkCyan);
            Console.WriteLine($"  ▶  {title.ToUpper()}");
            Console.WriteLine("  " + new string('─', Width - 2));
            ResetColor();
        }

        // Prints a concept explanation — used for core definitions
        // Color: White (readable, neutral — this is the "main text")
        public static void Concept(string text)
        {
            SetColor(ConsoleColor.White);
            PrintWrapped(text, "  ");
            ResetColor();
        }

        // Prints a real-world analogy box — helps learners connect
        // abstract code ideas to familiar real-world things
        public static void Analogy(string title, string text)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkYellow);
            Console.WriteLine($"  🌍  REAL-WORLD ANALOGY: {title}");
            SetColor(ConsoleColor.Yellow);
            PrintWrapped(text, "     ");
            ResetColor();
            Console.WriteLine();
        }

        // Prints a highlighted KEY POINT callout box
        public static void KeyPoint(string text)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.Green);
            Console.WriteLine("  ╔" + new string('─', Width - 2) + "╗");
            Console.Write("  ║  KEY POINT: ");
            SetColor(ConsoleColor.White);
            // Print text, then pad right to fill the box width
            string content = text.Length > Width - 16 ? text[..(Width - 18)] + ".." : text;
            Console.Write(content.PadRight(Width - 14));
            SetColor(ConsoleColor.Green);
            Console.WriteLine("║");
            Console.WriteLine("  ╚" + new string('─', Width - 2) + "╝");
            ResetColor();
            Console.WriteLine();
        }

        // Prints a block of code lines in yellow with a grey header bar.
        // The 'params' keyword means you can pass lines as individual args
        // OR as an array — convenient for callers.
        public static void Code(string label, params string[] lines)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkGray);
            Console.WriteLine($"  ┌── CODE: {label} " + new string('─', Math.Max(0, Width - 12 - label.Length)) + "┐");
            ResetColor();

            int lineNum = 1;
            foreach (var line in lines)
            {
                // Line number in dark gray (like an IDE)
                SetColor(ConsoleColor.DarkGray);
                Console.Write($"  │ {lineNum++,2} │ ");

                // Comments (// ...) in dark green — like IDE syntax highlighting
                if (line.TrimStart().StartsWith("//") || line.TrimStart().StartsWith("/*"))
                {
                    SetColor(ConsoleColor.DarkGreen);
                }
                else if (line.TrimStart().StartsWith("❌") || line.TrimStart().StartsWith("BAD"))
                {
                    SetColor(ConsoleColor.Red);
                }
                else
                {
                    SetColor(ConsoleColor.Yellow);
                }
                Console.WriteLine(line);
            }

            SetColor(ConsoleColor.DarkGray);
            Console.WriteLine("  └" + new string('─', Width) + "┘");
            ResetColor();
            Console.WriteLine();
        }

        // Prints a good practice example (green ✓)
        public static void Good(string label, string text)
        {
            SetColor(ConsoleColor.Green);
            Console.WriteLine($"  ✓  GOOD — {label}");
            SetColor(ConsoleColor.DarkGreen);
            PrintWrapped(text, "     ");
            ResetColor();
        }

        // Prints a bad practice / anti-pattern example (red ✗)
        public static void Bad(string label, string text)
        {
            SetColor(ConsoleColor.Red);
            Console.WriteLine($"  ✗  AVOID — {label}");
            SetColor(ConsoleColor.DarkRed);
            PrintWrapped(text, "     ");
            ResetColor();
        }

        // Prints an ASCII diagram — used for mental models
        public static void Diagram(string title, params string[] lines)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkCyan);
            Console.WriteLine($"  DIAGRAM: {title}");
            Console.WriteLine("  " + new string('·', Width - 2));
            SetColor(ConsoleColor.Cyan);
            foreach (var line in lines)
                Console.WriteLine("  " + line);
            SetColor(ConsoleColor.DarkCyan);
            Console.WriteLine("  " + new string('·', Width - 2));
            ResetColor();
            Console.WriteLine();
        }

        // Prints a "common mistake" warning block
        public static void Mistake(string mistake, string fix)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.Red);
            Console.WriteLine("  ⚠  COMMON MISTAKE:");
            SetColor(ConsoleColor.DarkRed);
            PrintWrapped(mistake, "     ");
            SetColor(ConsoleColor.Green);
            Console.WriteLine("  ✓  FIX:");
            SetColor(ConsoleColor.DarkGreen);
            PrintWrapped(fix, "     ");
            ResetColor();
            Console.WriteLine();
        }

        // Prints an exercise block where the learner thinks before seeing the answer
        public static void Exercise(string prompt, string answer)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.Magenta);
            Console.WriteLine("  ┌" + new string('─', Width - 1));
            Console.WriteLine("  │  ✏  EXERCISE — Think before looking!");
            Console.WriteLine("  └" + new string('─', Width - 1));
            ResetColor();
            SetColor(ConsoleColor.White);
            PrintWrapped(prompt, "  ");
            ResetColor();

            Pause("Think about it, then press any key to reveal the answer...");

            SetColor(ConsoleColor.Green);
            Console.WriteLine("  ANSWER:");
            SetColor(ConsoleColor.DarkGreen);
            PrintWrapped(answer, "  ");
            ResetColor();
            Console.WriteLine();
        }

        // Runs a full quiz. Returns the score for potential progress tracking.
        public static int RunQuiz(string title, QuizQuestion[] questions)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.Magenta);
            Console.WriteLine($"  ╔{new string('═', Width)}╗");
            Console.WriteLine($"  ║  📝  QUIZ: {title,-63}║");
            Console.WriteLine($"  ╚{new string('═', Width)}╝");
            ResetColor();
            Console.WriteLine();

            int score = 0;

            for (int i = 0; i < questions.Length; i++)
            {
                var q = questions[i];
                SetColor(ConsoleColor.White);
                Console.WriteLine($"  Q{i + 1} of {questions.Length}: {q.Question}");
                Console.WriteLine();

                // Print each option
                char[] labels = { 'A', 'B', 'C', 'D' };
                for (int j = 0; j < q.Options.Length; j++)
                {
                    SetColor(j == q.CorrectIndex ? ConsoleColor.White : ConsoleColor.Gray);
                    Console.WriteLine($"     [{labels[j]}]  {q.Options[j]}");
                }
                Console.WriteLine();

                // Get answer
                SetColor(ConsoleColor.Yellow);
                Console.Write("  Your answer (A/B/C/D): ");
                ResetColor();

                char answer = GetChoiceKey(q.Options.Length);
                int answerIndex = answer - 'A';
                Console.WriteLine(answer);

                if (answerIndex == q.CorrectIndex)
                {
                    score++;
                    SetColor(ConsoleColor.Green);
                    Console.WriteLine("  ✓  Correct!");
                }
                else
                {
                    SetColor(ConsoleColor.Red);
                    Console.WriteLine($"  ✗  Not quite. The answer was [{labels[q.CorrectIndex]}].");
                }

                SetColor(ConsoleColor.DarkCyan);
                Console.WriteLine($"     💡 {q.Explanation}");
                ResetColor();
                Console.WriteLine();
            }

            // Score summary
            double pct = (double)score / questions.Length * 100;
            SetColor(pct >= 70 ? ConsoleColor.Green : ConsoleColor.Yellow);
            Console.WriteLine($"  SCORE: {score}/{questions.Length}  ({pct:F0}%)  " +
                (pct == 100 ? "🏆 Perfect!" : pct >= 70 ? "✓ Solid understanding!" : "📖 Review this chapter again."));
            ResetColor();
            Console.WriteLine();

            return score;
        }

        // Shows the "press any key" pause prompt
        public static void Pause(string message = "Press any key to continue...")
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkGray);
            Console.Write($"  [{message}] ");
            ResetColor();
            Console.ReadKey(intercept: true);
            Console.WriteLine();
            Console.WriteLine();
        }

        // Separator line
        public static void Separator()
        {
            SetColor(ConsoleColor.DarkGray);
            Console.WriteLine("  " + new string('·', Width - 2));
            ResetColor();
        }

        // Print a "live demo" — shows what code would produce
        public static void LiveDemo(string label, Action demo)
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkGray);
            Console.WriteLine($"  ┌── LIVE DEMO: {label} " + new string('─', Math.Max(0, Width - 15 - label.Length)) + "┐");
            SetColor(ConsoleColor.White);
            demo();
            SetColor(ConsoleColor.DarkGray);
            Console.WriteLine("  └" + new string('─', Width) + "┘");
            ResetColor();
            Console.WriteLine();
        }

        // Print normal text
        public static void Print(string text = "")
        {
            SetColor(ConsoleColor.White);
            if (string.IsNullOrEmpty(text))
                Console.WriteLine();
            else
                PrintWrapped(text, "  ");
            ResetColor();
        }

        // Print a menu item
        public static void MenuItem(string key, string label, string description = "")
        {
            SetColor(ConsoleColor.Yellow);
            Console.Write($"   [{key}]  ");
            SetColor(ConsoleColor.White);
            Console.Write(label);
            if (!string.IsNullOrEmpty(description))
            {
                SetColor(ConsoleColor.DarkGray);
                Console.Write($"  —  {description}");
            }
            Console.WriteLine();
            ResetColor();
        }

        // Clear screen and reset
        public static void Clear()
        {
            Console.Clear();
            ResetColor();
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────

        private static void SetColor(ConsoleColor c) => Console.ForegroundColor = c;
        private static void ResetColor() => Console.ResetColor();

        // Wraps long text at word boundaries so it fits in the console
        private static void PrintWrapped(string text, string indent)
        {
            int maxWidth = Width - indent.Length;
            var words = text.Split(' ');
            var line = indent;

            foreach (var word in words)
            {
                if (line.Length + word.Length + 1 > Width)
                {
                    Console.WriteLine(line);
                    line = indent + word + " ";
                }
                else
                {
                    line += word + " ";
                }
            }
            if (line.Trim().Length > 0)
                Console.WriteLine(line);
        }

        // Reads a single key press and validates it's a valid choice (A-D)
        private static char GetChoiceKey(int optionCount)
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true).KeyChar;
                char upper = char.ToUpper(key);
                if (upper >= 'A' && upper < 'A' + optionCount)
                    return upper;
                // Invalid key — flash red and ask again
                Console.Write("\b \b"); // erase the character
            }
        }
    }
}
