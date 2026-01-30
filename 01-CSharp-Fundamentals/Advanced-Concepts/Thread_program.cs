using System;
using System.Threading;

namespace C_Advanced
{

    //What: A basic unit of CPU execution.
    //Why: Used when you need full control over thread lifecycle.
    //When: Rare in modern apps; use only for long-running or low-level control scenarios.

    internal class Thread_program
    {
        public static void run()
        {
            Thread thread = new Thread(() => { Thread_program.HeavyCalculation(); });
            thread.Start();


            Thread thread1 = new Thread(() => {
                int i = 0;
                while(true)
                {
                    if(i == 100000)
                    {
                        break;
                    }
                    Console.WriteLine("Hello"+ i);
                    i++;
                }
              });

            thread1.Start();

            
        }



        public static void HeavyCalculation()
        {
            int seed = 8;
            double sum = 0;
            for (int i = 0; i < 20_000_000; i++)
            {
                sum += Math.Sqrt(i + seed);
                Console.WriteLine(sum);
            }
            if (sum < 0) Console.WriteLine("Impossible");
        }
    }
}
