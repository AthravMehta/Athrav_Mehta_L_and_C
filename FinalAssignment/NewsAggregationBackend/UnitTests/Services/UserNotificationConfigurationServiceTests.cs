using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using NewsAggregation.Enums;

namespace UnitTests.Services
{
    [TestClass]
    public class UserNotificationConfigurationServiceTests
    {
        #region Private Fields

        private Mock<IMapper> _mapperMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ICrudBaseRepository<Category>> _categoryRepositoryMock;
        private Mock<ICrudBaseRepository<UserNotificationConfiguration>> _crudBaseRepositoryMock;
        private Mock<IUserNotificationConfigurationRepository> _userNotificationConfigurationRepositoryMock;
        private UserNotificationConfigurationService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _categoryRepositoryMock = new Mock<ICrudBaseRepository<Category>>();
            _crudBaseRepositoryMock = new Mock<ICrudBaseRepository<UserNotificationConfiguration>>();
            _userNotificationConfigurationRepositoryMock = new Mock<IUserNotificationConfigurationRepository>();
            _service = new UserNotificationConfigurationService(
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _crudBaseRepositoryMock.Object,
                _userNotificationConfigurationRepositoryMock.Object
            );
        }

        #endregion

        #region AddAsync Tests

        [TestMethod]
        public async Task AddAsync_ShouldReturnConfiguration_WhenValidDto()
        {
            var dto = new UserNotificationConfigurationDto
            {
                UserId = 1,
                CategoryId = 2,
                IsEnabled = true
            };

            _crudBaseRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserNotificationConfiguration>())).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            Assert.AreEqual(dto.UserId, result.UserId);
            Assert.AreEqual(dto.CategoryId, result.CategoryId);
            Assert.AreEqual(dto.IsEnabled, result.IsEnabled);
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.AddAsync(null));
        }

        #endregion

        #region UpdateAsync Tests

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedConfiguration_WhenConfigurationExists()
        {
            var dto = new UserNotificationConfigurationDto
            {
                UserId = 1,
                CategoryId = 2,
                IsEnabled = false
            };

            var entity = new UserNotificationConfiguration
            {
                UserNotificationConfigurationId = 1,
                UserId = 1,
                CategoryId = 2,
                IsEnabled = true
            };

            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _crudBaseRepositoryMock.Setup(x => x.UpdateAsync(entity)).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, dto);

            Assert.AreEqual(dto.UserId, result.UserId);
            Assert.AreEqual(dto.IsEnabled, result.IsEnabled);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(1, null));
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenConfigurationDoesNotExist()
        {
            var dto = new UserNotificationConfigurationDto { UserId = 1, CategoryId = 2, IsEnabled = true };
            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserNotificationConfiguration)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(999, dto));
        }

        #endregion

        #region DeleteAsync Tests

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteConfiguration_WhenConfigurationExists()
        {
            var entity = new UserNotificationConfiguration { UserNotificationConfigurationId = 1, UserId = 1, CategoryId = 2 };
            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _crudBaseRepositoryMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _crudBaseRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldThrowApiException_WhenConfigurationDoesNotExist()
        {
            _crudBaseRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserNotificationConfiguration)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.DeleteAsync(999));
        }

        #endregion

        #region GetAllConfigurationAsync Tests

        [TestMethod]
        public async Task GetAllConfigurationAsync_ShouldReturnConfigurations_WhenConfigurationsExist()
        {
            var entities = new List<UserNotificationConfiguration>
            {
                new UserNotificationConfiguration { UserNotificationConfigurationId = 1, UserId = 1, CategoryId = 1, IsEnabled = true },
                new UserNotificationConfiguration { UserNotificationConfigurationId = 2, UserId = 1, CategoryId = 2, IsEnabled = false }
            };

            var dtos = new List<UserNotificationConfigurationDto>
            {
                new UserNotificationConfigurationDto { UserNotificationConfigurationId = 1, UserId = 1, CategoryId = 1, IsEnabled = true },
                new UserNotificationConfigurationDto { UserNotificationConfigurationId = 2, UserId = 1, CategoryId = 2, IsEnabled = false }
            };

            _crudBaseRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserNotificationConfigurationDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllConfigurationAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.First().UserNotificationConfigurationId);
            Assert.AreEqual(2, result.Last().UserNotificationConfigurationId);
        }

        [TestMethod]
        public async Task GetAllConfigurationAsync_ShouldThrowApiException_WhenNoConfigurationsExist()
        {
            _crudBaseRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync((IEnumerable<UserNotificationConfiguration>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllConfigurationAsync());
        }

        [TestMethod]
        public async Task GetAllConfigurationAsync_ShouldThrowApiException_WhenEmptyConfigurationsList()
        {
            _crudBaseRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<UserNotificationConfiguration>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllConfigurationAsync());
        }

        #endregion

        #region GetAllUserConfigurationAsync Tests

        [TestMethod]
        public async Task GetAllUserConfigurationAsync_ShouldReturnConfigurations_WhenConfigurationsExist()
        {
            var entities = new List<UserNotificationConfiguration>
            {
                new UserNotificationConfiguration { UserNotificationConfigurationId = 1, UserId = 1, CategoryId = 1, IsEnabled = true },
                new UserNotificationConfiguration { UserNotificationConfigurationId = 2, UserId = 1, CategoryId = 2, IsEnabled = false }
            };

            var dtos = new List<UserNotificationConfigurationDto>
            {
                new UserNotificationConfigurationDto { UserNotificationConfigurationId = 1, UserId = 1, CategoryId = 1, IsEnabled = true },
                new UserNotificationConfigurationDto { UserNotificationConfigurationId = 2, UserId = 1, CategoryId = 2, IsEnabled = false }
            };

            _userNotificationConfigurationRepositoryMock.Setup(x => x.GetAllUserConfigurationAsync()).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserNotificationConfigurationDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllUserConfigurationAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.First().UserNotificationConfigurationId);
            Assert.AreEqual(2, result.Last().UserNotificationConfigurationId);
        }

        [TestMethod]
        public async Task GetAllUserConfigurationAsync_ShouldThrowApiException_WhenNoConfigurationsExist()
        {
            _userNotificationConfigurationRepositoryMock.Setup(x => x.GetAllUserConfigurationAsync()).ReturnsAsync((IEnumerable<UserNotificationConfiguration>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllUserConfigurationAsync());
        }

        [TestMethod]
        public async Task GetAllUserConfigurationAsync_ShouldThrowApiException_WhenEmptyConfigurationsList()
        {
            _userNotificationConfigurationRepositoryMock.Setup(x => x.GetAllUserConfigurationAsync()).ReturnsAsync(new List<UserNotificationConfiguration>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllUserConfigurationAsync());
        }

        #endregion

        #region ExistsAsync Tests

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnTrue_WhenConfigurationExists()
        {
            _userNotificationConfigurationRepositoryMock.Setup(x => x.ExistsAsync(1, 2)).ReturnsAsync(true);

            var result = await _service.ExistsAsync(1, 2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnFalse_WhenConfigurationDoesNotExist()
        {
            _userNotificationConfigurationRepositoryMock.Setup(x => x.ExistsAsync(1, 2)).ReturnsAsync(false);

            var result = await _service.ExistsAsync(1, 2);

            Assert.IsFalse(result);
        }

        #endregion

        #region SaveChangesAsync Tests

        [TestMethod]
        public async Task SaveChangesAsync_ShouldCallRepository()
        {
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.SaveChangesAsync();

            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region CreateNotificationConfigForAllUsersAsync Tests

        [TestMethod]
        public async Task CreateNotificationConfigForAllUsersAsync_ShouldCreateConfigsForNewUser_WhenNewUserProvided()
        {
            var newUser = new User { UserId = 1, Username = "newuser" };
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Technology" },
                new Category { CategoryId = 2, Name = "Science" }
            };

            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            _userNotificationConfigurationRepositoryMock.Setup(x => x.ExistsAsync(1, 1)).ReturnsAsync(false);
            _userNotificationConfigurationRepositoryMock.Setup(x => x.ExistsAsync(1, 2)).ReturnsAsync(false);
            _crudBaseRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserNotificationConfiguration>())).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.CreateNotificationConfigForAllUsersAsync(null, newUser);

            _crudBaseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserNotificationConfiguration>()), Times.Exactly(2));
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task CreateNotificationConfigForAllUsersAsync_ShouldThrowApiException_WhenNoCategoriesExist()
        {
            var newUser = new User { UserId = 1, Username = "newuser" };
            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync((IEnumerable<Category>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateNotificationConfigForAllUsersAsync(null, newUser));
        }

        [TestMethod]
        public async Task CreateNotificationConfigForAllUsersAsync_ShouldThrowApiException_WhenNoUsersExist()
        {
            var categoryId = 3;
            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(It.IsAny<RoleEnum>())).ReturnsAsync((List<User>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.CreateNotificationConfigForAllUsersAsync(categoryId, null));
        }

        #endregion

        #region InitializeNotificationConfigurationsAsync Tests

        [TestMethod]
        public async Task InitializeNotificationConfigurationsAsync_ShouldCreateConfigsForAllUsersAndCategories()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1" },
                new User { UserId = 2, Username = "user2" }
            };

            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Technology" },
                new Category { CategoryId = 2, Name = "Science" }
            };

            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(It.IsAny<RoleEnum>())).ReturnsAsync(users);
            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            _userNotificationConfigurationRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _crudBaseRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserNotificationConfiguration>())).Returns(Task.CompletedTask);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.InitializeNotificationConfigurationsAsync();

            _crudBaseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserNotificationConfiguration>()), Times.Exactly(4));
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task InitializeNotificationConfigurationsAsync_ShouldThrowApiException_WhenNoUsersExist()
        {
            var categories = new List<Category> { new Category { CategoryId = 1, Name = "Technology" } };
            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(It.IsAny<RoleEnum>())).ReturnsAsync((List<User>)null);
            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.InitializeNotificationConfigurationsAsync());
        }

        [TestMethod]
        public async Task InitializeNotificationConfigurationsAsync_ShouldThrowApiException_WhenNoCategoriesExist()
        {
            var users = new List<User> { new User { UserId = 1, Username = "user1" } };
            _userRepositoryMock.Setup(x => x.GetAllUsersAsync(It.IsAny<RoleEnum>())).ReturnsAsync(users);
            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync((IEnumerable<Category>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.InitializeNotificationConfigurationsAsync());
        }

        #endregion
    }
} 