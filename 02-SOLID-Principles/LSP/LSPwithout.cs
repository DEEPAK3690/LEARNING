using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP
{
    class LSPwithout
    {

        public class Bird
        {
            public virtual void Fly()
            {
                Console.WriteLine("Bird is flying");
            }
        }

        public class Sparrow : Bird
        {
            public override void Fly()
            {
                Console.WriteLine("Sparrow is flying high!");
            }
        }

        public class Penguin : Bird
        {
            public override void Fly()
            {
                throw new NotImplementedException("Penguins cannot fly!");
            }
        }

        public class Birds
        {
            public static void execute()
            {
                Bird sparrow = new Sparrow();
                sparrow.Fly();  

                Bird penguin = new Penguin();
                penguin.Fly();  // ❌ Crashes! Penguins cannot fly.
            }
        }





    }
}
