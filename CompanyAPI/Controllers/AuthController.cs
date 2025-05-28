using CompanyAPI.Application.Features.Authentication.DTOs;
using CompanyAPI.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CompanyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IOptions<JwtSettings> jwtSettings, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Login and get JWT token
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (IsValidUser(request.Username, request.Password))
            {
                var token = GenerateJwtToken(request.Username);
                var expiry = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<int>("Jwt:ExpiryInMinutes", 60));

                _logger.LogInformation("User {Username} logged in successfully", request.Username);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Expiry = expiry,
                    TokenType = "Bearer"
                });
            }

            _logger.LogWarning("Failed login attempt for username {Username}", request.Username);
            return Unauthorized(new { error = "Invalid username or password" });
        }

        private bool IsValidUser(string username, string password)
        {
            // Simple hardcoded users for test purposes
            // In production, validate against a proper user store
            var validUsers = new Dictionary<string, string>
            {
                { "admin", "password123" },
                { "user", "user123" },
                { "demo", "demo123" }
            };

            return validUsers.ContainsKey(username) && validUsers[username] == password;
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
