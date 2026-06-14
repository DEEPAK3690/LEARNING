# C# Coding Interview Cheat Sheet

---

## Dictionary `Dictionary<K, V>`

```
// Declare & Initialize
var dict = new Dictionary<string, int>();
var dict = new Dictionary<string, int> { {"a", 1}, {"b", 2} };

// Add / Update
dict["key"] = 10;
dict.Add("key", 10);           // throws if key exists

// Access
int val = dict["key"];         // throws if missing
dict.TryGetValue("key", out int val);  // safe

// Check
dict.ContainsKey("key");
dict.ContainsValue(10);

// Remove
dict.Remove("key");

// Iterate
foreach (var kvp in dict)      { kvp.Key; kvp.Value; }
foreach (var key in dict.Keys) { }
foreach (var val in dict.Values) { }

// Useful
dict.Count;
dict.GetValueOrDefault("key", 0);  // returns default if missing
```

---

## HashSet `HashSet<T>`

```
// Declare & Initialize
var set = new HashSet<int>();
var set = new HashSet<int> { 1, 2, 3 };

// Add / Remove
set.Add(5);
set.Remove(5);

// Check
set.Contains(5);

// Set operations
set.UnionWith(other);          // set = set ∪ other
set.IntersectWith(other);      // set = set ∩ other
set.ExceptWith(other);         // set = set - other
set.IsSubsetOf(other);
set.IsSupersetOf(other);

// Iterate
foreach (var item in set) { }

set.Count;
```

---

## List `List<T>`

```
// Declare & Initialize
var list = new List<int>();
var list = new List<int> { 1, 2, 3 };
var list = new List<int>(new int[] { 1, 2, 3 });

// Add
list.Add(4);
list.AddRange(new[] { 5, 6 });
list.Insert(0, 99);            // insert at index

// Access
int val = list[0];
list[0] = 10;

// Remove
list.Remove(4);                // removes first occurrence by value
list.RemoveAt(0);              // removes by index
list.RemoveRange(0, 2);        // removes count elements from index

// Search
list.Contains(3);
list.IndexOf(3);
list.LastIndexOf(3);
list.Find(x => x > 2);
list.FindIndex(x => x > 2);

// Sort
list.Sort();
list.Sort((a, b) => b.CompareTo(a));  // descending
list.Reverse();

// Useful
list.Count;
list.ToArray();
list.Min(); list.Max(); list.Sum();  // LINQ
```

---

## Stack `Stack<T>` _(LIFO)_

```
// Declare & Initialize
var stack = new Stack<int>();
var stack = new Stack<int>(new[] { 1, 2, 3 });  // 3 is on top

// Push / Pop / Peek
stack.Push(5);
int top = stack.Pop();         // removes + returns
int top = stack.Peek();        // returns without removing

// Check
stack.Contains(5);
stack.Count;

// Iterate (top to bottom)
foreach (var item in stack) { }
```

---

## Queue `Queue<T>` _(FIFO)_

```
// Declare & Initialize
var queue = new Queue<int>();

// Enqueue / Dequeue / Peek
queue.Enqueue(5);
int front = queue.Dequeue();   // removes + returns
int front = queue.Peek();      // returns without removing

// Check
queue.Contains(5);
queue.Count;

// Iterate (front to back)
foreach (var item in queue) { }
```

---

## PriorityQueue `PriorityQueue<T, TPriority>` _(Min-Heap, .NET 6+)_

```
// Declare
var pq = new PriorityQueue<string, int>();

// Enqueue with priority (lower = higher priority)
pq.Enqueue("task", 1);
pq.Enqueue("urgent", 0);

// Dequeue (lowest priority number first)
string item = pq.Dequeue();
pq.TryDequeue(out string item, out int priority);
pq.Peek();

pq.Count;

// Max-Heap trick: negate the priority on enqueue
pq.Enqueue("item", -score);
```

---

## LinkedList `LinkedList<T>`

```
// Declare & Initialize
var ll = new LinkedList<int>();
var ll = new LinkedList<int>(new[] { 1, 2, 3 });

// Add
ll.AddFirst(0);
ll.AddLast(4);
ll.AddAfter(ll.Find(2), 99);
ll.AddBefore(ll.Find(2), 99);

// Access
ll.First.Value;
ll.Last.Value;
LinkedListNode<int> node = ll.Find(2);  // O(n)
node.Next; node.Previous;

// Remove
ll.RemoveFirst();
ll.RemoveLast();
ll.Remove(node);               // O(1) if you have the node

ll.Count;
```

---

## Array

```
// Declare & Initialize
int[] arr = new int[5];
int[] arr = { 1, 2, 3, 4, 5 };
int[,] matrix = new int[3, 4];             // 2D
int[][] jagged = new int[3][];             // jagged

// Access
int val = arr[0];
int val = matrix[row, col];

// Useful
arr.Length;
matrix.GetLength(0);           // row count
matrix.GetLength(1);           // col count

Array.Sort(arr);
Array.Reverse(arr);
Array.Fill(arr, 0);
Array.Copy(src, dest, length);
Array.IndexOf(arr, 3);

// Span for slicing (no copy)
arr.AsSpan(1, 3);
```

---

## String

```
// Common operations
str.Length;
str[i];                        // char at index
str.ToCharArray();
new string(charArray);

str.Contains("sub");
str.StartsWith("ab"); str.EndsWith("ab");
str.IndexOf('a');
str.LastIndexOf('a');
str.Substring(start, length);

str.Replace("old", "new");
str.ToUpper(); str.ToLower();
str.Trim(); str.TrimStart(); str.TrimEnd();
str.Split(',');
string.Join(",", arr);

str.PadLeft(5, '0');           // "00042"
str.PadRight(5);

// StringBuilder for mutation
var sb = new StringBuilder();
sb.Append("hello");
sb.Insert(0, "x");
sb.Remove(0, 1);
sb.ToString();
sb.Length;
sb[i];                         // access char
sb[i] = 'x';                   // set char
```

---

## Sorting & Comparison

```
// Sort array of objects
Array.Sort(arr, (a, b) => a.CompareTo(b));       // ascending
Array.Sort(arr, (a, b) => b.CompareTo(a));       // descending

// Sort list
list.Sort((a, b) => a.x != b.x ? a.x - b.x : a.y - b.y);  // multi-key

// LINQ sort (returns new IEnumerable, not in-place)
var sorted = list.OrderBy(x => x.val).ThenByDescending(x => x.id);
```

---

## LINQ Essentials

```
// Filter / Map / Reduce
list.Where(x => x > 2);
list.Select(x => x * 2);
list.Aggregate(0, (acc, x) => acc + x);

// Quantifiers
list.Any(x => x > 5);
list.All(x => x > 0);

// Aggregates
list.Count(x => x > 0);
list.Sum(x => x);
list.Min(); list.Max();
list.Average();

// Grouping
list.GroupBy(x => x.key);      // returns IGrouping<K,V>
foreach (var g in groups) { g.Key; g.ToList(); }

// Distinct / flat
list.Distinct();
list.SelectMany(x => x.inner);

// First / Single
list.First(x => x > 0);        // throws if none
list.FirstOrDefault(x => x > 0);
list.SingleOrDefault(x => x == 5);  // throws if >1

// Convert
list.ToList(); list.ToArray(); list.ToHashSet();
list.ToDictionary(x => x.id, x => x.name);
```

---

## Useful Math & Tricks

```
Math.Abs(x);
Math.Max(a, b); Math.Min(a, b);
Math.Pow(2, 10);               // double
Math.Sqrt(x);
Math.Floor(x); Math.Ceiling(x); Math.Round(x);
Math.Log(x); Math.Log(x, 2);   // log base 2

int.MaxValue;  int.MinValue;
long.MaxValue; long.MinValue;

// Bit tricks
x & 1;                         // is odd?
x >> 1;                        // divide by 2
x << 1;                        // multiply by 2
x & (x - 1);                   // clear lowest set bit
x | (1 << k);                  // set bit k
x & ~(1 << k);                 // clear bit k
(x >> k) & 1;                  // check bit k

// Modulo (for circular / overflow)
(i + 1) % n;
((i % mod) + mod) % mod;       // avoid negative mod
```

---

## Common Patterns

```
// Swap
(a, b) = (b, a);

// Null coalescing
var val = dict.GetValueOrDefault(key, defaultVal);

// Ternary
int result = condition ? a : b;

// Type checking
if (obj is string s) { /* use s */ }

// Infinity substitute
int INF = int.MaxValue / 2;   // safe for INF + INF without overflow
```

---

## HackerEarth / Competitive Programming — Input Patterns

### Template (fast I/O)

```
using System;
using System.IO;

class Solution {
    static StreamReader sr = new StreamReader(Console.OpenStandardInput());
    static string ReadLine() => sr.ReadLine();
    static string[] ReadTokens() => ReadLine().Trim().Split(' ');
    static int ReadInt() => int.Parse(ReadLine().Trim());

    static void Main() {
        // your code here
    }
}
```

---

### Single value

```
5
```

```
int n = int.Parse(Console.ReadLine());
```

---

### Multiple values on one line

```
3 7 2
```

```
var parts = Console.ReadLine().Split(' ');
int a = int.Parse(parts[0]);
int b = int.Parse(parts[1]);
int c = int.Parse(parts[2]);

// or inline
int[] vals = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);
```

---

### Array on one line _(most common)_

```
5
1 3 5 7 9
```

```
int n = int.Parse(Console.ReadLine());
int[] arr = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);
```

---

### Array each element on its own line

```
5
1
3
5
7
9
```

```
int n = int.Parse(Console.ReadLine());
int[] arr = new int[n];
for (int i = 0; i < n; i++)
    arr[i] = int.Parse(Console.ReadLine());
```

---

### Multiple test cases

```
3          ← T (number of test cases)
5
1 3 5 7 9
4
2 4 6 8
2
10 20
```

```
int t = int.Parse(Console.ReadLine());
while (t-- > 0) {
    int n = int.Parse(Console.ReadLine());
    int[] arr = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);
    // solve
}
```

---

### 2D Matrix

```
3 4          ← rows cols
1 2 3 4
5 6 7 8
9 0 1 2
```

```
var rc = Console.ReadLine().Split(' ');
int rows = int.Parse(rc[0]), cols = int.Parse(rc[1]);
int[,] grid = new int[rows, cols];
for (int r = 0; r < rows; r++) {
    var row = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);
    for (int c = 0; c < cols; c++) grid[r, c] = row[c];
}
```

---

### Graph: edges list

```
4 5          ← N nodes, M edges
1 2
1 3
2 4
3 4
2 3
```

```
var nm = Console.ReadLine().Split(' ');
int n = int.Parse(nm[0]), m = int.Parse(nm[1]);

var adj = new List<int>[n + 1];
for (int i = 0; i <= n; i++) adj[i] = new List<int>();

for (int i = 0; i < m; i++) {
    var e = Console.ReadLine().Split(' ');
    int u = int.Parse(e[0]), v = int.Parse(e[1]);
    adj[u].Add(v);
    adj[v].Add(u);   // remove for directed
}
```

---

### Read until EOF

```
string line;
while ((line = Console.ReadLine()) != null) {
    // process line
}
```

---

### Output

```
Console.WriteLine(answer);                     // single value
Console.WriteLine(string.Join(" ", arr));      // space-separated
Console.WriteLine(string.Join("\n", list));    // one per line

// Fast bulk output
var sb = new StringBuilder();
foreach (var x in results) sb.AppendLine(x.ToString());
Console.Write(sb);
```

---

### Quick helpers

```
// Parse pair (a, b) from a line
(int a, int b) ReadPair() {
    var p = Console.ReadLine().Split(' ');
    return (int.Parse(p[0]), int.Parse(p[1]));
}

// Parse long array (when values exceed int range)
long[] arr = Array.ConvertAll(Console.ReadLine().Split(' '), long.Parse);

// Trim safety (avoids issues with trailing spaces/\r\n on Windows)
Console.ReadLine().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
```