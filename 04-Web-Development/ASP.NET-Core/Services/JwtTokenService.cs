using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyWebApplication.Models;

namespace MyWebApplication.Services
{
    /// <summary>
    /// Service for generating JWT (JSON Web Tokens)
    /// 
    /// WHY JWT?
    /// - Stateless: Server doesn't need to store session info
    /// - Scalable: Works great in distributed/cloud systems
    /// - Mobile-friendly: Perfect for API authentication
    /// - Secure: Contains signature that can't be forged
    /// 
    /// HOW IT WORKS:
    /// 1. User logs in with username/password (Authentication)
    /// 2. Server validates credentials and generates JWT
    /// 3. Client stores JWT and sends it with each request
    /// 4. Server validates JWT signature to confirm it's authentic
    /// 5. Server reads claims from JWT to determine permissions (Authorization)
    /// 
    /// JWT STRUCTURE: header.payload.signature
    /// - header: Algorithm used (HS256)
    /// - payload: User info (userId, role, email, etc.)
    /// - signature: HMAC hash to verify token wasn't modified
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            // In real projects, NEVER hardcode secrets!
            // Use environment variables, Azure Key Vault, etc.
            _secretKey = configuration["Jwt:SecretKey"] ?? "this-is-a-super-secret-key-min-32-characters-long!!!";
            _issuer = configuration["Jwt:Issuer"] ?? "MyWebApplication";
            _audience = configuration["Jwt:Audience"] ?? "MyWebApplicationUsers";
            _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        /// <summary>
        /// Generates a JWT token for the user
        /// This token is sent back to the client after successful login
        /// </summary>
        public string GenerateToken(User user)
        {
            // Create claims - these are the data inside the token
            var claims = new List<Claim>
            {
                // Standard claims
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                // Custom claim for role (used in Authorization)
                new Claim(ClaimTypes.Role, user.Role),
                // Custom claims
                new Claim("userId", user.Id.ToString()),
                new Claim("role", user.Role)
            };

            // Create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            // Return token as string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Extracts claims from a JWT token
        /// Used to validate token and get user information
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
