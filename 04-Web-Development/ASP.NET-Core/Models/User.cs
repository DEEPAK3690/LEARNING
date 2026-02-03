namespace MyWebApplication.Models
{
    /// <summary>
    /// Represents a user in the system
    /// Used for Authentication (verifying who the user is)
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Hashed password (never store plain text passwords!)
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// User's role determines their Authorization (what they can do)
        /// </summary>
        public string Role { get; set; } // "Admin", "Manager", "Employee"
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Request model for user login
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Response model for successful login
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // JWT Token
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// User information sent in response (safe to send to client)
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
