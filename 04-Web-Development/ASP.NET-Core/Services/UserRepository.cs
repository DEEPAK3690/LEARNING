using MyWebApplication.Models;

namespace MyWebApplication.Services
{
    /// <summary>
    /// Repository for managing users
    /// In a real project, this would connect to a database (SQL Server, PostgreSQL, etc.)
    /// </summary>
    public interface IUserRepository
    {
        User GetByUsername(string username);
        User GetById(int id);
        void AddUser(User user);
        bool UserExists(string username);
    }

    public class UserRepository : IUserRepository
    {
        // In-memory storage (for demo purposes)
        // In real projects: Replace with Entity Framework, Dapper, etc.
        private static readonly List<User> Users = new()
        {
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                // Password: "Admin@123" (hashed - NEVER store plain text!)
                PasswordHash = "$2a$11$SlVLhQe2YkHkfmXxBQaG1OZ7lPJSLJvvDNMmzQqn0Zz3bYPJrFX0i",
                Role = "Admin",
                CreatedAt = DateTime.Now,
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "manager",
                Email = "manager@example.com",
                // Password: "Manager@123"
                PasswordHash = "$2a$11$wBrJJNEv1sQSUNkGCqBpTeQkrXVvmH5.6IH.kRq1qZVv3xzLO7ola",
                Role = "Manager",
                CreatedAt = DateTime.Now,
                IsActive = true
            },
            new User
            {
                Id = 3,
                Username = "employee",
                Email = "employee@example.com",
                // Password: "Employee@123"
                PasswordHash = "$2a$11$v1j7ljM0iGxZNvCBc/LmuezG7l9tGxvpR7yqN.mKx7gXX.7nkw9va",
                Role = "Employee",
                CreatedAt = DateTime.Now,
                IsActive = true
            }
        };

        public User GetByUsername(string username)
        {
            return Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public User GetById(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            user.Id = Users.Max(u => u.Id) + 1;
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            Users.Add(user);
        }

        public bool UserExists(string username)
        {
            return Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
