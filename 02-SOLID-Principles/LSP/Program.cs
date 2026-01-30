using System;
using static LSP.LSPwithout;

namespace LSP
{
    class Program
    {
        static void Main(string[] args)
        {
            //In C#, this means that if a class B inherits from a class A, then an object of type A can be replaced with an object of type B without breaking the program.

            //If a subclass changes the expected behavior of the base class in a way that breaks assumptions, it violates LSP.
            // A subclass should not change the behavior of the base class.
            //If a subclass cannot fully support the base class functionality, rethink inheritance!
            //Use abstract classes or interfaces to ensure correct behavior.

            //Avoid forcing methods onto subclasses if they don’t apply.



            //Birds.execute();

            Birdsfly.execute();

        }
    }
}
