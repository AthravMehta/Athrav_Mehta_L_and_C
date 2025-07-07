using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        #region Private Fields

        private Mock<IMapper> _mapperMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IJwtTokenService> _jwtTokenServiceMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<ICrudBaseRepository<User>> _crudBaseRepositoryMock;
        private Mock<ILogger<CrudBaseService<User>>> _loggerMock;
        private Mock<IUserNotificationConfigurationService> _userNotificationConfigurationServiceMock;
        private UserService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _crudBaseRepositoryMock = new Mock<ICrudBaseRepository<User>>();
            _loggerMock = new Mock<ILogger<CrudBaseService<User>>>();
            _userNotificationConfigurationServiceMock = new Mock<IUserNotificationConfigurationService>();
            _service = new UserService(
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _jwtTokenServiceMock.Object,
                _encryptionServiceMock.Object,
                _crudBaseRepositoryMock.Object,
                _loggerMock.Object,
                _userNotificationConfigurationServiceMock.Object
            );
        }

        #endregion

        #region CreateUserAsync Tests

        [TestMethod]
        public async Task CreateUserAsync_ShouldReturnUserDataWithToken_WhenValidUser()
        {
            var userDto = new UserCreateDto
            {
                Username = "testuser123",
                Password = "Test@123",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            var user = new User
            {
                UserId = 1,
                Username = "testuser123",
                Email = "test@example.com",
                RoleId = RoleEnum.User,
                CreatedDateTime = DateTime.UtcNow,
                LastUpdatedDateTime = DateTime.UtcNow
            };

            var userReadDto = new UserReadDto
            {
                UserId = 1,
                Username = "testuser123",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            var token = "jwt_token_here";

            _userRepositoryMock.Setup(x => x.GetUserByName(userDto.Username)).ReturnsAsync((User)null);
            _mapperMock.Setup(x => x.Map<User>(userDto)).Returns(user);
            _encryptionServiceMock.Setup(x => x.Encrypt(userDto.Password)).Returns("hashed_password");
            _crudBaseRepositoryMock.Setup(x => x.AddAsync(user)).Returns(Task.CompletedTask);
            _userNotificationConfigurationServiceMock.Setup(x => x.CreateNotificationConfigForAllUsersAsync(null, user)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<UserReadDto>(user)).Returns(userReadDto);
            _jwtTokenServiceMock.Setup(x => x.GenerateToken(user.UserId.ToString(), user.Username, user.Email, It.IsAny<List<string>>())).Returns(token);

            var result = await _service.CreateUserAsync(userDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(userReadDto, result.User);
            Assert.AreEqual(token, result.token);
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldThrowApiException_WhenUserAlreadyExists()
        {
            var userDto = new UserCreateDto
            {
                Username = "existinguser",
                Password = "Test@123",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            var existingUser = new User { UserId = 1, Username = "existinguser" };
            _userRepositoryMock.Setup(x => x.GetUserByName(userDto.Username)).ReturnsAsync(existingUser);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateUserAsync(userDto));
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateUserAsync(null));
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldThrowApiException_WhenUsernameIsTooShort()
        {
            var userDto = new UserCreateDto
            {
                Username = "short",
                Password = "Test@123",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateUserAsync(userDto));
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldThrowApiException_WhenPasswordIsInvalid()
        {
            var userDto = new UserCreateDto
            {
                Username = "testuser123",
                Password = "weak",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateUserAsync(userDto));
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldThrowApiException_WhenEmailIsInvalid()
        {
            var userDto = new UserCreateDto
            {
                Username = "testuser123",
                Password = "Test@123",
                Email = "invalid-email",
                RoleId = RoleEnum.User
            };

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateUserAsync(userDto));
        }

        #endregion

        #region GetAllUsersAsync Tests

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExist()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", RoleId = RoleEnum.User },
                new User { UserId = 2, Username = "user2", Email = "user2@example.com", RoleId = RoleEnum.User }
            };

            var userReadDtos = new List<UserReadDto>
            {
                new UserReadDto { UserId = 1, Username = "user1", Email = "user1@example.com", RoleId = RoleEnum.User },
                new UserReadDto { UserId = 2, Username = "user2", Email = "user2@example.com", RoleId = RoleEnum.User }
            };

            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(RoleEnum.User)).ReturnsAsync(users);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserReadDto>>(users)).Returns(userReadDtos);

            var result = await _service.GetAllUsersAsync(RoleEnum.User);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("user1", result.First().Username);
            Assert.AreEqual("user2", result.Last().Username);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnAdminUsers_WhenRoleIsAdmin()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "admin1", Email = "admin1@example.com", RoleId = RoleEnum.Admin }
            };

            var userReadDtos = new List<UserReadDto>
            {
                new UserReadDto { UserId = 1, Username = "admin1", Email = "admin1@example.com", RoleId = RoleEnum.Admin }
            };

            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(RoleEnum.Admin)).ReturnsAsync(users);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserReadDto>>(users)).Returns(userReadDtos);

            var result = await _service.GetAllUsersAsync(RoleEnum.Admin);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("admin1", result.First().Username);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldThrowApiException_WhenNoUsersExist()
        {
            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(RoleEnum.User)).ReturnsAsync((List<User>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllUsersAsync(RoleEnum.User));
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldThrowApiException_WhenEmptyUsersList()
        {
            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(RoleEnum.User)).ReturnsAsync(new List<User>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllUsersAsync(RoleEnum.User));
        }

        #endregion

        #region GetUserByName Tests

        [TestMethod]
        public async Task GetUserByName_ShouldReturnUser_WhenUserExists()
        {
            var username = "testuser";
            var user = new User { UserId = 1, Username = "testuser", Email = "test@example.com", RoleId = RoleEnum.User };

            _userRepositoryMock.Setup(x => x.GetUserByName(username)).ReturnsAsync(user);

            var result = await _service.GetUserByName(username);

            Assert.AreEqual(user, result);
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowApiException_WhenUsernameIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetUserByName(null));
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowApiException_WhenUsernameIsEmpty()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetUserByName(""));
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowApiException_WhenUsernameIsWhitespace()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetUserByName("   "));
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowApiException_WhenUserDoesNotExist()
        {
            var username = "nonexistentuser";
            _userRepositoryMock.Setup(x => x.GetUserByName(username)).ReturnsAsync((User)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetUserByName(username));
        }

        #endregion

        #region VerifyPassword Tests

        [TestMethod]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsValid()
        {
            var user = new User { UserId = 1, Username = "testuser", PasswordHash = "hashed_password" };
            var providedPassword = "Test@123";

            _encryptionServiceMock.Setup(x => x.Verify(user.PasswordHash, providedPassword)).Returns(true);

            var result = _service.VerifyPassword(user, providedPassword);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsInvalid()
        {
            var user = new User { UserId = 1, Username = "testuser", PasswordHash = "hashed_password" };
            var providedPassword = "WrongPassword";

            _encryptionServiceMock.Setup(x => x.Verify(user.PasswordHash, providedPassword)).Returns(false);

            var result = _service.VerifyPassword(user, providedPassword);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerifyPassword_ShouldThrowApiException_WhenUserIsNull()
        {
            Assert.ThrowsException<ApiException>(() => _service.VerifyPassword(null, "password"));
        }

        [TestMethod]
        public void VerifyPassword_ShouldThrowApiException_WhenPasswordIsNull()
        {
            var user = new User { UserId = 1, Username = "testuser" };
            Assert.ThrowsException<ApiException>(() => _service.VerifyPassword(user, null));
        }

        [TestMethod]
        public void VerifyPassword_ShouldThrowApiException_WhenPasswordIsEmpty()
        {
            var user = new User { UserId = 1, Username = "testuser" };
            Assert.ThrowsException<ApiException>(() => _service.VerifyPassword(user, ""));
        }

        [TestMethod]
        public void VerifyPassword_ShouldThrowApiException_WhenPasswordIsWhitespace()
        {
            var user = new User { UserId = 1, Username = "testuser" };
            Assert.ThrowsException<ApiException>(() => _service.VerifyPassword(user, "   "));
        }

        #endregion
    }
} 