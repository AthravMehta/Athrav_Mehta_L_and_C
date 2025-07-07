using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsAggregation.Configurations;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Repository
{
    [TestClass]
    public class UserNotificationConfigurationRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _dbContext;
        private UserNotificationConfigurationRepository _repository;
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
            _repository = new UserNotificationConfigurationRepository(_dbContext, _requestContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region GetAllUserConfigurationAsync Tests

        [TestMethod]
        public async Task GetAllUserConfigurationAsync_ShouldReturnEmptyEnumerable_WhenUserIdIsNull()
        {
            _requestContext.UserId = null;
            var result = await _repository.GetAllUserConfigurationAsync();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllUserConfigurationAsync_ShouldReturnEmptyEnumerable_WhenNoConfigurationsExist()
        {
            var userId = 1;
            _requestContext.UserId = userId;
            var configurations = new List<UserNotificationConfiguration>();
            _dbContext.UserNotificationConfigurations.AddRange(configurations);
            _dbContext.SaveChanges();
            var result = await _repository.GetAllUserConfigurationAsync();
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region ExistsAsync Tests

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnTrue_WhenConfigurationExists()
        {
            var userId = 1;
            var categoryId = 2;
            var configurations = new List<UserNotificationConfiguration>
            {
                new UserNotificationConfiguration { UserId = userId, CategoryId = categoryId, IsEnabled = true },
                new UserNotificationConfiguration { UserId = 2, CategoryId = 3, IsEnabled = true }
            };
            _dbContext.UserNotificationConfigurations.AddRange(configurations);
            _dbContext.SaveChanges();
            var result = await _repository.ExistsAsync(userId, categoryId);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnFalse_WhenConfigurationDoesNotExist()
        {
            var userId = 1;
            var categoryId = 2;
            var configurations = new List<UserNotificationConfiguration>
            {
                new UserNotificationConfiguration { UserId = 2, CategoryId = 3, IsEnabled = true },
                new UserNotificationConfiguration { UserId = 1, CategoryId = 4, IsEnabled = true }
            };
            _dbContext.UserNotificationConfigurations.AddRange(configurations);
            _dbContext.SaveChanges();
            var result = await _repository.ExistsAsync(userId, categoryId);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnFalse_WhenNoConfigurationsExist()
        {
            var userId = 1;
            var categoryId = 2;
            var configurations = new List<UserNotificationConfiguration>();
            _dbContext.UserNotificationConfigurations.AddRange(configurations);
            _dbContext.SaveChanges();
            var result = await _repository.ExistsAsync(userId, categoryId);
            Assert.IsFalse(result);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateRepository_WhenValidParameters()
        {
            var repository = new UserNotificationConfigurationRepository(_dbContext, _requestContext);
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserNotificationConfigurationRepository(null, _requestContext));
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenRequestContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserNotificationConfigurationRepository(_dbContext, null));
        }

        #endregion
    }
} 