
# SQL Server & T-SQL – Complete Interview Recall Cheat Sheet (Beginner)
_Last updated: Consolidated revision pack_

> Purpose:
> - One file
> - Quick recall before interview
> - Covers concepts + doubts + why + examples

---

## 1. Database Fundamentals (Very Common Interview Area)

### What is a Database?
A database is a structured and persistent storage system that allows data to be stored, retrieved, updated, and deleted efficiently.

**Why not files?**
- No relationships
- No concurrency control
- No data integrity
- Poor performance

---

### Tables, Rows, Columns
- **Table** → structure to store data
- **Row** → one record
- **Column** → one attribute

```text
Table = Excel sheet
Row   = One line
Column= One field
```

---

### Primary Key (PK)
- Uniquely identifies a row
- Cannot be NULL
- Cannot be duplicate
- One PK per table
- Automatically indexed (usually clustered)

```sql
UserId INT PRIMARY KEY
```

**Interview Q**
- Can a table exist without PK? → Yes (not recommended)
- Can PK be composite? → Yes

---

### Foreign Key (FK)
- Creates relationship between tables
- Refers to PK of another table
- Prevents invalid data

```sql
FOREIGN KEY (UserId) REFERENCES Users(UserId)
```

**Interview Q**
- Should FK be indexed? → Yes (best practice)

---

### NULL vs NOT NULL
- NULL = unknown / no value
- NOT NULL = mandatory value
- NULL ≠ empty string

---

## 2. SQL Basics (CRUD – Must Know)

### SELECT
```sql
SELECT * FROM Users;
SELECT Name, Email FROM Users;
```

### WHERE
```sql
SELECT * FROM Users WHERE UserId = 1;
```

### INSERT
```sql
INSERT INTO Users (Name, Email)
VALUES ('Deepak', 'deepak@gmail.com');
```

### UPDATE (ALWAYS USE WHERE)
```sql
UPDATE Users
SET Email = 'new@gmail.com'
WHERE UserId = 1;
```

### DELETE (ALWAYS USE WHERE)
```sql
DELETE FROM Users WHERE UserId = 1;
```

**Interview warning**
- UPDATE/DELETE without WHERE → production disaster

---

## 3. JOINs (Very Important)

### INNER JOIN
Returns only matching rows.

```sql
SELECT U.Name, O.OrderId
FROM Users U
INNER JOIN Orders O ON U.UserId = O.UserId;
```

### LEFT JOIN
Returns all rows from left table.

```sql
SELECT U.Name, O.OrderId
FROM Users U
LEFT JOIN Orders O ON U.UserId = O.UserId;
```

**Interview one-liner**
- INNER JOIN → matching only
- LEFT JOIN → all left + matches

---

## 4. Aggregates, GROUP BY, HAVING

### Aggregate Functions
- COUNT
- SUM
- AVG
- MIN
- MAX

```sql
SELECT COUNT(*) FROM Orders;
```

---

### GROUP BY
Groups rows before aggregation.

```sql
SELECT UserId, COUNT(*)
FROM Orders
GROUP BY UserId;
```

---

### HAVING
Filters groups.

```sql
SELECT UserId, SUM(Amount)
FROM Orders
GROUP BY UserId
HAVING SUM(Amount) > 1000;
```

**WHERE vs HAVING**
- WHERE → row filter
- HAVING → group filter

---

## 5. Subqueries (Interview Favorite)

### IN
```sql
SELECT * FROM Users
WHERE UserId IN (SELECT UserId FROM Orders);
```

### NOT IN (Be careful with NULLs)
```sql
SELECT * FROM Users
WHERE UserId NOT IN (SELECT UserId FROM Orders);
```

---

### EXISTS (Preferred)
```sql
SELECT * FROM Users U
WHERE EXISTS (
    SELECT 1 FROM Orders O WHERE O.UserId = U.UserId
);
```

### NOT EXISTS (Best for "no data")
```sql
SELECT * FROM Users U
WHERE NOT EXISTS (
    SELECT 1 FROM Orders O WHERE O.UserId = U.UserId
);
```

---

## 6. Indexing (Basics – Interview Level)

### What is an Index?
A performance structure that allows SQL Server to locate rows quickly without scanning the entire table.

### Types
- **Clustered Index**
  - One per table
  - Usually PK
  - Defines physical order
- **Non-Clustered Index**
  - Multiple allowed
  - Separate structure

```sql
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
```

---

### When Index Helps
- WHERE
- JOIN
- ORDER BY
- GROUP BY

### When Index Hurts
- INSERT
- UPDATE
- DELETE

---

## 7. SQL Server & T-SQL

### SQL vs SQL Server
- SQL → language
- SQL Server → Microsoft RDBMS

### What is T-SQL?
SQL Server–specific extension of SQL.

---

### Variables
```sql
DECLARE @UserId INT = 1;
SELECT * FROM Users WHERE UserId = @UserId;
```

---

### IF / ELSE
```sql
IF EXISTS (SELECT 1 FROM Users WHERE UserId = 1)
    PRINT 'User exists';
ELSE
    PRINT 'User not exists';
```

---

## 8. Stored Procedures (Mandatory Topic)

### What is a Stored Procedure?
Precompiled block of T-SQL stored in the database.

### Why use Stored Procedures?
- Performance
- Reusability
- Security
- Centralized logic

---

### Stored Procedure Example
```sql
CREATE PROCEDURE GetUserById
    @UserId INT
AS
BEGIN
    SELECT * FROM Users WHERE UserId = @UserId;
END;
```

Execute:
```sql
EXEC GetUserById @UserId = 1;
```

---

### SP Interview Q&A
- Can SP accept parameters? → Yes
- Can SP return data? → Yes
- SP vs query? → SP is reusable and secure

---

## 9. Common Doubts You Asked (Consolidated)

### SELECT 1 vs SELECT *
- SELECT 1 → existence check (used with EXISTS)
- SELECT * → returns data

---

### Can table have multiple primary keys?
- ❌ No
- ✅ Can have composite PK

---

### Why JOIN instead of multiple queries?
- Better performance
- Cleaner logic
- Less app-side processing

---

### Why NOT EXISTS preferred over NOT IN?
- NOT IN fails with NULLs
- NOT EXISTS is safer and faster

---

## 10. Final Interview Recall Mantra

```
PK → identity
FK → relationship
JOIN → connect tables
GROUP BY → grouping
HAVING → filter groups
EXISTS → safest subquery
Index → fast read, slow write
SP → reusable DB logic
```

---
End of consolidated interview cheat sheet
