using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class UserArticleReadTrackingRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _dbContext;
        private UserArticleReadTrackingRepository _repository;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var requestContext = new NewsAggregation.Configurations.RequestContext();
            _dbContext = new NewsAggregationDbContext(options, requestContext);
            _repository = new UserArticleReadTrackingRepository(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region GetCategoriesByUserReadSequenceAsync Tests

        [TestMethod]
        public async Task GetCategoriesByUserReadSequenceAsync_ShouldReturnCategoriesInOrder_WhenReadTrackingsExist()
        {
            var userId = 1;
            _dbContext.UserArticleReadTrackings.AddRange(new List<UserArticleReadTracking>
            {
                new UserArticleReadTracking { UserId = userId, CategoryId = 1, CreatedDateTime = DateTime.UtcNow.AddDays(-3) },
                new UserArticleReadTracking { UserId = userId, CategoryId = 2, CreatedDateTime = DateTime.UtcNow.AddDays(-1) },
                new UserArticleReadTracking { UserId = userId, CategoryId = 1, CreatedDateTime = DateTime.UtcNow.AddDays(-2) },
                new UserArticleReadTracking { UserId = userId, CategoryId = 3, CreatedDateTime = DateTime.UtcNow },
                new UserArticleReadTracking { UserId = 2, CategoryId = 4, CreatedDateTime = DateTime.UtcNow }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetCategoriesByUserReadSequenceAsync(userId);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(3, result[0]); 
            Assert.AreEqual(1, result[1]);
            Assert.AreEqual(2, result[2]); 
        }

        [TestMethod]
        public async Task GetCategoriesByUserReadSequenceAsync_ShouldReturnEmptyList_WhenNoReadTrackingsExist()
        {
            var userId = 1;
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetCategoriesByUserReadSequenceAsync(userId);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetCategoriesByUserReadSequenceAsync_ShouldReturnEmptyList_WhenNoReadTrackingsForUser()
        {
            var userId = 1;
            _dbContext.UserArticleReadTrackings.AddRange(new List<UserArticleReadTracking>
            {
                new UserArticleReadTracking { UserId = 2, CategoryId = 1, CreatedDateTime = DateTime.UtcNow },
                new UserArticleReadTracking { UserId = 3, CategoryId = 2, CreatedDateTime = DateTime.UtcNow }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetCategoriesByUserReadSequenceAsync(userId);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetCategoriesByUserReadSequenceAsync_ShouldReturnDistinctCategories_WhenMultipleReadingsInSameCategory()
        {
            var userId = 1;
            _dbContext.UserArticleReadTrackings.AddRange(new List<UserArticleReadTracking>
            {
                new UserArticleReadTracking { UserId = userId, CategoryId = 1, CreatedDateTime = DateTime.UtcNow.AddDays(-3) },
                new UserArticleReadTracking { UserId = userId, CategoryId = 1, CreatedDateTime = DateTime.UtcNow.AddDays(-1) },
                new UserArticleReadTracking { UserId = userId, CategoryId = 2, CreatedDateTime = DateTime.UtcNow },
                new UserArticleReadTracking { UserId = userId, CategoryId = 1, CreatedDateTime = DateTime.UtcNow.AddDays(-2) }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetCategoriesByUserReadSequenceAsync(userId);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateRepository_WhenValidDbContext()
        {
            var repository = new UserArticleReadTrackingRepository(_dbContext);
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenDbContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserArticleReadTrackingRepository(null));
        }

        #endregion
    }
} 