
# SQL Server Learning Notes (With Real-Time Scenarios)

---

## 1. Introduction to SQL

### What is SQL?

* **SQL (Structured Query Language)** is used to access and manipulate databases.
* It is the standard language for relational database systems like  **SQL Server** ,  **MySQL** ,  **PostgreSQL** , etc.

### What is RDBMS?

* **RDBMS (Relational Database Management System)** organizes data into tables with rows and columns.
* It enforces relationships between tables using **primary** and  **foreign keys** .

### RDBMS vs NoSQL

| Feature   | RDBMS (SQL)                               | NoSQL                                  |
| --------- | ----------------------------------------- | -------------------------------------- |
| Structure | Fixed schema (tables, rows, columns)      | Flexible (documents, key-value, graph) |
| Examples  | SQL Server, MySQL                         | MongoDB, Cassandra                     |
| Best for  | Structured data and complex relationships | Unstructured or fast-changing data     |

💡 **Real-time Example:**

In a  **banking system** , customer, account, and transaction data are interrelated and well-structured — ideal for  **SQL Server** . But a **social media app** dealing with posts, likes, and comments may prefer **NoSQL** due to unstructured and variable data.

---

## 2. SQL Basics

### Tables, Rows, and Columns

* **Table:** A collection of related data entries.
* **Row (Record):** Represents one data entry.
* **Column (Field):** Represents a specific attribute for all records.

### SELECT Statement

```sql
SELECT * FROM Customers;
```

Retrieves all data from the `Customers` table.

💡 **When to use:** Useful for quick data checks during development or debugging.

### SELECT DISTINCT

```sql
SELECT DISTINCT Country FROM Customers;
```

Returns unique values from a column.

💡 **Scenario:** When creating a **filter dropdown list** (e.g., list of unique countries in an application UI).

### WHERE Clause

```sql
SELECT * FROM Customers WHERE Country = 'Mexico';
```

Filters records based on conditions.

💡 **Scenario:** In an e-commerce system, filter orders by customer region.

### ORDER BY

```sql
SELECT * FROM Products ORDER BY Price DESC;
```

Sorts records in ascending or descending order.

💡 **Scenario:** Displaying a “Top 10 Most Expensive Products” list.

### LIKE Operator

```sql
SELECT * FROM Customers WHERE CustomerName LIKE 'R%';
```

Finds records matching a pattern.

💡 **Scenario:** Searching for customers whose name starts with a letter entered by the user.

---

## 3. Data Manipulation Statements

### INSERT INTO

```sql
INSERT INTO Customers (CustomerName, City, Country)
VALUES ('Cardinal', 'Stavanger', 'Norway');
```

Adds a new record.

💡 **Scenario:** Inserting a new user registration record.

### UPDATE

```sql
UPDATE Customers SET City = 'Frankfurt' WHERE CustomerID = 1;
```

Updates existing records.

💡 **Scenario:** Updating a customer’s address in a CRM system.

### DELETE

```sql
DELETE FROM Customers WHERE CustomerID = 5;
```

Deletes records based on conditions.

💡 **Scenario:** Removing inactive or duplicate accounts.

### DROP TABLE

```sql
DROP TABLE TempData;
```

Removes a table structure permanently.

💡 **Scenario:** Used during database clean-up or migration.

---

## 4. Aggregate Functions

| Function    | Description         | Example                                |
| ----------- | ------------------- | -------------------------------------- |
| `COUNT()` | Counts rows         | `SELECT COUNT(*) FROM Orders;`       |
| `SUM()`   | Adds numeric values | `SELECT SUM(Salary) FROM Employees;` |
| `AVG()`   | Finds average       | `SELECT AVG(Age) FROM Employees;`    |
| `MIN()`   | Finds smallest      | `SELECT MIN(Price) FROM Products;`   |
| `MAX()`   | Finds largest       | `SELECT MAX(Price) FROM Products;`   |

💡 **Scenario:** In dashboards — e.g., total number of orders, highest sale value, average customer spending.

---

## 5. Joins

| Type           | Description                               | Example Scenario                                         |
| -------------- | ----------------------------------------- | -------------------------------------------------------- |
| `INNER JOIN` | Returns only matching rows                | Display orders with their customer details               |
| `LEFT JOIN`  | Returns all from left, matched from right | Show all customers, even those with no orders            |
| `RIGHT JOIN` | Returns all from right, matched from left | Show all orders, even those without customer data (rare) |
| `FULL JOIN`  | Returns all records from both sides       | Merge data from two departments                          |

### INNER JOIN Example

```sql
SELECT ProductName, CategoryName
FROM Products
INNER JOIN Categories ON Products.CategoryID = Categories.CategoryID;
```

💡 **Scenario:** Display products with their respective categories on an admin page.

### LEFT JOIN Example

```sql
SELECT c.CustomerName, o.OrderID
FROM Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID;
```

💡 **Scenario:** Identify customers who haven’t placed any orders.

### FULL JOIN Example

```sql
SELECT c.CustomerName, o.OrderID
FROM Customers c
FULL JOIN Orders o ON c.CustomerID = o.CustomerID;
```

💡 **Scenario:** Combine customer and order data for a comprehensive data migration.

---

## 6. GROUP BY Clause

Groups rows and performs aggregate functions.

```sql
SELECT CustomerID, COUNT(OrderID) AS OrderCount
FROM Orders
GROUP BY CustomerID;
```

💡 **Scenario:** Generate reports like number of orders per customer or monthly sales summaries.

---

## 7. Stored Procedures

### What is a Stored Procedure?

* A precompiled block of SQL code stored in the database.
* Can accept input parameters and return results.

### Example

```sql
CREATE PROCEDURE GetCustomersByCountry
    @Country NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Customers WHERE Country = @Country;
END;
```

Execute:

```sql
EXEC GetCustomersByCountry @Country = 'Mexico';
```

💡 **Scenario:** In enterprise systems, stored procedures are used for **common repetitive queries** (e.g., generating sales reports) and for **security** (to avoid direct table access).

---

## 8. SQL Constraints and Keys

### Common Constraints

| Constraint      | Description                     | Example Scenario                     |
| --------------- | ------------------------------- | ------------------------------------ |
| `NOT NULL`    | Prevents null values            | Every employee must have a name      |
| `UNIQUE`      | Ensures unique values           | No two users can have the same email |
| `PRIMARY KEY` | Uniquely identifies each record | Each product has a unique ProductID  |
| `FOREIGN KEY` | Links two tables                | Each order references a customer     |

💡 **Scenario:** When designing a  **student management system** , `StudentID` (PK) and `CourseID` (FK) maintain relationship integrity.

---

## 9. Indexes

Indexes speed up data retrieval at the cost of slower insert/update/delete operations.

### Clustered Index

```sql
CREATE CLUSTERED INDEX IX_Employee_EmpID ON Employee(EmpID);
```

💡 **Scenario:** Use clustered index on primary key columns like `EmployeeID`.

---

## 10. T-SQL (Transact-SQL)

### Variables

```sql
DECLARE @Name NVARCHAR(50);
SET @Name = 'Deepak';
SELECT 'Hello, ' + @Name AS Greeting;
```

💡 **Scenario:** Used inside stored procedures to store temporary data.

### Control Flow

```sql
DECLARE @Age INT = 25;
IF @Age >= 18
    PRINT 'Adult';
ELSE
    PRINT 'Minor';
```

💡 **Scenario:** Apply business logic (e.g., calculate tax rates based on age or income).

---

## 11. Functions

Create reusable logic blocks in SQL.

```sql
CREATE FUNCTION GetBonus(@Salary DECIMAL)
RETURNS DECIMAL
AS
BEGIN
    RETURN @Salary * 0.10;
END;
```

💡 **Scenario:** Use for  **salary calculation** ,  **tax computation** , or **discount rules** in enterprise applications.

---

## ✅ Quick Interview Tips

* Understand **real-time use** of each SQL concept — interviewers love scenario-based answers.
* Practice writing **joins and aggregate queries** from sample tables.
* Be ready to explain **why** you’d use a stored procedure, trigger, or index.
* Always mention **performance impact** when discussing indexing and joins.
