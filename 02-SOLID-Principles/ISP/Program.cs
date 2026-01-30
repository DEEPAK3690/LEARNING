using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ISP
{
    class Program
    {
        static void Main(string[] args)
        {
            //Clients should not be forced to depend on interfaces they do not use."
            //Creating a large interface with many unrelated methods, we should create smaller, more specific interfaces that a class can implement based on its needs.

            BasicPrinter basicPrinter = new BasicPrinter();
            basicPrinter.Print("Hello World");

            AdvancedPrinter advancedPrinter = new AdvancedPrinter();
            advancedPrinter.Print("Hello World");
            advancedPrinter.Scan("Hello World_scan");
            advancedPrinter.Fax("Hello World");
        }
    }
}
