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
    public class UserNotificationControllerTests
    {
        #region Private Fields

        private Mock<IUserNotificationService> _serviceMock;
        private Mock<ILogger<UserNotificationController>> _loggerMock;
        private UserNotificationController _controller;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IUserNotificationService>();
            _loggerMock = new Mock<ILogger<UserNotificationController>>();
            _controller = new UserNotificationController(_loggerMock.Object, _serviceMock.Object);
        }

        #endregion

        #region GetAllNotifications Tests

        [TestMethod]
        public async Task GetAllNotifications_ShouldReturnOkResult_WhenNotificationsExist()
        {
            var expectedNotifications = new List<UserNotificationDto>
            {
                new UserNotificationDto
                {
                    UserNotificationId = 1,
                    UserId = 1,
                    ArticleId = 1,
                    SentDateTime = DateTime.Now,
                    IsRead = false
                },
                new UserNotificationDto
                {
                    UserNotificationId = 2,
                    UserId = 1,
                    ArticleId = 2,
                    SentDateTime = DateTime.Now,
                    IsRead = true
                }
            };

            _serviceMock.Setup(x => x.GetAllUserNotificationAsync())
                .ReturnsAsync(expectedNotifications);

            var result = await _controller.GetAllNotifications();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedNotifications, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllNotifications_ShouldReturnEmptyList_WhenNoNotificationsExist()
        {
            var expectedNotifications = new List<UserNotificationDto>();

            _serviceMock.Setup(x => x.GetAllUserNotificationAsync())
                .ReturnsAsync(expectedNotifications);

            var result = await _controller.GetAllNotifications();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedNotifications, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllNotifications_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _serviceMock.Setup(x => x.GetAllUserNotificationAsync())
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetAllNotifications();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region AddNotification Tests

        [TestMethod]
        public async Task AddNotification_ShouldReturnCreatedAtAction_WhenValidNotification()
        {
            var notificationDto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = false
            };

            var expectedResult = new UserNotificationDto
            {
                UserNotificationId = 1,
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = false
            };

            _serviceMock.Setup(x => x.AddAsync(notificationDto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.AddNotification(notificationDto);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(UserNotificationController.AddNotification), createdAtActionResult.ActionName);
            Assert.AreEqual(expectedResult, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task AddNotification_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var notificationDto = new UserNotificationDto();
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            var result = await _controller.AddNotification(notificationDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task AddNotification_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var notificationDto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = false
            };

            _serviceMock.Setup(x => x.AddAsync(notificationDto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.AddNotification(notificationDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region UpdateNotification Tests

        [TestMethod]
        public async Task UpdateNotification_ShouldReturnOkResult_WhenNotificationUpdated()
        {
            var notificationDto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = true
            };

            var expectedResult = new UserNotificationDto
            {
                UserNotificationId = 1,
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = true
            };

            _serviceMock.Setup(x => x.UpdateAsync(1, notificationDto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.UpdateNotification(1, notificationDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateNotification_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var notificationDto = new UserNotificationDto();
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            var result = await _controller.UpdateNotification(1, notificationDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateNotification_ShouldReturnNotFound_WhenNotificationDoesNotExist()
        {
            var notificationDto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = true
            };

            _serviceMock.Setup(x => x.UpdateAsync(999, notificationDto))
                .ReturnsAsync((UserNotificationDto)null);

            var result = await _controller.UpdateNotification(999, notificationDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateNotification_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var notificationDto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 1,
                SentDateTime = DateTime.Now,
                IsRead = true
            };

            _serviceMock.Setup(x => x.UpdateAsync(1, notificationDto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.UpdateNotification(1, notificationDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region DeleteNotification Tests

        [TestMethod]
        public async Task DeleteNotification_ShouldReturnOkResult_WhenNotificationDeleted()
        {
            _serviceMock.Setup(x => x.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteNotification(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value;
            var message = value.GetType().GetProperty("Message")?.GetValue(value, null) as string;
            Assert.AreEqual(SuccessConstants.CategoryHidden, message);
        }

        [TestMethod]
        public async Task DeleteNotification_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _serviceMock.Setup(x => x.DeleteAsync(1))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.DeleteNotification(1);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion
    }
} 