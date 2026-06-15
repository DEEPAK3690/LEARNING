using System;
using System.Security.Cryptography.X509Certificates;

namespace DSA.Patterns
{
    public static class Logical
    {
        /// <summary>
        /// Finds two numbers in an array that add up to a target sum.
        /// </summary>
        public static void TwoSum(int[] nums, int target)
        {
            //for (int i = 0; i < nums.Length; i++)
            //{
            //    for (int j = i + 1; j < nums.Length; j++)
            //    {
            //        if (nums[i] + nums[j] == target)
            //        {
            //            Console.WriteLine($"Pair found: {nums[i]} + {nums[j]} = {target}");
            //        }
            //    }
            //    Console.WriteLine("No pair found");
            //}

            // Use a hash set to store numbers we've seen
            var seen = new HashSet<int>();

            foreach (int num in nums)
            {
                int complement = target - num;

                if (seen.Contains(complement))
                {
                    Console.WriteLine($"Pair found: {complement} + {num} = {target}");
                    return;
                }

                seen.Add(num);
            }

            Dictionary<int, int> numToIndex = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                int complement = target - nums[i];

                if (numToIndex.ContainsKey(complement))
                {
                    //Console.WriteLine($"Pair found: {complement} + {nums[i]} = {target}");
                    return;
                }

                numToIndex[i] = 1;
            }
        }


        public static int leadingZeroes(int n)
        {
            int count = 0;

            for (int i = 31; i >= 0; i--)
            {
                if (((n >> i) & 1) == 0)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine($"count:{count}");
            return count;
        }

        public static void PrintBinary(int n)
        {
            string binary = Convert.ToString(n, 2);
            Console.WriteLine($"Binary representation of {n} is: {binary}");
        }

        public static int[] matchchecker()
        {
            int p = 5;

            int k = 2;

            int[] n = { 9, 5, 3, 6, 2 };

            int[] n1 = {-1, -1, -1, -1, -1};

            for (int i = 0; i < n.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (n[i] <= n[j] + k && n[i] >= n[j] - k)
                    {
                        if (n1[j] != 0)
                        {
                            continue;
                        }
                        n1[i] = j+1;
                        n1[j] = i+1;
                        break;
                    }
                }
            }

            return n1;
        }
    }
}