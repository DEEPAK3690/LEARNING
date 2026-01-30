using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace C_Advanced
{
    //async marks a method that runs asynchronously.
    //await is used to "wait" for an asynchronous operation to complete without blocking the main thread.
    internal class Async_Await
    {
        public async void run()
        {
            Console.WriteLine("Start");
            //Task.Delay(5000).Wait();// Blocks the thread that is currently executing this method
            await Task.Delay(1000); //No thread blocking — async wait
            await DownloadContentAsync(); ; // This line waits for the async method to complete
            //DownloadContentAsync();// This line starts the task but doesn't wait for it
            //Console.WriteLine(result.Substring(0, 50));
            Console.WriteLine("End");
        }

        static async Task<string> DownloadContentAsync()
        {
            using HttpClient client = new();
         
            return await client.GetStringAsync("https://example.com");
        }

        public void runSync()
        {
            Console.WriteLine("Start");
            string result = DownloadContent();
            Console.WriteLine(result.Substring(0, 50));
            Console.WriteLine("End");
        }

        static string DownloadContent()
        {
            using HttpClient client = new();
            Task<string> task = client.GetStringAsync("https://example.com");
            return task.Result; // blocks thread
        }

    }
}
