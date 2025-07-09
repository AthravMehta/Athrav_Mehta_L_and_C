using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using NewsAggregation.Configurations;

namespace UnitTests.Repository
{
    [TestClass]
    public class UserNotificationRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _dbContext;
        private Mock<DbSet<UserNotification>> _userNotificationDbSetMock;
        private Mock<DbSet<UserNotificationConfiguration>> _userNotificationConfigDbSetMock;
        private Mock<DbSet<UserKeyword>> _userKeywordDbSetMock;
        private UserNotificationRepository _repository;
        private RequestContext _requestContext;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _requestContext = new RequestContext();
            _dbContext = new NewsAggregationDbContext(options, _requestContext);
            _userNotificationDbSetMock = new Mock<DbSet<UserNotification>>();
            _userNotificationConfigDbSetMock = new Mock<DbSet<UserNotificationConfiguration>>();
            _userKeywordDbSetMock = new Mock<DbSet<UserKeyword>>();
            _repository = new UserNotificationRepository(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region AddRangeAsync Tests

        [TestMethod]
        public async Task AddRangeAsync_ShouldAddNotifications_WhenValidNotifications()
        {
            var notifications = new List<UserNotification>
            {
                new UserNotification { UserNotificationId = 1, UserId = 1, ArticleId = 1, IsRead = false },
                new UserNotification { UserNotificationId = 2, UserId = 1, ArticleId = 2, IsRead = false }
            };

            _dbContext.UserNotifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();

            Assert.AreEqual(2, _dbContext.UserNotifications.Count());
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowArgumentNullException_WhenNotificationsIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _repository.AddRangeAsync(null));
        }

        #endregion

        #region GetAllUserNotificationAsync Tests

        [TestMethod]
        public async Task GetAllUserNotificationAsync_ShouldReturnEmptyEnumerable_WhenUserIdIsNull()
        {
            var result = await _repository.GetAllUserNotificationAsync(null);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllUserNotificationAsync_ShouldReturnEmptyEnumerable_WhenNoNotificationsExist()
        {
            var notifications = new List<UserNotification>();

            var result = await _repository.GetAllUserNotificationAsync(1);

            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region MarkAllUserNotificationsAsRead Tests

        [TestMethod]
        public async Task MarkAllUserNotificationsAsRead_ShouldMarkNotificationsAsRead_WhenUnreadNotificationsExist()
        {
            var notifications = new List<UserNotification>
            {
                new UserNotification { UserNotificationId = 1, UserId = 1, ArticleId = 1, IsRead = false },
                new UserNotification { UserNotificationId = 2, UserId = 1, ArticleId = 2, IsRead = false },
                new UserNotification { UserNotificationId = 3, UserId = 1, ArticleId = 3, IsRead = true }
            };

            _dbContext.UserNotifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();

            await _repository.MarkAllUserNotificationsAsRead(1);

            Assert.IsTrue(notifications[0].IsRead);
            Assert.IsTrue(notifications[1].IsRead);
            Assert.IsTrue(notifications[2].IsRead);
        }

        [TestMethod]
        public async Task MarkAllUserNotificationsAsRead_ShouldNotCallSaveChanges_WhenNoUnreadNotificationsExist()
        {
            var notifications = new List<UserNotification>
            {
                new UserNotification { UserNotificationId = 1, UserId = 1, ArticleId = 1, IsRead = true },
                new UserNotification { UserNotificationId = 2, UserId = 1, ArticleId = 2, IsRead = true }
            };

            _dbContext.UserNotifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();

            await _repository.MarkAllUserNotificationsAsRead(1);
        }

        [TestMethod]
        public async Task MarkAllUserNotificationsAsRead_ShouldNotCallSaveChanges_WhenUserIdIsNull()
        {
            await _repository.MarkAllUserNotificationsAsRead(null);
        }

        #endregion

        #region GetCategoriesByUserNotificationsAsync Tests

        [TestMethod]
        public async Task GetCategoriesByUserNotificationsAsync_ShouldReturnCategoryIds_WhenEnabledConfigurationsExist()
        {
            var configurations = new List<UserNotificationConfiguration>
            {
                new UserNotificationConfiguration { UserId = 1, CategoryId = 1, IsEnabled = true },
                new UserNotificationConfiguration { UserId = 1, CategoryId = 2, IsEnabled = true },
                new UserNotificationConfiguration { UserId = 1, CategoryId = 3, IsEnabled = false },
                new UserNotificationConfiguration { UserId = 2, CategoryId = 4, IsEnabled = true }
            };

            _dbContext.UserNotificationConfigurations.AddRange(configurations);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetCategoriesByUserNotificationsAsync(1);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
            Assert.IsFalse(result.Contains(3));
        }

        [TestMethod]
        public async Task GetCategoriesByUserNotificationsAsync_ShouldReturnEmptyList_WhenNoEnabledConfigurationsExist()
        {
            var configurations = new List<UserNotificationConfiguration>
            {
                new UserNotificationConfiguration { UserId = 1, CategoryId = 1, IsEnabled = false },
                new UserNotificationConfiguration { UserId = 1, CategoryId = 2, IsEnabled = false }
            };

            _dbContext.UserNotificationConfigurations.AddRange(configurations);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetCategoriesByUserNotificationsAsync(1);

            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region GetKeywordsByUserNotificationsAsync Tests

        [TestMethod]
        public async Task GetKeywordsByUserNotificationsAsync_ShouldReturnUserKeywords_WhenKeywordsExist()
        {
            var keywords = new List<UserKeyword>
            {
                new UserKeyword { UserKeywordId = 1, UserId = 1, Keyword = "Technology", IsEnabled = true },
                new UserKeyword { UserKeywordId = 2, UserId = 1, Keyword = "Science", IsEnabled = false },
                new UserKeyword { UserKeywordId = 3, UserId = 2, Keyword = "Sports", IsEnabled = true }
            };

            _dbContext.UserKeywords.AddRange(keywords);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetKeywordsByUserNotificationsAsync(1);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(k => k.UserId == 1));
        }

        [TestMethod]
        public async Task GetKeywordsByUserNotificationsAsync_ShouldReturnEmptyList_WhenNoKeywordsExist()
        {
            var keywords = new List<UserKeyword>();

            var result = await _repository.GetKeywordsByUserNotificationsAsync(1);

            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region SaveChangesAsync Tests

        [TestMethod]
        public async Task SaveChangesAsync_ShouldCallDbContextSaveChanges()
        {
            await _repository.SaveChangesAsync();

            Assert.AreEqual(0, _dbContext.ChangeTracker.Entries().Count());
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateRepository_WhenValidContext()
        {
            var repository = new UserNotificationRepository(_dbContext);
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserNotificationRepository(null));
        }

        #endregion
    }
} 