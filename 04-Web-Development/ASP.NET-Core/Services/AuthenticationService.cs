using MyWebApplication.Models;

namespace MyWebApplication.Services
{
    /// <summary>
    /// Authentication Service
    /// 
    /// AUTHENTICATION vs AUTHORIZATION:
    /// 
    /// AUTHENTICATION (Who are you?)
    /// - Verifies the user's identity
    /// - Asks: "Are you really John Smith?"
    /// - Methods: Username/Password, OAuth, Biometric, etc.
    /// - Example: Checking if username and password match
    /// - Happens FIRST
    /// 
    /// AUTHORIZATION (What are you allowed to do?)
    /// - Determines what the authenticated user can access
    /// - Asks: "As an Admin, can you delete users?"
    /// - Based on: Roles, Permissions, Claims
    /// - Example: Only Admins can delete employees
    /// - Happens AFTER authentication
    /// 
    /// FLOW:
    /// 1. User submits login form (username, password)
    /// 2. AuthService validates credentials (AUTHENTICATION)
    /// 3. If valid, JWT token is generated with user's role
    /// 4. User sends token with each request
    /// 5. Authorization middleware checks user's role/permissions (AUTHORIZATION)
    /// 6. If authorized, request proceeds; if not, returns 403 Forbidden
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates user (verifies username and password)
        /// </summary>
        (bool success, User user, string message) AuthenticateUser(string username, string password);

        /// <summary>
        /// Validates if a password matches the stored hash
        /// Using bcrypt for security
        /// </summary>
        bool ValidatePassword(string plainPassword, string hash);

        /// <summary>
        /// Hashes a password for storage
        /// Never store plain text passwords!
        /// </summary>
        string HashPassword(string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Step 1 of Login Flow: AUTHENTICATION
        /// Verifies username and password are correct
        /// </summary>
        public (bool success, User user, string message) AuthenticateUser(string username, string password)
        {
            // Find user by username
            var user = _userRepository.GetByUsername(username);

            if (user == null)
            {
                return (false, null, "User not found");
            }

            if (!user.IsActive)
            {
                return (false, null, "User account is inactive");
            }

            // Validate password
            if (!ValidatePassword(password, user.PasswordHash))
            {
                return (false, null, "Invalid password");
            }

            return (true, user, "Authentication successful");
        }

        /// <summary>
        /// Validates password using bcrypt
        /// bcrypt is much more secure than simple hashing
        /// It's specifically designed for password storage
        /// 
        /// Why bcrypt?
        /// - Slow by design (prevents brute force attacks)
        /// - Salt built-in (prevents rainbow table attacks)
        /// - Industry standard for password hashing
        /// </summary>
        public bool ValidatePassword(string plainPassword, string hash)
        {
            try
            {
                // Using BCrypt.Net-Next NuGet package
                // In real projects: dotnet add package BCrypt.Net-Next
                return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password validation error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Hashes password using bcrypt
        /// Store this hash in database, never store plain password!
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
