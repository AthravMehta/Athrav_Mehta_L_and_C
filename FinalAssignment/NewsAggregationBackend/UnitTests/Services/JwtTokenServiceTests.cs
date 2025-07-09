using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace UnitTests.Services
{
    [TestClass]
    public class JwtTokenServiceTests
    {
        #region Private Fields

        private Mock<IConfiguration> _configurationMock;
        private Mock<IConfigurationSection> _jwtSectionMock;
        private JwtTokenService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _jwtSectionMock = new Mock<IConfigurationSection>();
            
            _jwtSectionMock.Setup(x => x[AppConstants.JwtSecretKey]).Returns("TestSecretKey123456789012345678901234567890");
            _jwtSectionMock.Setup(x => x[AppConstants.JwtIssuer]).Returns("TestIssuer");
            _jwtSectionMock.Setup(x => x[AppConstants.JwtAudience]).Returns("TestAudience");
            
            _configurationMock.Setup(x => x.GetSection(AppConstants.JwtSection)).Returns(_jwtSectionMock.Object);
            
            _service = new JwtTokenService(_configurationMock.Object);
        }

        #endregion

        #region GenerateToken Tests

        [TestMethod]
        public void GenerateToken_ShouldReturnValidToken_WhenValidParameters()
        {
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User", "Admin" };

            var token = _service.GenerateToken(userId, username, email, roles);

            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.AreEqual(userId, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
            Assert.AreEqual(username, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value);
            Assert.AreEqual(email, jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value);
            Assert.AreEqual(2, jwtToken.Claims.Count(c => c.Type == ClaimTypes.Role));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "User"));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenUserIdIsNull()
        {
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(null, username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenUserIdIsEmpty()
        {
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken("", username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenUserIdIsWhitespace()
        {
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken("   ", username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenUsernameIsNull()
        {
            var userId = "123";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, null, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenUsernameIsEmpty()
        {
            var userId = "123";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, "", email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenUsernameIsWhitespace()
        {
            var userId = "123";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, "   ", email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenEmailIsNull()
        {
            var userId = "123";
            var username = "testuser";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, null, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenEmailIsEmpty()
        {
            var userId = "123";
            var username = "testuser";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, "", roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenEmailIsWhitespace()
        {
            var userId = "123";
            var username = "testuser";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, "   ", roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenRolesIsNull()
        {
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, email, null));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenRolesIsEmpty()
        {
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string>();

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenSecretKeyIsMissing()
        {
            _jwtSectionMock.Setup(x => x[AppConstants.JwtSecretKey]).Returns((string)null);
            
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenIssuerIsMissing()
        {
            _jwtSectionMock.Setup(x => x[AppConstants.JwtIssuer]).Returns((string)null);
            
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldThrowApiException_WhenAudienceIsMissing()
        {
            _jwtSectionMock.Setup(x => x[AppConstants.JwtAudience]).Returns((string)null);
            
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            Assert.ThrowsException<ApiException>(() => _service.GenerateToken(userId, username, email, roles));
        }

        [TestMethod]
        public void GenerateToken_ShouldGenerateTokenWithSingleRole_WhenOneRoleProvided()
        {
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };

            var token = _service.GenerateToken(userId, username, email, roles);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.AreEqual(1, jwtToken.Claims.Count(c => c.Type == ClaimTypes.Role));
            Assert.AreEqual("User", jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        }

        [TestMethod]
        public void GenerateToken_ShouldGenerateTokenWithMultipleRoles_WhenMultipleRolesProvided()
        {
            var userId = "123";
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User", "Admin", "Moderator" };

            var token = _service.GenerateToken(userId, username, email, roles);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.AreEqual(3, jwtToken.Claims.Count(c => c.Type == ClaimTypes.Role));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "User"));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Moderator"));
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateService_WhenValidConfiguration()
        {
            var service = new JwtTokenService(_configurationMock.Object);
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Constructor_ShouldThrowApiException_WhenConfigurationIsNull()
        {
            Assert.ThrowsException<ApiException>(() => new JwtTokenService(null));
        }

        #endregion
    }
} 