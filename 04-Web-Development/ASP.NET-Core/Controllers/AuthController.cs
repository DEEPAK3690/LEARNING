using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models;
using MyWebApplication.Services;

namespace MyWebApplication.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// 
    /// WHAT IS THIS?
    /// - Handles user login/registration
    /// - Generates JWT tokens for authenticated users
    /// - Public endpoints (no authentication required)
    /// 
    /// KEY ENDPOINTS:
    /// 1. POST /api/auth/login - Authenticates user and returns JWT token
    /// 2. POST /api/auth/register - Creates a new user account
    /// 3. POST /api/auth/validate - Validates a JWT token
    /// 
    /// SECURITY NOTES:
    /// - These endpoints are PUBLIC (no [Authorize] attribute)
    /// - Password is hashed using bcrypt
    /// - JWT token expires after 60 minutes
    /// - Always use HTTPS in production
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _tokenService;

        public AuthController(
            IAuthenticationService authService,
            IUserRepository userRepository,
            IJwtTokenService tokenService)
        {
            _authService = authService;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// POST /api/auth/login
        /// 
        /// WHAT HAPPENS HERE:
        /// 1. User sends username and password
        /// 2. We verify credentials (AUTHENTICATION)
        /// 3. If valid, we generate JWT token
        /// 4. Token is sent back to client
        /// 5. Client uses token for subsequent requests
        /// 
        /// CURL EXAMPLE:
        /// curl -X POST "https://localhost:5001/api/auth/login" \
        ///   -H "Content-Type: application/json" \
        ///   -d '{"username":"admin","password":"Admin@123"}'
        /// 
        /// RESPONSE:
        /// {
        ///   "success": true,
        ///   "message": "Login successful",
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "user": {
        ///     "id": 1,
        ///     "username": "admin",
        ///     "email": "admin@example.com",
        ///     "role": "Admin"
        ///   }
        /// }
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Username and password are required" });
            }

            // Step 1: Authenticate (verify username and password)
            var (success, user, message) = _authService.AuthenticateUser(request.Username, request.Password);

            if (!success)
            {
                // Return 401 Unauthorized - Invalid credentials
                return Unauthorized(new { success = false, message });
            }

            // Step 2: Generate JWT token (if authentication succeeded)
            var token = _tokenService.GenerateToken(user);

            // Step 3: Return token to client
            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                }
            });
        }

        /// <summary>
        /// POST /api/auth/register
        /// 
        /// Creates a new user account
        /// In real projects, add email verification, password strength requirements, etc.
        /// 
        /// CURL EXAMPLE:
        /// curl -X POST "https://localhost:5001/api/auth/register" \
        ///   -H "Content-Type: application/json" \
        ///   -d '{"username":"newuser","email":"newuser@example.com","password":"Pass@123"}'
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Username, email, and password are required" });
            }

            // Check if user already exists
            if (_userRepository.UserExists(request.Username))
            {
                return BadRequest(new { success = false, message = "Username already exists" });
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _authService.HashPassword(request.Password),
                Role = "Employee", // Default role for new users
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _userRepository.AddUser(user);

            return Ok(new { success = true, message = "User registered successfully" });
        }

        /// <summary>
        /// GET /api/auth/validate
        /// 
        /// Validates a JWT token (optional endpoint)
        /// Used to check if token is still valid
        /// 
        /// CURL EXAMPLE:
        /// curl -X GET "https://localhost:5001/api/auth/validate" \
        ///   -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// </summary>
        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            // Get token from Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { success = false, message = "No token provided" });
            }

            var principal = _tokenService.GetPrincipalFromToken(token);

            if (principal == null)
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new 
            { 
                success = true, 
                message = "Token is valid",
                userId,
                role
            });
        }
    }

    /// <summary>
    /// Request model for user registration
    /// </summary>
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
