using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Configurations;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Repository
{
    [TestClass]
    public class UserRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _dbContext;
        private Mock<DbSet<User>> _userDbSetMock;
        private UserRepository _repository;
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
            _userDbSetMock = new Mock<DbSet<User>>();
            _repository = new UserRepository(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region GetUserByName Tests

        [TestMethod]
        public async Task GetUserByName_ShouldReturnUser_WhenUserExists()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "testuser", Email = "test@example.com", RoleId = RoleEnum.User, PasswordHash = "hash1" },
                new User { UserId = 2, Username = "adminuser", Email = "admin@example.com", RoleId = RoleEnum.Admin, PasswordHash = "hash2" }
            }.AsQueryable();

            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _dbContext.Set<User>().AddRange(users);
            _dbContext.SaveChanges();

            var result = await _repository.GetUserByName("testuser");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.UserId);
            Assert.AreEqual("testuser", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [TestMethod]
        public async Task GetUserByName_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "testuser", Email = "test@example.com", RoleId = RoleEnum.User, PasswordHash = "hash1" }
            }.AsQueryable();

            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _dbContext.Set<User>().AddRange(users);
            _dbContext.SaveChanges();

            var result = await _repository.GetUserByName("nonexistentuser");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowArgumentNullException_WhenUsernameIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _repository.GetUserByName(null));
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowArgumentNullException_WhenUsernameIsEmpty()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _repository.GetUserByName(""));
        }

        [TestMethod]
        public async Task GetUserByName_ShouldThrowArgumentNullException_WhenUsernameIsWhitespace()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _repository.GetUserByName("   "));
        }

        #endregion

        #region GetAllUsersAsync Tests

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExistWithDefaultRole()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", RoleId = RoleEnum.User, PasswordHash = "hash1" },
                new User { UserId = 2, Username = "user2", Email = "user2@example.com", RoleId = RoleEnum.User, PasswordHash = "hash2" },
                new User { UserId = 3, Username = "admin1", Email = "admin1@example.com", RoleId = RoleEnum.Admin, PasswordHash = "hash3" }
            };

            _dbContext.Set<User>().AddRange(users);
            _dbContext.SaveChanges();

            var result = await _repository.GetAllUsersAsync();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(u => u.RoleId == RoleEnum.User));
            Assert.AreEqual("user1", result[0].Username);
            Assert.AreEqual("user2", result[1].Username);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExistWithSpecificRole()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", RoleId = RoleEnum.User, PasswordHash = "hash1" },
                new User { UserId = 2, Username = "admin1", Email = "admin1@example.com", RoleId = RoleEnum.Admin, PasswordHash = "hash2" },
                new User { UserId = 3, Username = "admin2", Email = "admin2@example.com", RoleId = RoleEnum.Admin, PasswordHash = "hash3" }
            };

            _dbContext.Set<User>().AddRange(users);
            _dbContext.SaveChanges();

            var result = await _repository.GetAllUsersAsync(RoleEnum.Admin);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(u => u.RoleId == RoleEnum.Admin));
            Assert.AreEqual("admin1", result[0].Username);
            Assert.AreEqual("admin2", result[1].Username);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExistWithRole()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Email = "user1@example.com", RoleId = RoleEnum.User, PasswordHash = "hash1" }
            };

            _dbContext.Set<User>().AddRange(users);
            _dbContext.SaveChanges();

            var result = await _repository.GetAllUsersAsync(RoleEnum.Admin);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            var users = new List<User>();

            _dbContext.Set<User>().AddRange(users);
            _dbContext.SaveChanges();

            var result = await _repository.GetAllUsersAsync();

            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateRepository_WhenValidDbContext()
        {
            var repository = new UserRepository(_dbContext);
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenDbContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserRepository(null));
        }

        #endregion
    }
} 