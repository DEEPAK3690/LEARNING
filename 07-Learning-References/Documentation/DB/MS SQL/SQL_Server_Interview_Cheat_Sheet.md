
# SQL Server Interview Cheat Sheet (Beginner)
_Last updated: Revision Pack_

---

## 1. Database Fundamentals

### What is a Database?
A database is a structured system to store, manage, and retrieve data efficiently.

### Table / Row / Column
- Table: Collection of data (like Excel sheet)
- Row: One record
- Column: One attribute

### Primary Key (PK)
- Uniquely identifies a row
- Cannot be NULL or duplicate
- Usually indexed (clustered by default)

### Foreign Key (FK)
- Creates relationship between tables
- Refers to PK of another table
- Maintains data integrity

### NULL vs NOT NULL
- NULL = no value / unknown
- NOT NULL = value required

---

## 2. SQL Basics (CRUD)

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

### UPDATE
```sql
UPDATE Users
SET Email = 'new@gmail.com'
WHERE UserId = 1;
```

### DELETE
```sql
DELETE FROM Users WHERE UserId = 1;
```

---

## 3. JOINs

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

### Interview Line
INNER JOIN → matching only  
LEFT JOIN → all left + matches

---

## 4. Aggregate Functions

| Function | Description |
|--------|-------------|
| COUNT | Number of rows |
| SUM | Total |
| AVG | Average |
| MIN | Minimum |
| MAX | Maximum |

```sql
SELECT COUNT(*) FROM Orders;
```

---

## 5. GROUP BY & HAVING

### GROUP BY
Groups rows before aggregation.
```sql
SELECT UserId, COUNT(*) 
FROM Orders
GROUP BY UserId;
```

### HAVING
Filters groups.
```sql
SELECT UserId, SUM(Amount)
FROM Orders
GROUP BY UserId
HAVING SUM(Amount) > 1000;
```

### WHERE vs HAVING
- WHERE → filters rows
- HAVING → filters groups

---

## 6. Subqueries

### IN
```sql
SELECT * FROM Users
WHERE UserId IN (SELECT UserId FROM Orders);
```

### EXISTS
```sql
SELECT * FROM Users U
WHERE EXISTS (
    SELECT 1 FROM Orders O WHERE O.UserId = U.UserId
);
```

### NOT EXISTS (Best Practice)
```sql
SELECT * FROM Users U
WHERE NOT EXISTS (
    SELECT 1 FROM Orders O WHERE O.UserId = U.UserId
);
```

---

## 7. Indexing (Basics)

### What is an Index?
An index helps SQL Server find data faster without scanning the whole table.

### Types
- Clustered Index: One per table (usually PK)
- Non-Clustered Index: Multiple allowed

### Example
```sql
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
```

### Interview Points
- Index improves SELECT, JOIN
- Index slows INSERT/UPDATE
- FK columns should be indexed

---

## 8. Microsoft SQL Server & T-SQL

### SQL vs SQL Server
- SQL: Query language
- SQL Server: Database system

### What is T-SQL?
Microsoft SQL Server extension of SQL.

### Variables
```sql
DECLARE @Id INT = 1;
SELECT * FROM Users WHERE UserId = @Id;
```

### IF / ELSE
```sql
IF EXISTS (SELECT 1 FROM Users WHERE UserId = 1)
    PRINT 'Exists';
ELSE
    PRINT 'Not Exists';
```

---

## 9. Stored Procedures

### What is a Stored Procedure?
Precompiled T-SQL code stored in DB.

### Why Use?
- Performance
- Reusability
- Security

### Example
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

## 10. Common Interview Q&A

### Q: Difference between WHERE and HAVING?
A: WHERE filters rows, HAVING filters groups.

### Q: How many clustered indexes per table?
A: One.

### Q: Does index improve INSERT?
A: No, it slows INSERT.

### Q: Stored Procedure vs Query?
A: SP is reusable, secure, precompiled.

---

## Final Revision Mantra

- JOIN connects tables
- GROUP BY groups rows
- HAVING filters aggregates
- EXISTS is safer than IN
- Index = faster reads, slower writes

---
End of Cheat Sheet
