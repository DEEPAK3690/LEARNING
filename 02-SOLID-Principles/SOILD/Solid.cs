using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOILD
{

    // SRP: Separate responsibilities
    class Order // Handles Order Data
    {
        public List<string> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }

    class OrderProcessor // Handles Business Logic
    {
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"Processing order with {order.Items.Count} items. Total: {order.TotalAmount}");
        }
    }

    // OCP: Payment methods are open for extension, closed for modification
    interface IPaymentProcessor
    {
        void Pay(decimal amount);
    }

    class CreditCardPayment : IPaymentProcessor
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Paid {amount} using Credit Card.");
        }
    }

    class PayPalPayment : IPaymentProcessor
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Paid {amount} using PayPal.");
        }
    }

    // LSP: Subtypes can replace base type without breaking functionality
    abstract class Notification
    {
        public abstract void Send(string message);
    }

    class EmailNotification : Notification
    {
        public override void Send(string message)
        {
            Console.WriteLine($"Email Sent: {message}");
        }
    }

    class SMSNotification : Notification
    {
        public override void Send(string message)
        {
            Console.WriteLine($"SMS Sent: {message}");
        }
    }

    // ISP: Split large interfaces into smaller ones
    interface IPrintable
    {
        void Print();
    }

    interface IEmailable
    {
        void SendEmail();
    }

    class Invoice : IPrintable, IEmailable
    {
        private readonly Order _order;

        public Invoice(Order order)
        {
            _order = order;
        }

        public void Print()
        {
            Console.WriteLine("------ Invoice ------");
            Console.WriteLine("Items Purchased:");
            foreach (var item in _order.Items)
            {
                Console.WriteLine($"- {item}");
            }
            Console.WriteLine($"Total Amount: {_order.TotalAmount}");
            Console.WriteLine("---------------------");
        }

        public void SendEmail()
        {
            Console.WriteLine("Invoice sent via email.");
        }
    }

    // DIP: Depend on abstractions (interfaces) instead of concrete implementations
    class Checkout
    {
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly Notification _notification;

        public Checkout(IPaymentProcessor paymentProcessor, Notification notification)
        {
            _paymentProcessor = paymentProcessor;
            _notification = notification;
        }

        public void CompleteOrder(Order order)
        {
            _paymentProcessor.Pay(order.TotalAmount);
            _notification.Send("Order completed successfully.");
            Invoice invoice = new Invoice(order);
            invoice.Print();
            invoice.SendEmail();
        }
    }

}
