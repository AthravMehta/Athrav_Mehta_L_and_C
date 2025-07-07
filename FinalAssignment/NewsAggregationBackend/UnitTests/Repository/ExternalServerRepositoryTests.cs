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
    public class ExternalServerRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _dbContext;
        private ExternalServerRepository _repository;

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
            _repository = new ExternalServerRepository(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllServers_WhenNoFilterProvided()
        {
            _dbContext.ExternalServers.AddRange(new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Server 1", IsActive = true, ApiKeyHash = "hash1", BaseUrl = "http://server1.com" },
                new ExternalServer { ExternalServerId = 2, ServerName = "Server 2", IsActive = false, ApiKeyHash = "hash2", BaseUrl = "http://server2.com" },
                new ExternalServer { ExternalServerId = 3, ServerName = "Server 3", IsActive = true, ApiKeyHash = "hash3", BaseUrl = "http://server3.com" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetAllAsync();
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Server 1", result.First().ServerName);
            Assert.AreEqual("Server 3", result.Last().ServerName);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnActiveServers_WhenActiveFilterIsTrue()
        {
            _dbContext.ExternalServers.AddRange(new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Active Server 1", IsActive = true, ApiKeyHash = "hash1", BaseUrl = "http:ive1.com" },
                new ExternalServer { ExternalServerId = 2, ServerName = "Inactive Server", IsActive = false, ApiKeyHash = "hash2", BaseUrl = "http://inactive.com" },
                new ExternalServer { ExternalServerId = 3, ServerName = "Active Server 2", IsActive = true, ApiKeyHash = "hash3", BaseUrl = "http:ive2.com" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetAllAsync(true);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(s => s.IsActive));
            Assert.AreEqual("Active Server 1", result.First().ServerName);
            Assert.AreEqual("Active Server 2", result.Last().ServerName);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnInactiveServers_WhenActiveFilterIsFalse()
        {
            _dbContext.ExternalServers.AddRange(new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Active Server", IsActive = true, ApiKeyHash = "hash1", BaseUrl = "http:ive.com" },
                new ExternalServer { ExternalServerId = 2, ServerName = "Inactive Server 1", IsActive = false, ApiKeyHash = "hash2", BaseUrl = "http://inactive1.com" },
                new ExternalServer { ExternalServerId = 3, ServerName = "Inactive Server 2", IsActive = false, ApiKeyHash = "hash3", BaseUrl = "http://inactive2.com" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetAllAsync(false);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(s => !s.IsActive));
            Assert.AreEqual("Inactive Server 1", result.First().ServerName);
            Assert.AreEqual("Inactive Server 2", result.Last().ServerName);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoServersExist()
        {
            var result = await _repository.GetAllAsync();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoActiveServersExist()
        {
            _dbContext.ExternalServers.AddRange(new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Inactive Server 1", IsActive = false, ApiKeyHash = "hash1", BaseUrl = "http://inactive1.com" },
                new ExternalServer { ExternalServerId = 2, ServerName = "Inactive Server 2", IsActive = false, ApiKeyHash = "hash2", BaseUrl = "http://inactive2.com" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetAllAsync(true);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoInactiveServersExist()
        {
            _dbContext.ExternalServers.AddRange(new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Active Server 1", IsActive = true, ApiKeyHash = "hash1", BaseUrl = "http:ive1.com" },
                new ExternalServer { ExternalServerId = 2, ServerName = "Active Server 2", IsActive = true, ApiKeyHash = "hash2", BaseUrl = "http:ive2.com" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetAllAsync(false);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateRepository_WhenValidDbContext()
        {
            var repository = new ExternalServerRepository(_dbContext);
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenDbContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalServerRepository(null));
        }

        #endregion
    }
} 