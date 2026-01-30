using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace C_Advanced
{

    //A CancellationToken in C# is a way to gracefully stop an ongoing task or operation—like stopping a download, a loop, or a long-running method—when a cancellation is requested.

    //Instead of forcefully aborting a thread(which is dangerous), the CancellationToken lets you cooperatively cancel a task by checking the token during execution.
    internal class Cancellation
    {
        public async Task Run()
        {
            Console.WriteLine("Starting work without cancellation...");
            await DoWorkWithoutCancellation();
            Console.WriteLine("Work completed (no way to cancel).");
        }

        public static async Task DoWorkWithoutCancellation()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Working... {i + 1}");
                await Task.Delay(1000); // Simulates 1 second of work
            }
        }

        public async Task Runcancel()
        {
            Console.WriteLine("Starting work with cancellation...");

            using CancellationTokenSource cts = new CancellationTokenSource();

            // Simulate external cancellation request after 3.5 seconds
            Task.Run(() =>
            {
                //Thread.Sleep(3500);
                Console.ReadKey();
                cts.Cancel();
                Console.WriteLine("Cancellation requested!");
            });

            try
            {
                await DoWorkWithCancellation(cts.Token);
                Console.WriteLine("Work completed successfully.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Work was cancelled.");
            }
        }

        public static async Task DoWorkWithCancellation(CancellationToken token)
        {
            for (int i = 0; i < 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation requested. Stopping work.");
                    return; // or throw new OperationCanceledException();
                }
                token.ThrowIfCancellationRequested(); // Or check with token.IsCancellationRequested
                Console.WriteLine($"Working... {i + 1}");
                await Task.Delay(1000); // Supports cancellation
            }
        }
    }
}
