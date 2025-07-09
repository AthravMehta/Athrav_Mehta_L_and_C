using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NewsAggregation.Controllers;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsAggregation.Constants;
using NewsAggregation.Utilities;
using NewsAggregation.Enums;
using NewsAggregation.Entities;

namespace UnitTests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        #region Private Fields

        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _controller;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _controller = new UserController(_userServiceMock.Object, _loggerMock.Object);
        }

        #endregion

        #region CreateUser Tests

        [TestMethod]
        public async Task CreateUser_ShouldReturnOkResult_WhenValidUser()
        {
            var userDto = new UserCreateDto
            {
                Username = "testuser",
                Password = "Test@123",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            var expectedResult = new UserDataWithTokenDto
            {
                User = new UserReadDto
                {
                    UserId = 1,
                    Username = "testuser",
                    Email = "test@example.com",
                    RoleId = RoleEnum.User,
                    CreatedDateTime = DateTime.Now,
                    LastUpdatedDateTime = DateTime.Now
                },
                token = "sample-token"
            };

            _userServiceMock.Setup(x => x.CreateUserAsync(userDto)).Returns(Task.FromResult(expectedResult));

            var result = await _controller.CreateUser(userDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task CreateUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var userDto = new UserCreateDto();
            _controller.ModelState.AddModelError("Username", "Username is required");

            var result = await _controller.CreateUser(userDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task CreateUser_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var userDto = new UserCreateDto
            {
                Username = "testuser",
                Password = "Test@123",
                Email = "test@example.com",
                RoleId = RoleEnum.User
            };

            _userServiceMock.Setup(x => x.CreateUserAsync(userDto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.CreateUser(userDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetAllUsers Tests

        [TestMethod]
        public async Task GetAllUsers_ShouldReturnOkResult_WhenUsersExist()
        {
            var expectedUsers = new List<UserReadDto>
            {
                new UserReadDto
                {
                    UserId = 1,
                    Username = "user1",
                    Email = "user1@example.com",
                    RoleId = RoleEnum.User,
                    CreatedDateTime = DateTime.Now,
                    LastUpdatedDateTime = DateTime.Now
                },
                new UserReadDto
                {
                    UserId = 2,
                    Username = "user2",
                    Email = "user2@example.com",
                    RoleId = RoleEnum.User,
                    CreatedDateTime = DateTime.Now,
                    LastUpdatedDateTime = DateTime.Now
                }
            };

            _userServiceMock.Setup(x => x.GetAllUsersAsync(RoleEnum.User))
                .ReturnsAsync(expectedUsers);

            var result = await _controller.GetAllUsers(RoleEnum.User);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUsers, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            var expectedUsers = new List<UserReadDto>();

            _userServiceMock.Setup(x => x.GetAllUsersAsync(RoleEnum.User))
                .ReturnsAsync(expectedUsers);

            var result = await _controller.GetAllUsers(RoleEnum.User);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUsers, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllUsers_ShouldReturnAdminUsers_WhenRoleIsAdmin()
        {
            var expectedUsers = new List<UserReadDto>
            {
                new UserReadDto
                {
                    UserId = 1,
                    Username = "admin1",
                    Email = "admin1@example.com",
                    RoleId = RoleEnum.Admin,
                    CreatedDateTime = DateTime.Now,
                    LastUpdatedDateTime = DateTime.Now
                }
            };

            _userServiceMock.Setup(x => x.GetAllUsersAsync(RoleEnum.Admin))
                .ReturnsAsync(expectedUsers);

            var result = await _controller.GetAllUsers(RoleEnum.Admin);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUsers, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllUsers_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _userServiceMock.Setup(x => x.GetAllUsersAsync(RoleEnum.User))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetAllUsers(RoleEnum.User);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetUserByName Tests

        [TestMethod]
        public async Task GetUserByName_ShouldReturnOkResult_WhenUserExists()
        {
            var username = "testuser";
            var expectedUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                RoleId = RoleEnum.User,
                CreatedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now
            };

            _userServiceMock.Setup(x => x.GetUserByName(username)).Returns(Task.FromResult(expectedUser));

            var result = await _controller.GetUserByName(username);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUser, okResult.Value);
        }

        [TestMethod]
        public async Task GetUserByName_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var username = "nonexistentuser";

            _userServiceMock.Setup(x => x.GetUserByName(username))
                .ReturnsAsync((User)null);

            var result = await _controller.GetUserByName(username);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task GetUserByName_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var username = "testuser";

            _userServiceMock.Setup(x => x.GetUserByName(username))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetUserByName(username);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion
    }
} 