using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Advanced
{

    //Task: represents a void-returning async operation.

    //Task<T>: returns a result(T) from an async operation.

    //ValueTask<T>: optimized version to avoid allocations when the result is already available.
    internal class TASKProgram
    {
        public async Task run()
        {
            int result = await ComputeAsync(5);
            Console.WriteLine($"Result: {result}");
        }

        static async Task<int> ComputeAsync(int x)
        {
            await Task.Delay(1000); // Simulate work
            return x * x;
        }
    }
}
