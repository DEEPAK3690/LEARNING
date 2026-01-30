using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCP
{
    public interface IPayment
    {
        void ProcessPayment();
    }

    public class CreditCardPayment : IPayment
    {
        public void ProcessPayment()
        {
            Console.WriteLine("Processing Credit Card Payment");
        }
    }

    public class PayPalPayment : IPayment
    {
        public void ProcessPayment()
        {
            Console.WriteLine("Processing PayPal Payment");
        }
    }

    // New payment methods can be added without modifying existing code
    public class BitcoinPayment : IPayment
    {
        public void ProcessPayment()
        {
            Console.WriteLine("Processing Bitcoin Payment");
        }
    }

    public class PaymentProcessor
    {
        private readonly IPayment _payment;

        public PaymentProcessor(IPayment payment)
        {
            _payment = payment;
        }

        public void Process()
        {
            _payment.ProcessPayment();
        }
    }

}
