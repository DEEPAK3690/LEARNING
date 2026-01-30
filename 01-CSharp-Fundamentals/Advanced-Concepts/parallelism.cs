using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Advanced
{

    //Multiple tasks run at the same time, truly simultaneous
    //Thread Use	May use 1 thread for many tasks(concurrency)	   Uses multiple threads on multiple cores
    internal class parallelism_program
    {
        public static void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, 18, i =>
            {
                double result = HeavyCalculation(i);
                Console.WriteLine($"Result {i}: {result}, Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId} Core: {Thread.GetCurrentProcessorId()}");
            });
            stopwatch.Stop();
            Console.WriteLine($"All tasks done in {stopwatch.ElapsedMilliseconds} ms");
        }
        static double HeavyCalculation(int value)
        {
            double sum = 0;
            for (int i = 0; i < 20_000_000; i++)
                sum += Math.Sqrt(value + i);
            return sum;
        }
    }
}
