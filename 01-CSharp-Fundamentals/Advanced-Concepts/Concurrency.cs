using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace C_Advanced
{

    //Concurrency is about dealing with multiple tasks at the same time — but not necessarily running simultaneously. It allows switching between tasks, giving the appearance of doing things simultaneously, even on a single-core machine.
    // If a thread is free(and cores are available), concurrency can behave like parallelism.

    internal class Concurrency_Program
    {
        public static void RUN()
        {

            int count = 18;
            Stopwatch sw = Stopwatch.StartNew();

            Task[] tasks = new Task[count];

            for (int i = 0; i < count; i++)
            {
                int local = i;
                tasks[i] = Task.Run(() =>
                {
                    Console.WriteLine($"Task {local} started on Thread {Thread.CurrentThread.ManagedThreadId}, Core: {Thread.GetCurrentProcessorId()}");
                    HeavyCalculation(local);
                    Console.WriteLine($"Task {local} finished");
                });
            }

            Task.WaitAll(tasks);
            sw.Stop();
            //Task.Run(() => HeavyCalculation(4));

            Console.WriteLine($"All tasks done in {sw.ElapsedMilliseconds} ms");
        }

        static void HeavyCalculation(int seed)
        {
            double sum = 0;
            for (int i = 0; i < 20_000_000; i++)
                sum += Math.Sqrt(i + seed);
            if (sum < 0) Console.WriteLine("Impossible");
        }

    }
}
