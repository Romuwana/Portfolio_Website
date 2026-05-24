using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioAPI.Controllers // Change this namespace if yours is different!
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            // 1. HARDCODED CREDENTIALS
            var validUser = "Romu";       // <-- Put your original username here
            var validPass = "Hellen@2005**";    // <-- Put your original password here

            // Verify credentials
            if (request.Username == validUser && request.Password == validPass)
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new { token = token }); // Returns the token to Angular
            }

            return Unauthorized(new { message = "Invalid credentials. Access Denied." });
        }

        private string GenerateJwtToken(string username)
        {
            // 2. HARDCODED JWT SETTINGS
            var secretKey = "ThisIsAMassiveSecretKeyForYourPortfolioThatNeedsToBeAtLeast32CharactersLong!";
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