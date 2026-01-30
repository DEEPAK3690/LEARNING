using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP
{

    public abstract class Bird
    {
        public abstract void Eat();
    }

    public class FlyingBird : Bird
    {
        public void Fly() => Console.WriteLine("Flying bird is flying");
        public override void Eat() => Console.WriteLine("Bird is eating");
    }

    public class Sparrow : FlyingBird
    {
        public override void Eat() => Console.WriteLine("Sparrow is eating seeds.");
    }

    public class Penguin : Bird
    {
        public override void Eat() => Console.WriteLine("Penguin is eating fish.");
        public void Swim() => Console.WriteLine("Penguin is swimming.");
    }

    public class Birdsfly
    {
        public static void execute()
        {
            FlyingBird sparrow = new Sparrow();
            sparrow.Fly();
            sparrow.Eat();

            Bird penguin = new Penguin();
            penguin.Eat();
            ((Penguin)penguin).Swim();
        }
    }

}
