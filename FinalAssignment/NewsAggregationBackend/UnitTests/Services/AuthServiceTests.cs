using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Models;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using Moq;
using NewsAggregation.Enums;

namespace UnitTests.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        #region Private Fields

        private Mock<IUserService> _userServiceMock;
        private Mock<IJwtTokenService> _jwtTokenServiceMock;
        private Mock<IMapper> _mapperMock;
        private AuthService _authService;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _mapperMock = new Mock<IMapper>();
            _authService = new AuthService(_userServiceMock.Object, _jwtTokenServiceMock.Object, _mapperMock.Object);
        }

        #endregion

        [TestMethod]
        public async Task LoginAsync_ShouldThrowApiException_WhenLoginDtoIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _authService.LoginAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldThrowApiException_WhenUserNotFound()
        {
            var loginDto = new LoginDto { Username = "user", Password = "pass" };
            _userServiceMock.Setup(x => x.GetUserByName(loginDto.Username)).ReturnsAsync((User)null);

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _authService.LoginAsync(loginDto));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldThrowApiException_WhenPasswordInvalid()
        {
            var loginDto = new LoginDto { Username = "user", Password = "wrongpass" };
            var user = new User { Username = "user", PasswordHash = "hash", RoleId = 0, UserId = 1, Email = "a@b.com" };
            _userServiceMock.Setup(x => x.GetUserByName(loginDto.Username)).ReturnsAsync(user);
            _userServiceMock.Setup(x => x.VerifyPassword(user, loginDto.Password)).Returns(false);

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _authService.LoginAsync(loginDto));
            Assert.AreEqual(ErrorResponse.ErrorEnum.Validation, ex.ErrorCode);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnUserDataWithToken_WhenCredentialsAreValid()
        {
            var loginDto = new LoginDto { Username = "user", Password = "pass" };
            var user = new User { Username = "user", PasswordHash = "hash", RoleId = 0, UserId = 10, Email = "a@b.com" };
            var userReadDto = new UserReadDto { Username = "user", RoleId = 0, UserId = 10, Email = "a@b.com" };
            var token = "token";

            _userServiceMock.Setup(x => x.GetUserByName(loginDto.Username)).ReturnsAsync(user);
            _userServiceMock.Setup(x => x.VerifyPassword(user, loginDto.Password)).Returns(true);
            _mapperMock.Setup(x => x.Map<UserReadDto>(user)).Returns(userReadDto);
            _jwtTokenServiceMock.Setup(x => x.GenerateToken(user.UserId.ToString(), user.Username, user.Email, It.IsAny<List<string>>())).Returns(token);

            var result = await _authService.LoginAsync(loginDto);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual(userReadDto.Username, result.User.Username);
            Assert.AreEqual(token, result.token);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnUserDataWithToken_WhenRealUserCredentialsAreValid()
        {
            var loginDto = new LoginDto { Username = "user1234", Password = "User@1234" };
            var user = new User { Username = "user1234", PasswordHash = "hashedPassword", RoleId = RoleEnum.User, UserId = 1, Email = "user1234@example.com" };
            var userReadDto = new UserReadDto { Username = "user1234", RoleId = RoleEnum.User, UserId = 1, Email = "user1234@example.com" };
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidXNlcm5hbWUiOiJ1c2VyMTIzNCIsImVtYWlsIjoidXNlcjEyMzRAZXhhbXBsZS5jb20iLCJyb2xlcyI6WyIwIl0sIm5iZiI6MTY5OTk5OTk5OSwiZXhwIjoxNzAwMDg2Mzk5fQ.example";

            _userServiceMock.Setup(x => x.GetUserByName(loginDto.Username)).ReturnsAsync(user);
            _userServiceMock.Setup(x => x.VerifyPassword(user, loginDto.Password)).Returns(true);
            _mapperMock.Setup(x => x.Map<UserReadDto>(user)).Returns(userReadDto);
            _jwtTokenServiceMock.Setup(x => x.GenerateToken(user.UserId.ToString(), user.Username, user.Email, It.IsAny<List<string>>())).Returns(token);

            var result = await _authService.LoginAsync(loginDto);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual("user1234", result.User.Username);
            Assert.AreEqual("user1234@example.com", result.User.Email);
            Assert.AreEqual(1, result.User.UserId);
            Assert.AreEqual(RoleEnum.User, result.User.RoleId);
            Assert.AreEqual(token, result.token);
        }
    }
}
