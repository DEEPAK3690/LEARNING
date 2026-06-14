// ============================================================
// FILE: Ch00_Mindset.cs
// PURPOSE: Before we write a single line of OOP code, we need
//          to understand HOW experienced developers think.
//          This chapter rewires your mental approach to software.
// ============================================================

namespace DSA.OOP
{
    internal static class Ch00_Mindset
    {
        public static void Run()
        {
            UI.Header("CHAPTER 0: THE DEVELOPER MINDSET");
            UI.Print("Before you learn OOP syntax, you need to think like a developer.");
            UI.Print("Syntax is easy to look up. Mindset is what separates average coders from great engineers.");
            UI.Pause();

            Section1_ProblemSolvingFirst();
            Section2_HowToReadRequirements();
            Section3_BreakingProblemsDown();
            Section4_ReadingCode();
            Section5_Debugging();
            Section6_WritingMaintainableCode();
            Section7_HabitsOfGreatEngineers();
            Section8_HowToKeepLearning();
            RunQuiz();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section1_ProblemSolvingFirst()
        {
            UI.SubHeader("1. The #1 Skill: Problem Solving Before Coding");

            UI.Concept(
                "The biggest mistake beginners make is opening their IDE and immediately " +
                "starting to type code. Experienced developers THINK first, code second. " +
                "Often the thinking takes longer than the coding — and that is completely normal."
            );

            UI.Analogy("Building a House",
                "Imagine hiring a builder and they start laying bricks on day one without " +
                "blueprints. The foundation is in the wrong spot. Now they have to tear it down. " +
                "Software is the same. Rushing to code without a plan creates code that needs " +
                "to be rewritten. Time spent planning is never wasted."
            );

            UI.Diagram("The Developer's Problem-Solving Loop",
                "┌─────────────────────────────────────────────────────────────┐",
                "│                                                             │",
                "│   1. UNDERSTAND  ──▶  2. PLAN  ──▶  3. CODE  ──▶  4. TEST │",
                "│        │                                         │          │",
                "│        └────────────── (repeat) ────────────────┘          │",
                "│                                                             │",
                "│  Don't move to step 3 until you can explain step 1 and 2  │",
                "│  in plain English to someone who doesn't know programming. │",
                "└─────────────────────────────────────────────────────────────┘"
            );

            UI.SubHeader("The Five Questions Before Writing Code");

            UI.Print("Ask these five questions EVERY time you face a programming task:\n");

            string[][] questions = {
                new[]{"1. WHAT is the problem?",
                      "State it clearly in one sentence. If you can't, you don't understand it yet."},
                new[]{"2. WHAT are the inputs and outputs?",
                      "What data goes in? What should come out? What happens with invalid input?"},
                new[]{"3. WHAT are the constraints?",
                      "Speed requirements? Memory limits? Which devices? Which browsers? API rate limits?"},
                new[]{"4. WHAT are edge cases?",
                      "Empty input, maximum values, null, zero, duplicate data — your code must handle these."},
                new[]{"5. WHAT's the simplest solution first?",
                      "Start simple. You can optimize later. Working code beats perfect-but-unfinished code."},
            };

            foreach (var q in questions)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  {q[0]}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"  → {q[1]}");
            }
            Console.ResetColor();

            UI.KeyPoint("Never start coding until you can explain the problem and your plan in plain English.");
            UI.Pause();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section2_HowToReadRequirements()
        {
            UI.SubHeader("2. How to Read Requirements Like an Engineer");

            UI.Concept(
                "In real projects, requirements are rarely perfect. They're written by product managers, " +
                "clients, or business analysts — not developers. Your job is to read between the lines " +
                "and ask the right clarifying questions."
            );

            UI.Diagram("What a Requirement Says vs What It Means",
                "┌──────────────────────────────────────────────────────────────┐",
                "│  Client says: 'Make it fast.'                               │",
                "│  Engineer asks: 'Fast compared to what? Under what load?    │",
                "│                  How many users? What's the acceptable       │",
                "│                  response time? 200ms? 2s?'                 │",
                "├──────────────────────────────────────────────────────────────┤",
                "│  Client says: 'Users should be able to log in.'             │",
                "│  Engineer asks: 'Email/password? Google SSO? Multi-factor?  │",
                "│                  What happens after 5 failed attempts?       │",
                "│                  How long are sessions valid?'              │",
                "└──────────────────────────────────────────────────────────────┘"
            );

            UI.Print("WHAT TO LOOK FOR when reading requirements:\n");

            var points = new[]
            {
                ("Vague words",        "'Good performance', 'easy to use', 'secure' — push for specific numbers."),
                ("Hidden rules",       "If the user is an admin, they can delete. Who else can delete? No one?"),
                ("Missing scenarios",  "What if the network fails? What if the database is down? What if input is empty?"),
                ("Conflicting needs",  "'It must be both fast AND store everything forever.' — You may need to trade off."),
                ("Unstated context",   "Who are the users? What devices? What volume? What regulations apply?"),
            };

            foreach (var (term, desc) in points)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  • {term,-22}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(desc);
            }
            Console.ResetColor();

            UI.Exercise(
                "PRACTICE: Read this requirement and identify what's missing:\n\n" +
                "  'Build a form where users can submit their name and email. " +
                "The form should validate input and store the data.'\n\n" +
                "Write down at least 5 questions you'd ask before coding.",

                "Questions you should have asked:\n" +
                "  1. What counts as 'valid' — email format? Name length? No special chars?\n" +
                "  2. Where is data stored — SQL DB? Firebase? In-memory?\n" +
                "  3. What happens after submission — success message? Email confirmation?\n" +
                "  4. What happens if the email already exists in the database?\n" +
                "  5. Is there a rate limit — can one person submit 1000 times?\n" +
                "  6. What data privacy rules apply? GDPR? Do we need consent checkbox?\n" +
                "  7. What devices / browsers must this work on?"
            );
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section3_BreakingProblemsDown()
        {
            UI.SubHeader("3. Breaking Large Problems Into Small Tasks");

            UI.Concept(
                "Every large software project feels overwhelming at first. " +
                "The professional response is NOT to panic and start randomly coding. " +
                "It's to recursively break the problem down until each piece is so small " +
                "you could implement it in under an hour."
            );

            UI.Analogy("Eating an Elephant",
                "How do you eat an elephant? One bite at a time. " +
                "How do you build a banking app? One small feature at a time. " +
                "Log in → Create account → View balance → Make transfer → View history. " +
                "Each of those is a bite. Never try to swallow the whole elephant at once."
            );

            UI.Diagram("Decomposition Tree — Building a 'Login' Feature",
                "LOGIN FEATURE",
                "├── UI: Login form (HTML/XAML/etc)",
                "│   ├── Email input field with validation",
                "│   ├── Password input field (masked)",
                "│   └── Submit button → calls API",
                "├── API: POST /api/auth/login",
                "│   ├── Validate request body (not empty, valid email format)",
                "│   ├── Find user in database by email",
                "│   ├── Verify password hash (bcrypt compare)",
                "│   ├── Generate JWT token with expiry",
                "│   └── Return token or error response",
                "└── DATABASE: Users table",
                "    ├── Column: Id (int, PK, auto-increment)",
                "    ├── Column: Email (varchar, unique, indexed)",
                "    └── Column: PasswordHash (varchar)"
            );

            UI.Print("THE DECOMPOSITION PROCESS:\n");
            UI.Print("  Step 1: Write down the big goal in one sentence.");
            UI.Print("  Step 2: List the major parts (features/components).");
            UI.Print("  Step 3: For each part, list the sub-tasks.");
            UI.Print("  Step 4: For each sub-task, estimate complexity (1-3 hours).");
            UI.Print("  Step 5: If any task is > 3 hours, break it down further.");
            UI.Print("  Step 6: Start with the most uncertain/risky piece first.");
            UI.Print("         (Don't leave the hard parts for last — derisk early!)");

            UI.KeyPoint("A task you can complete in 1 hour is a task you can make progress on right now.");
            UI.Pause();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section4_ReadingCode()
        {
            UI.SubHeader("4. How to Read and Understand Code You Didn't Write");

            UI.Concept(
                "Professional developers spend MORE time reading code than writing it. " +
                "You'll inherit legacy systems, read open-source libraries, review teammates' PRs. " +
                "Being a great code reader is as important as being a great code writer."
            );

            UI.Print("THE STRATEGY FOR READING UNFAMILIAR CODE:\n");

            var steps = new[]
            {
                ("Start from the entry point", "Find main(), App.Run(), or whatever starts execution. Follow the flow."),
                ("Read top-down, not left-right","Understand the structure before the details. Skip method bodies first."),
                ("Name variables as you read", "Build a mental model. 'This is the user's bank balance. This is the list of transactions.'"),
                ("Use the debugger, not just eyes","Step through the code. Watch real values change. More reliable than guessing."),
                ("Read tests first", "Tests tell you WHAT the code is supposed to do. Read tests before reading the implementation."),
                ("Look for patterns", "Once you know OOP and design patterns, you'll recognize them instantly. This chapter is your key."),
            };

            foreach (var (title, desc) in steps)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n  ▸ {title}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"    {desc}");
            }
            Console.ResetColor();

            UI.Mistake(
                "Reading code by trying to understand every line before moving on. " +
                "You get lost in details and miss the big picture.",
                "Read for STRUCTURE first. Understand what each class/method is RESPONSIBLE FOR " +
                "before you dig into HOW it implements that responsibility."
            );
            UI.Pause();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section5_Debugging()
        {
            UI.SubHeader("5. How to Debug Effectively");

            UI.Concept(
                "Debugging is not typing random things until the error goes away. " +
                "It's a systematic process of forming hypotheses and testing them. " +
                "The best debuggers are logical, calm, and methodical — not lucky."
            );

            UI.Diagram("The Debugging Scientific Method",
                "┌─────────────────────────────────────────────────────────────┐",
                "│  1. OBSERVE    → What EXACTLY is the symptom? Error message?│",
                "│                  Wrong output? App crash? Slow performance?  │",
                "│                                                             │",
                "│  2. REPRODUCE  → Can you make it happen consistently?       │",
                "│                  If not, it might be race condition / timing.│",
                "│                                                             │",
                "│  3. ISOLATE    → Binary search the problem. Is it in this   │",
                "│                  function? In this class? In this layer?    │",
                "│                                                             │",
                "│  4. HYPOTHESIZE→ 'I think the problem is X because Y.'     │",
                "│                  State a specific, falsifiable theory.       │",
                "│                                                             │",
                "│  5. TEST       → Make one change. One. Did it fix it?       │",
                "│                  If yes: WHY did that fix it? Understand it. │",
                "│                  If no: back to step 3.                    │",
                "└─────────────────────────────────────────────────────────────┘"
            );

            UI.Print("RUBBER DUCK DEBUGGING:\n");
            UI.Concept(
                "Explain your bug to a rubber duck (or a colleague, or out loud to yourself). " +
                "The act of articulating the problem forces your brain to organize the information " +
                "differently. You'll often discover the bug mid-explanation. This works every single time."
            );

            UI.Print("\nCOMMON DEBUGGING TOOLS IN .NET:\n");
            var tools = new[]
            {
                ("Breakpoints", "Pause execution at any line. Inspect variable values at that moment."),
                ("Watch Window", "Track specific variables as you step through code."),
                ("Call Stack",   "See which method called which. Trace back to the source of the problem."),
                ("Console.WriteLine", "The oldest trick. Print values to see what's actually happening."),
                ("Exception Details", "Read the FULL exception message AND the inner exception. Don't skim."),
                ("Git Blame / Log",   "When did this break? Find the commit that introduced the bug."),
            };

            foreach (var (tool, desc) in tools)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  • {tool,-25}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(desc);
            }
            Console.ResetColor();

            UI.KeyPoint("When you fix a bug, write a test that would have caught it. Never fix the same bug twice.");
            UI.Pause();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section6_WritingMaintainableCode()
        {
            UI.SubHeader("6. Writing Code That Others (and Future You) Can Maintain");

            UI.Concept(
                "Code is read far more often than it is written. " +
                "You will spend most of your career reading and modifying existing code — " +
                "code written by past-you, your team, or someone who left the company years ago. " +
                "Writing maintainable code is an act of respect for your future self and your team."
            );

            UI.Print("THE PRINCIPLES OF MAINTAINABLE CODE:\n");

            UI.Code("Naming — the most powerful tool",
                "// BAD — what does 'x' hold? What is 'process'?",
                "int x = 85;",
                "bool process(List<int> d) { ... }",
                "",
                "// GOOD — names document intent without comments",
                "int employeeAge = 85;",
                "bool IsEligibleForRetirement(List<Employee> employees) { ... }",
                "",
                "// RULE: A good name eliminates the need for a comment.",
                "// If you need a comment to explain what a variable is, rename it."
            );

            UI.Code("Functions — one job, one responsibility",
                "// BAD — this function does THREE things:",
                "void ProcessUser(User user)",
                "{",
                "    ValidateEmail(user.Email);  // job 1",
                "    SaveToDatabase(user);       // job 2",
                "    SendWelcomeEmail(user);     // job 3",
                "}",
                "",
                "// GOOD — three functions, each with one clear responsibility",
                "// Now each is testable, reusable, and understandable alone.",
                "void ValidateUser(User user) { ... }",
                "void PersistUser(User user) { ... }",
                "void NotifyUser(User user) { ... }"
            );

            UI.Code("Comments — explain WHY, not WHAT",
                "// BAD comment — explains what the code clearly shows anyway:",
                "i++;  // increment i by 1",
                "",
                "// BAD comment — explains a poorly-named thing:",
                "// x is the user's age",
                "int x = user.Age;",
                "",
                "// GOOD comment — explains WHY this unusual decision was made:",
                "// Sleep 500ms to avoid Stripe's rate limit (100 req/sec).",
                "// Remove only after confirming rate limit is lifted.",
                "await Task.Delay(500);"
            );

            UI.Mistake(
                "Writing 'clever' code that squeezes many operations onto one line to look smart.",
                "Write boring, obvious code. Boring code is maintainable code. " +
                "Your teammates will thank you. Your future self will thank you."
            );
            UI.Pause();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section7_HabitsOfGreatEngineers()
        {
            UI.SubHeader("7. Daily Habits of Experienced Software Engineers");

            UI.Concept(
                "Becoming a great developer isn't one big insight. It's a series of small daily " +
                "habits compounded over years. Here are the habits that separate senior engineers " +
                "from developers who plateau."
            );

            var habits = new[]
            {
                ("Version control everything", "Commit small, commit often. Write meaningful commit messages. " +
                 "Git is your safety net. Use it."),

                ("Write tests alongside code", "Don't write all the code, then all the tests. Write a test, " +
                 "make it pass, refactor. (TDD: Test-Driven Development)"),

                ("Code review culture", "Get your code reviewed. Review others' code. Reviews catch bugs, " +
                 "spread knowledge, and build better code."),

                ("Read error messages fully", "Read the ENTIRE error. Stack trace. Inner exception. Line number. " +
                 "Most answers are IN the message."),

                ("Use documentation", "docs.microsoft.com is your friend. Reading official docs is faster than " +
                 "reading a Stack Overflow comment from 2009."),

                ("Refactor continuously", "Leave the code cleaner than you found it (Boy Scout Rule). " +
                 "Don't wait for a 'refactor sprint' that never comes."),

                ("Understand before optimizing", "Don't optimize code you don't understand. " +
                 "Don't optimize before you've measured. Premature optimization is the root of much evil."),

                ("Keep a learning log", "Write down what you learn each day. Teaching others (even in notes) " +
                 "solidifies your own understanding."),
            };

            foreach (var (title, desc) in habits)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  ◆ {title}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"    {desc}");
            }
            Console.ResetColor();

            UI.KeyPoint("Consistency beats intensity. 1 hour of focused learning every day beats 8 hours once a week.");
            UI.Pause();
        }

        // ─────────────────────────────────────────────────────────────
        private static void Section8_HowToKeepLearning()
        {
            UI.SubHeader("8. How to Keep Growing as a Developer");

            UI.Concept(
                "Technology changes fast. A developer who stops learning becomes obsolete. " +
                "The best engineers are not those who know the most right now — they're those " +
                "who know HOW to learn new things quickly."
            );

            UI.Diagram("The Learning Flywheel",
                "                   ┌──────────────────┐",
                "                   │  Learn a concept  │",
                "                   └────────┬──────────┘",
                "                            │",
                "                            ▼",
                "          ┌────────────────────────────────┐",
                "          │  Build something small with it  │",
                "          └───────────────┬────────────────┘",
                "                          │",
                "                          ▼",
                "            ┌─────────────────────────┐",
                "            │  Teach it or explain it  │  ← This is the key",
                "            └────────────┬────────────┘",
                "                         │",
                "                         ▼",
                "              ┌─────────────────────┐",
                "              │  Identify gaps and   │",
                "              │  go deeper or wider  │",
                "              └──────────┬──────────┘",
                "                         │",
                "                         └──▶ (repeat)"
            );

            UI.Print("RESOURCES THAT ACTUALLY MAKE YOU BETTER:\n");
            var resources = new[]
            {
                ("Write code daily",       "30 minutes of actual coding > 2 hours of watching tutorials."),
                ("Build real projects",    "A CRUD app with EF Core + SQL teaches more than 10 YouTube videos."),
                ("Read open-source code",  "GitHub. Pick a library you use. Read how it's implemented."),
                ("Do code challenges",     "LeetCode, HackerRank for interview prep. Focus on Easy/Medium."),
                ("Find a mentor",          "Someone 2-3 years ahead of you. Ask them what they wish they'd learned sooner."),
                ("Write about what you learn", "A dev blog, notes.txt, or even this app — writing forces clarity."),
            };

            foreach (var (r, d) in resources)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n  • {r,-30}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(d);
            }
            Console.ResetColor();

            UI.Exercise(
                "REFLECTION: What is one concept from your current job or project that you feel " +
                "uncertain about? Write it down. Then plan: will you read the docs, build a small " +
                "demo, or find a mentor who knows it well? Make a decision right now.",

                "There's no single correct answer — the value is in the act of identifying " +
                "your gap and committing to a plan. Most developers never identify their gaps " +
                "deliberately — they just wait until they're stuck. You're already ahead."
            );
        }

        // ─────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Developer Mindset", new QuizQuestion[]
            {
                new(
                    "A client says 'the app should be fast'. What should you do FIRST?",
                    new[] {
                        "Start optimizing the database queries immediately",
                        "Ask clarifying questions: fast under what load? What's the acceptable response time?",
                        "Add indexes to all database tables as a precaution",
                        "Rewrite the slowest functions in a lower-level language"
                    },
                    1,
                    "'Fast' is vague. Always push for specific, measurable requirements before acting."
                ),
                new(
                    "You've just been assigned a 6-month project. What's your FIRST step?",
                    new[] {
                        "Set up the project structure and start coding the core features",
                        "Write a detailed database schema first",
                        "Break it into smaller milestones, identify the riskiest parts, plan the first sprint",
                        "Ask the manager to reduce the scope"
                    },
                    2,
                    "Large projects must be decomposed. Start by identifying what you don't know yet — those are the risks."
                ),
                new(
                    "You write code that works but is hard to read. Your teammate must maintain it. What's the problem?",
                    new[] {
                        "Nothing — working code is good code",
                        "The teammate should learn to read better code",
                        "Unmaintainable code creates technical debt: it slows future work and hides bugs",
                        "It will only be a problem if the code is wrong"
                    },
                    2,
                    "Technical debt: every shortcut today becomes extra work tomorrow. Unmaintainable code is a liability."
                ),
                new(
                    "When should you add a comment to your code?",
                    new[] {
                        "After every line, to explain what the code does",
                        "Only when the code is complex enough that someone reading it in 6 months won't understand WHY this decision was made",
                        "Never — good code explains itself",
                        "At the start of every function to summarize what it does"
                    },
                    1,
                    "Comments explain WHY, not WHAT. Well-named code explains itself. Comments that restate the code are noise."
                ),
                new(
                    "You find a bug. What's the WORST debugging approach?",
                    new[] {
                        "Use the debugger to step through and watch variable values change",
                        "Explain the problem out loud to a colleague or rubber duck",
                        "Randomly change things until the error message disappears",
                        "Form a specific hypothesis about the cause and test it"
                    },
                    2,
                    "Random changes without understanding produce random results. Debugging is systematic hypothesis testing."
                ),
            });
        }
    }
}
