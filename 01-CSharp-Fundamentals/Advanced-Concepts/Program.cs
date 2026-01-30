using System;
using System.Threading.Tasks;

namespace C_Advanced
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Topics topics = Topics.Thread;

            switch (topics)
            {
                case Topics.Async_Await:
                    Console.WriteLine("Async_Await");
                    Async_Await async_Await = new Async_Await();
                    async_Await.run();
                    //async_Await.runSync();
                    Console.WriteLine("Async_Await end");
                    break;
                case Topics.Task:
                    Console.WriteLine("Task");
                    TASKProgram task = new TASKProgram();
                    await task.run();
                    Console.WriteLine("Task end");
                    break;
                case Topics.cancellation_token:
                    Console.WriteLine("cancellation_token");
                    Cancellation cancellation = new Cancellation();
                    //await cancellation.Run();
                    await cancellation.Runcancel();
                    break; 
                case Topics.Concurrency:
                    Console.WriteLine("Concurrency");
                    Concurrency_Program.RUN();
                    break;
                case Topics.parallelism:
                    Console.WriteLine("parallelism");
                    parallelism_program.Run();
                    break;
                case Topics.Thread:
                    Console.WriteLine("Thread");
                    Thread_program.run();
                    break;
            }

        }
    }
}
