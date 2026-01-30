# PATTERNS

## Understand the structure of a pattern

Every pattern is made up of **rows** and  **columns** .

Think like this:

* **Outer loop → rows**
* **Inner loop → columns (what to print in each row)**

## The Four-Step Universal Approach

**Step 1: Count the Number of Rows (Outer Loop)**

The first step is to identify how many **rows** (or lines) the pattern has. This determines what your outer loop will be. Simply count the horizontal lines in the pattern from top to bottom.

For example, if a pattern has 5 lines, your outer loop will run 5 times:

**Step 2: Focus on Columns and Connect Them to Rows (Inner Loop)**

For each row, determine how many **columns** (or elements) need to be printed. This is where you use nested loops. The key is to establish a relationship between the row number and the number of columns.

**Step 3: Determine What to Print**

Inside the inner loop, identify what needs to be printed at each position. This could be:

* A star (`*`)
* The row number (`i`)
* The column number (`j`)

**Step 4: Observe Symmetry (Optional)**

For complex patterns like diamonds, pyramids, or hourglass shapes, look for  **symmetry** . Many patterns can be broken down into two simpler patterns:

* Upper half and lower half
* Left side and right side

## Pyramid Pattern

```
    *
   ***
  *****
 *******
*********
```

Rows = 5

Columns = 9

[space] - [star] - [space]

[4, 1, 4] - 0
[3, 3, 3] - 1
[2, 5, 2] - 2
[1, 7, 1] - 3
[0, 9, 0] - 4

Space = (n - i - 1)

Star = (2 * i + 1) {i.e one more than twice} - The number multiplied by two, then increased by one

```
 for (int i = 0; i < n; i++)
 {
     int space = n - i - 1;
     int stars = 2 * i + 1;

     for (int j = 0; j < space; j++)
     {
         Console.Write(" ");
     }

     for (int j = 0; j < stars; j++)
     {
         Console.Write("*");
     }

     for (int j = 0; j < space; j++)
     {
         Console.Write(" ");
     }
     Console.WriteLine();
 }
```

## Inverted Pyramid

```
*********
 *******
  *****
   ***
    *
```

Rows = 5

Columns = 9

[space] - [star] - [space]

[0, 9, 0] - 0
[1, 7, 1] - 1
[2, 5, 2] - 2
[3, 3, 3] - 3
[4, 1, 4] - 4

Space = ( i )

Star = (2 * n ) - (2 * i + 1)

```
for (int i = 0; i < n; i++)
{
    int space = i;
    int stars = (2 * n) - (2 * i + 1);

    for (int j = 0; j < space; j++)
    {
        Console.Write(" ");
    }

    for (int j = 0; j < stars; j++)
    {
        Console.Write("*");
    }

    for (int j = 0; j < space; j++)
    {
        Console.Write(" ");
    }
    Console.WriteLine();
}
```

## Diamond

```
    *
   ***
  *****
 *******
*********
*********
 *******
  *****
   ***
    *
```

Here observe the Symmetry

Diamond = Upper Half Pyramid + Lower Half Inverted Pyramid

## Half Diamond

```
*
**
***
****
*****
****
***
**
*
```

Rows = 9

Column = 5

Here observe the Symmetry

Upper half Rows = 5

* Inner loop = No rows  equals No of columns = rows equals stars
* Upper half Column= (i) times

Lower half Rows = 4

* Lower half = Decrease in stars (2 * Upperhalf - row) = (2 * n - i)

```
int rows = 9;
int upperHalfRows = 5;
for (int i = 1; i <= rows; i++)
{
    int stars = i;

    if (i > upperHalfRows)
        stars = 2 * upperHalfRows - i;

    for (int j = 1; j <= stars; j++)
    {
        Console.Write("*");
    }
    Console.WriteLine();
}
```

## Binary Triangle

```
1
0 1
1 0 1
0 1 0 1
1 0 1 0 1
```

Rows = 5

Column = 5

Each row follows an **odd-even pattern** .

Each column alternates (flips) between `0` and `1`.

> Logic Explanation

Inner loop = No rows equals No of columns

* Column = (i) times
* Start = ( i % 2 == 0)
* Flip values = (1- start)

```
 for (int i = 1; i <= n; i++)
 {
     int start = 1;
     if (i % 2 == 0)
         start = 0;
     for (int j = 1; j <= i; j++)
     {
         Console.Write(start + " ");
         start = 1 - start;
     }
     Console.WriteLine();

 }
```

## Symmetrical_Numbers

```
1             1
1 2         2 1
1 2 3     3 2 1
1 2 3 4 4 3 2 1
```

Rows = 4

[number ] - [space ] - [number]

[1, 6, 1] - 1
[2, 4, 2] - 2
[3, 2, 3] - 3
[4, 0, 4] - 4

Number = ( i ) = rows

Space = 2 x (n - 1)

```
 for (int i = 1; i <= n; i++)
 {

     int num = i;
     for (int j = 1; j <= i; j++)
     {
         Console.Write(j + " ");
     }

     int space = 2 * (n - i);
     for (int j = 0; j < space; j++)
     {
         Console.Write("* ");
     }

     for (int j = i; j >= 1; j--)
     {
         Console.Write(j + " ");
     }

     Console.WriteLine();

 }
```

## Number Right Angled Triangle

```
1
2 3
4 5 6
7 8 9 10
11 12 13 14 15
```

Rows = 5

Column = 5

The numbers are incremented by 1

outer loop  = Rows

Inner loop = No rows equals No of columns

value = value + 1

```
int number = 1;
for (int i = 1; i <= n; i++)
{
    for (int j = 1; j <= i; j++)
    {
        Console.Write(number++ + " ");
    }
    Console.WriteLine();
}
```
