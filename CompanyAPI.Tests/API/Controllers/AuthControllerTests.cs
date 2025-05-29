using CompanyAPI.Application.Features.Authentication.DTOs;
using CompanyAPI.Auth;
using CompanyAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CompanyAPI.Tests.API.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            var jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsAVerySecretKeyForTestingPurposesOnly123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationMinutes = 60
            };

            var jwtOptions = Options.Create(jwtSettings);

            _configurationMock.Setup(x => x.GetSection("Jwt:ExpiryInMinutes"))
                .Returns(new Mock<IConfigurationSection>().Object);

            _controller = new AuthController(_configurationMock.Object, jwtOptions, _loggerMock.Object);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "password123"
            };

            // Act
            var result = _controller.Login(loginRequest) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var response = result.Value as LoginResponse;
            response.Should().NotBeNull();
            response.Token.Should().NotBeNullOrEmpty();
            response.TokenType.Should().Be("Bearer");
            response.Expiry.Should().BeAfter(System.DateTime.UtcNow);
        }

        [Theory]
        [InlineData("admin", "wrongpassword")]
        [InlineData("wronguser", "password123")]
        [InlineData("", "")]
        public void Login_InvalidCredentials_ReturnsUnauthorized(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            // Act
            var result = _controller.Login(loginRequest) as UnauthorizedObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(401);
        }

        [Theory]
        [InlineData("user", "user123")]
        [InlineData("demo", "demo123")]
        public void Login_OtherValidUsers_ReturnsOkWithToken(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            // Act
            var result = _controller.Login(loginRequest) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var response = result.Value as LoginResponse;
            response.Should().NotBeNull();
            response.Token.Should().NotBeNullOrEmpty();
        }
    }
}
