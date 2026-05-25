using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration; // Crucial for reading environment variables

namespace PortfolioAPI.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // 1. Inject configuration to access environment variables securely
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            // 2. Pull credentials securely from Render
            var validUser = _configuration["ADMIN_USERNAME"];
            var validPass = _configuration["ADMIN_PASSWORD"];

            // 3. Safety check in case Render is missing the variables
            if (string.IsNullOrEmpty(validUser) || string.IsNullOrEmpty(validPass))
            {
                return StatusCode(500, "Server configuration error: Admin credentials missing.");
            }

            // Verify credentials
            if (request.Username == validUser && request.Password == validPass)
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new { token = token }); 
            }

            return Unauthorized(new { message = "Invalid credentials. Access Denied." });
        }

        private string GenerateJwtToken(string username)
        {
            // 4. Pull the JWT Key securely from Render
            var secretKey = _configuration["JWT_SECRET_KEY"];
            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("Server configuration error: JWT Secret Key missing.");
            }

            var issuer = "PortfolioAPI";
            var audience = "PortfolioUI";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Build the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4), // Token lasts for 4 hours
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO to catch the incoming JSON from Angular
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
