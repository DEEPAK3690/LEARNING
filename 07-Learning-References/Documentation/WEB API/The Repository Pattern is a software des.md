# Repository Pattern

The Repository Pattern is a software design pattern that helps separate business logic from data access logic. It acts like a middle layer between your application and the database.

Instead of your app directly talking to a database (SQL, MongoDB, API, etc.), it talks to a  **repository** , which handles all data operations.

Your app → Repository → Data source
