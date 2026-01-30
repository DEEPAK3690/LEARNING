using System;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;

namespace DIP
{
    class Program
    {
        static void Main(string[] args)
        {
            //The Dependency Inversion Principle (DIP) states that:
            //High - level modules should not depend on low-level modules.Both should depend on abstractions.
            //Abstractions should not depend on details. Details should depend on abstractions.
            //In simple terms, instead of directly depending on concrete implementations, depend on abstractions(interfaces or abstract classes). This makes the system flexible, testable, and maintainable.

            // We can switch between different implementations easily
            IDataRepository repository = new DatabaseRepository(); // Or new FileRepository();
            //IDataRepository repository = new  FileRepository(); // Or new FileRepository();
            DataProcessor processor = new DataProcessor(repository);

            processor.ProcessData();




        }
    }
}
