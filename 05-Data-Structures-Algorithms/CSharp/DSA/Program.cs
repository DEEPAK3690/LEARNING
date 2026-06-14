using DSA.OOP;
using DSA.Patterns;

namespace DSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ── MODE SELECTION ────────────────────────────────────────
            // Choose between the OOP Learning app and the original DSA work.
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  ╔═══════════════════════════════════════════╗");
            Console.WriteLine("  ║        DEEPAK'S LEARNING PLATFORM        ║");
            Console.WriteLine("  ╚═══════════════════════════════════════════╝\n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [1]  OOP Mastery Guide  — Interactive learning app");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  [2]  DSA Practice       — Data structures & algorithms");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n  Choose (1 or 2): ");
            Console.ResetColor();

            var key = Console.ReadKey(intercept: true).KeyChar;
            Console.WriteLine();

            if (key == '2')
            {
                // ── ORIGINAL DSA CODE ─────────────────────────────────
                Console.WriteLine("DSA mode:");
                Logical.matchchecker();
            }
            else
            {
                // ── OOP LEARNING APP (default) ────────────────────────
                OOPProgram.Run();
            }
        }
    }
}
