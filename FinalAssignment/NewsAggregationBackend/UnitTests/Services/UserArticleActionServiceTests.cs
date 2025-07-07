using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Notifications.Contracts;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
using NewsAggregation.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using System.Linq.Expressions;

namespace UnitTests.Services
{
    [TestClass]
    public class UserArticleActionServiceTests
    {
        #region Private Fields

        private Mock<IFileProvider> _fileProviderMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<IUserArticleActionRepository> _userArticleActionRepositoryMock;
        private Mock<INotificationSenderFactory> _notificationSenderFactoryMock;
        private Mock<INotificationSender> _notificationSenderMock;
        private Mock<IBackgroundJobClient> _backgroundJobClientMock;
        private RequestContext _requestContext;
        private UserArticleActionService _service;

        private static class SuccessConstants
        {
            public const string ArticleAlreadyReported = "Article already reported";
            public const string ArticleReported = "Article reported successfully";
        }

        private static class AppConstants
        {
            public const int ReportThreshold = 3;
        }

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _userArticleActionRepositoryMock = new Mock<IUserArticleActionRepository>();
            _notificationSenderFactoryMock = new Mock<INotificationSenderFactory>();
            _notificationSenderMock = new Mock<INotificationSender>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            _requestContext = new RequestContext { UserId = 1, Email = "test@example.com", Roles = new List<string> { "User" } };
            _service = new UserArticleActionService(
                _fileProviderMock.Object,
                _userRepositoryMock.Object,
                _articleRepositoryMock.Object,
                _userArticleActionRepositoryMock.Object,
                _notificationSenderFactoryMock.Object,
                _backgroundJobClientMock.Object,
                _requestContext
            );
        }

        #endregion

        #region ToggleSaveAsync Tests

        [TestMethod]
        public async Task ToggleSaveAsync_ShouldReturnResponse_WhenCalled()
        {
            var response = new ToggleSaveResponseDto { IsSaved = true, Message = "Saved" };
            _userArticleActionRepositoryMock.Setup(x => x.ToggleSaveAsync(1, 2)).ReturnsAsync(response);
            _userArticleActionRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.ToggleSaveAsync(2);
            Assert.AreEqual(response, result);
        }

        #endregion

        #region AddArticleReaction Tests

        [TestMethod]
        public async Task AddArticleReaction_ShouldReturnTrue_WhenReactionAdded()
        {
            var dto = new ArticleReactionRequestDto { ArticleId = 2, ArticleReaction = ReactionEnum.Like };
            _userArticleActionRepositoryMock.Setup(x => x.AddArticleReaction(1, dto)).ReturnsAsync(true);
            _userArticleActionRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.AddArticleReaction(dto);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AddArticleReaction_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.AddArticleReaction(null));
        }

        #endregion

        #region DeleteArticleReaction Tests

        [TestMethod]
        public async Task DeleteArticleReaction_ShouldReturnTrue_WhenReactionDeleted()
        {
            _userArticleActionRepositoryMock.Setup(x => x.DeleteArticleReaction(1, 2)).ReturnsAsync(true);
            _userArticleActionRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeleteArticleReaction(2);
            Assert.IsTrue(result);
        }

        #endregion

        #region GetSavedArticleIdsByUserIdAsync Tests

        [TestMethod]
        public async Task GetSavedArticleIdsByUserIdAsync_ShouldReturnIds()
        {
            var ids = new List<int> { 1, 2, 3 };
            _userArticleActionRepositoryMock.Setup(x => x.GetSavedArticleIdsByUserIdAsync(1)).ReturnsAsync(ids);

            var result = await _service.GetSavedArticleIdsByUserIdAsync();
            CollectionAssert.AreEqual(ids, result.ToList());
        }

        #endregion

        #region GetReportCountForArticleAsync Tests

        [TestMethod]
        public async Task GetReportCountForArticleAsync_ShouldReturnCount()
        {
            _userArticleActionRepositoryMock.Setup(x => x.GetReportCountForArticleAsync(2)).ReturnsAsync(5);
            var result = await _service.GetReportCountForArticleAsync(2);
            Assert.AreEqual(5, result);
        }

        #endregion

        #region ReportArticleAsync Tests

        [TestMethod]
        public async Task ReportArticleAsync_ShouldReturnAlreadyReported_WhenUserAlreadyReported()
        {
            // Arrange
            var userArticleReportDto = new UserArticleReportDto { ArticleId = 1, ReportReason = "Spam" };
            _userArticleActionRepositoryMock.Setup(x => x.HasUserReportedArticleAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _service.ReportArticleAsync(userArticleReportDto);

            // Assert
            Assert.AreEqual("You have already reported this article.", result.Message);
        }

        [TestMethod]
        public async Task ReportArticleAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.ReportArticleAsync(null));
        }

        #endregion
    }
} 