using System;
namespace OCP
{
    class Program
    {
        static void Main(string[] args)
        {
            //"Software entities (such as classes, modules, functions, etc.) should be open for extension but closed for modification."
            //Why Do We Need OCP?
            //Reduces the risk of breaking existing functionality when adding new features.
            //Improves maintainability and scalability.
            //Supports single responsibility by preventing modification of existing code.
            //Enhances unit testing by allowing dependency injection and mock testing.
            IPayment payment = new PayPalPayment();
            PaymentProcessor paymentProcessor = new PaymentProcessor(payment);
            paymentProcessor.Process();
        }
    }
}
