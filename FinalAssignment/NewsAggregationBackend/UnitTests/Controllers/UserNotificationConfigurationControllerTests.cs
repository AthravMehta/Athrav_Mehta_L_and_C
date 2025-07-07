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

namespace UnitTests.Controllers
{
    [TestClass]
    public class UserNotificationConfigurationControllerTests
    {
        #region Private Fields

        private Mock<IUserNotificationConfigurationService> _serviceMock;
        private Mock<ILogger<UserNotificationConfigurationController>> _loggerMock;
        private UserNotificationConfigurationController _controller;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IUserNotificationConfigurationService>();
            _loggerMock = new Mock<ILogger<UserNotificationConfigurationController>>();
            _controller = new UserNotificationConfigurationController(_loggerMock.Object, _serviceMock.Object);
        }

        #endregion

        #region UpdateNotificationConfiguration Tests

        [TestMethod]
        public async Task UpdateNotificationConfiguration_ShouldReturnOkResult_WhenConfigurationUpdated()
        {
            var dto = new UserNotificationConfigurationDto
            {
                UserId = 1,
                CategoryId = 1,
                IsEnabled = true
            };

            var expectedResult = new UserNotificationConfigurationDto
            {
                UserNotificationConfigurationId = 1,
                UserId = 1,
                CategoryId = 1,
                IsEnabled = true
            };

            _serviceMock.Setup(x => x.UpdateAsync(1, dto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.UpdateNotificationConfiguration(1, dto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateNotificationConfiguration_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var dto = new UserNotificationConfigurationDto();
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            var result = await _controller.UpdateNotificationConfiguration(1, dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateNotificationConfiguration_ShouldReturnNotFound_WhenConfigurationDoesNotExist()
        {
            var dto = new UserNotificationConfigurationDto
            {
                UserId = 1,
                CategoryId = 1,
                IsEnabled = true
            };

            _serviceMock.Setup(x => x.UpdateAsync(999, dto))
                .ReturnsAsync((UserNotificationConfigurationDto)null);

            var result = await _controller.UpdateNotificationConfiguration(999, dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateNotificationConfiguration_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var dto = new UserNotificationConfigurationDto
            {
                UserId = 1,
                CategoryId = 1,
                IsEnabled = true
            };

            _serviceMock.Setup(x => x.UpdateAsync(1, dto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.UpdateNotificationConfiguration(1, dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetAllNotificationConfigurations Tests

        [TestMethod]
        public async Task GetAllNotificationConfigurations_ShouldReturnOkResult_WhenConfigurationsExist()
        {
            var expectedConfigurations = new List<UserNotificationConfigurationDto>
            {
                new UserNotificationConfigurationDto
                {
                    UserNotificationConfigurationId = 1,
                    UserId = 1,
                    CategoryId = 1,
                    IsEnabled = true
                },
                new UserNotificationConfigurationDto
                {
                    UserNotificationConfigurationId = 2,
                    UserId = 1,
                    CategoryId = 2,
                    IsEnabled = false
                }
            };

            _serviceMock.Setup(x => x.GetAllUserConfigurationAsync())
                .ReturnsAsync(expectedConfigurations);

            var result = await _controller.GetAllNotificationConfigurations();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedConfigurations, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllNotificationConfigurations_ShouldReturnEmptyList_WhenNoConfigurationsExist()
        {
            var expectedConfigurations = new List<UserNotificationConfigurationDto>();

            _serviceMock.Setup(x => x.GetAllUserConfigurationAsync())
                .ReturnsAsync(expectedConfigurations);

            var result = await _controller.GetAllNotificationConfigurations();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedConfigurations, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllNotificationConfigurations_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _serviceMock.Setup(x => x.GetAllUserConfigurationAsync())
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetAllNotificationConfigurations();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region InitializeNotificationConfigurations Tests

        [TestMethod]
        public async Task InitializeNotificationConfigurations_ShouldReturnOkResult_WhenInitializationSuccessful()
        {
            _serviceMock.Setup(x => x.InitializeNotificationConfigurationsAsync())
                .Returns(Task.CompletedTask);

            var result = await _controller.InitializeNotificationConfigurations();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value;
            var message = value.GetType().GetProperty("Message")?.GetValue(value, null) as string;
            Assert.AreEqual(SuccessConstants.UNCInitialize, message);
        }

        [TestMethod]
        public async Task InitializeNotificationConfigurations_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _serviceMock.Setup(x => x.InitializeNotificationConfigurationsAsync())
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.InitializeNotificationConfigurations();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion
    }
} 