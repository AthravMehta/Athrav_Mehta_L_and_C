using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
using NewsAggregation.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    [TestClass]
    public class UserNotificationServiceTests
    {
        #region Private Fields

        private Mock<ICrudBaseRepository<UserNotification>> _crudBaseRepositoryMock;
        private Mock<IUserNotificationRepository> _userNotificationRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private RequestContext _requestContext;
        private UserNotificationService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _crudBaseRepositoryMock = new Mock<ICrudBaseRepository<UserNotification>>();
            _userNotificationRepositoryMock = new Mock<IUserNotificationRepository>();
            _mapperMock = new Mock<IMapper>();
            _requestContext = new RequestContext { UserId = 1, Email = "test@example.com", Roles = new List<string> { "User" } };
            _service = new UserNotificationService(
                _crudBaseRepositoryMock.Object,
                _userNotificationRepositoryMock.Object,
                _requestContext,
                _mapperMock.Object
            );
        }

        #endregion

        #region AddAsync Tests

        [TestMethod]
        public async Task AddAsync_ShouldReturnNotification_WhenValidNotification()
        {
            var dto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 2,
                SentDateTime = DateTime.Now,
                IsRead = false
            };

            var entity = new UserNotification
            {
                UserNotificationId = 1,
                UserId = 1,
                ArticleId = 2,
                SentDateTime = DateTime.Now,
                IsRead = false
            };

            _mapperMock.Setup(x => x.Map<UserNotification>(dto)).Returns(entity);
            _crudBaseRepositoryMock.Setup(x => x.AddAsync(entity)).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            Assert.AreEqual(1, result.UserNotificationId);
            Assert.AreEqual(dto.UserId, result.UserId);
            Assert.AreEqual(dto.ArticleId, result.ArticleId);
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.AddAsync(null));
        }

        #endregion

        #region AddRangeAsync Tests

        [TestMethod]
        public async Task AddRangeAsync_ShouldAddNotifications_WhenValidNotifications()
        {
            var notifications = new List<UserNotification>
            {
                new UserNotification { UserId = 1, ArticleId = 1, SentDateTime = DateTime.Now, IsRead = false },
                new UserNotification { UserId = 1, ArticleId = 2, SentDateTime = DateTime.Now, IsRead = false }
            };

            _userNotificationRepositoryMock.Setup(x => x.AddRangeAsync(notifications)).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.AddRangeAsync(notifications);

            _userNotificationRepositoryMock.Verify(x => x.AddRangeAsync(notifications), Times.Once);
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowApiException_WhenNotificationsIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.AddRangeAsync(null));
        }

        #endregion

        #region UpdateAsync Tests

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedNotification_WhenNotificationExists()
        {
            var dto = new UserNotificationDto
            {
                UserId = 1,
                ArticleId = 2,
                SentDateTime = DateTime.Now,
                IsRead = true
            };

            var entity = new UserNotification
            {
                UserNotificationId = 1,
                UserId = 1,
                ArticleId = 2,
                SentDateTime = DateTime.Now,
                IsRead = false
            };

            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(x => x.Map(dto, entity)).Returns(entity);
            _crudBaseRepositoryMock.Setup(x => x.UpdateAsync(entity)).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, dto);

            Assert.AreEqual(1, result.UserNotificationId);
            Assert.AreEqual(dto.UserId, result.UserId);
            Assert.AreEqual(dto.ArticleId, result.ArticleId);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(1, null));
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenNotificationDoesNotExist()
        {
            var dto = new UserNotificationDto { UserId = 1, ArticleId = 2, SentDateTime = DateTime.Now, IsRead = false };
            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserNotification)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(999, dto));
        }

        #endregion

        #region DeleteAsync Tests

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteNotification_WhenNotificationExists()
        {
            var entity = new UserNotification { UserNotificationId = 1, UserId = 1, ArticleId = 2 };
            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _crudBaseRepositoryMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _crudBaseRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldThrowApiException_WhenNotificationDoesNotExist()
        {
            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserNotification)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.DeleteAsync(999));
        }

        #endregion

        #region GetAllUserNotificationAsync Tests

        [TestMethod]
        public async Task GetAllUserNotificationAsync_ShouldReturnNotifications_WhenNotificationsExist()
        {
            var entities = new List<UserNotification>
            {
                new UserNotification { UserNotificationId = 1, UserId = 1, ArticleId = 1, SentDateTime = DateTime.Now, IsRead = false },
                new UserNotification { UserNotificationId = 2, UserId = 1, ArticleId = 2, SentDateTime = DateTime.Now, IsRead = true }
            };

            var dtos = new List<UserNotificationDto>
            {
                new UserNotificationDto { UserNotificationId = 1, UserId = 1, ArticleId = 1, SentDateTime = DateTime.Now, IsRead = false },
                new UserNotificationDto { UserNotificationId = 2, UserId = 1, ArticleId = 2, SentDateTime = DateTime.Now, IsRead = true }
            };

            _userNotificationRepositoryMock.Setup(x => x.GetAllUserNotificationAsync(1)).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserNotificationDto>>(entities)).Returns(dtos);
            _userNotificationRepositoryMock.Setup(x => x.MarkAllUserNotificationsAsRead(1)).Returns(Task.CompletedTask);

            var result = await _service.GetAllUserNotificationAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.First().UserNotificationId);
            Assert.AreEqual(2, result.Last().UserNotificationId);
        }

        [TestMethod]
        public async Task GetAllUserNotificationAsync_ShouldThrowApiException_WhenNoNotificationsExist()
        {
            _userNotificationRepositoryMock.Setup(x => x.GetAllUserNotificationAsync(1)).ReturnsAsync((IEnumerable<UserNotification>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllUserNotificationAsync());
        }

        [TestMethod]
        public async Task GetAllUserNotificationAsync_ShouldThrowApiException_WhenEmptyNotificationsList()
        {
            _userNotificationRepositoryMock.Setup(x => x.GetAllUserNotificationAsync(1)).ReturnsAsync(new List<UserNotification>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllUserNotificationAsync());
        }

        #endregion
    }
} 