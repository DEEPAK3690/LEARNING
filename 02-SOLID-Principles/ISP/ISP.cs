using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP
{
    public interface IPrint
    {
        void Print(string content);
    }

    public interface IScan
    {
        void Scan(string content);
    }

    public interface IFax
    {
        void Fax(string content);
    }


    public class BasicPrinter : IPrint
    {
        public void Print(string content)
        {
            Console.WriteLine("Printing: " + content);
        }
    }

    public class AdvancedPrinter : IPrint, IScan, IFax
    {
        public void Print(string content)
        {
            Console.WriteLine("Printing: " + content);
        }

        public void Scan(string content)
        {
            Console.WriteLine("Scanning: " + content);
        }

        public void Fax(string content)
        {
            Console.WriteLine("Faxing: " + content);
        }
    }

}
