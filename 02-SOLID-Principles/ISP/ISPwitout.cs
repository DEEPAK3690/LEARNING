using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP
{
    public interface IPrinter
    {
        void Print(string content);
        void Scan(string content);
        void Fax(string content);
    }

    class ISPwitout : IPrinter
    {
        public void Print(string content)
        {
            Console.WriteLine("Printing: " + content);
        }

        public void Scan(string content)
        {
            throw new NotImplementedException(); // Not needed but still required
        }

        public void Fax(string content)
        {
            throw new NotImplementedException(); // Not needed but still required
        }
    }
}
