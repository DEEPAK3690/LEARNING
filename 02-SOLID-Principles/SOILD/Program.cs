using Microsoft.VisualBasic;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SOILD
{
    class Program
    {
        static void Main(string[] args)
        {
            //    Single Responsibility Principle(SRP) – A class should have only one reason to change.
            //    Open/Closed Principle (OCP) – A class should be open for extension but closed for modification.
            //Liskov Substitution Principle(LSP) – Subtypes must be substitutable for their base types.
            //    Interface Segregation Principle(ISP) – Clients should not be forced to depend on interfaces they do not use.
            //    Dependency Inversion Principle (DIP) – Depend on abstractions, not on concretions.

            Order order = new Order { Items = new List<string> { "Item1", "Item2" }, TotalAmount = 100 };

            IPaymentProcessor paymentProcessor = new CreditCardPayment(); // Can be switched easily
            Notification notification = new EmailNotification(); // Can replace with SMSNotification

            Checkout checkout = new Checkout(paymentProcessor, notification);
            checkout.CompleteOrder(order);
        }
    }
}
