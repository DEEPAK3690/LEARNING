using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCP
{
    public class PaymentProcessorwithout
    {
        public void ProcessPayment(string paymentType)
        {
            if (paymentType == "CreditCard")
            {
                Console.WriteLine("Processing Credit Card Payment");
            }
            else if (paymentType == "PayPal")
            {
                Console.WriteLine("Processing PayPal Payment");
            }
        }
    }

}
