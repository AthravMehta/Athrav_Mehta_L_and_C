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

namespace UnitTests.Services
{
    [TestClass]
    public class UserKeywordServiceTests
    {
        #region Private Fields

        private Mock<IMapper> _mapperMock;
        private Mock<ICrudBaseRepository<UserKeyword>> _userKeywordRepoMock;
        private UserKeywordService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _userKeywordRepoMock = new Mock<ICrudBaseRepository<UserKeyword>>();
            _service = new UserKeywordService(_mapperMock.Object, _userKeywordRepoMock.Object);
        }

        #endregion

        #region AddAsync Tests

        [TestMethod]
        public async Task AddAsync_ShouldReturnUserKeyword_WhenValidDto()
        {
            var dto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 2,
                Keyword = "Technology",
                IsEnabled = true
            };

            var entity = new UserKeyword
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 2,
                Keyword = "Technology",
                IsEnabled = true
            };

            _mapperMock.Setup(x => x.Map<UserKeyword>(dto)).Returns(entity);
            _userKeywordRepoMock.Setup(x => x.AddAsync(entity)).Returns(Task.CompletedTask);
            _userKeywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            Assert.AreEqual(1, result.UserKeywordId);
            Assert.AreEqual(dto.UserId, result.UserId);
            Assert.AreEqual(dto.CategoryId, result.CategoryId);
            Assert.AreEqual(dto.Keyword, result.Keyword);
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
        public async Task UpdateAsync_ShouldReturnUpdatedUserKeyword_WhenUserKeywordExists()
        {
            var dto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 2,
                Keyword = "Updated Technology",
                IsEnabled = false
            };

            var entity = new UserKeyword
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 2,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(x => x.Map(dto, entity)).Returns(entity);
            _userKeywordRepoMock.Setup(x => x.UpdateAsync(entity)).Returns(Task.CompletedTask);
            _userKeywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, dto);

            Assert.AreEqual(1, result.UserKeywordId);
            Assert.AreEqual(dto.UserId, result.UserId);
            Assert.AreEqual(dto.CategoryId, result.CategoryId);
            Assert.AreEqual(dto.Keyword, result.Keyword);
            Assert.AreEqual(dto.IsEnabled, result.IsEnabled);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(1, null));
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenUserKeywordDoesNotExist()
        {
            var dto = new UserKeywordDto { UserId = 1, CategoryId = 2, Keyword = "Technology", IsEnabled = true };
            _userKeywordRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserKeyword)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(999, dto));
        }

        #endregion

        #region DeleteAsync Tests

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteUserKeyword_WhenUserKeywordExists()
        {
            var entity = new UserKeyword { UserKeywordId = 1, UserId = 1, CategoryId = 2, Keyword = "Technology" };
            _userKeywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _userKeywordRepoMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);
            _userKeywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _userKeywordRepoMock.Verify(x => x.DeleteAsync(1), Times.Once);
            _userKeywordRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldThrowApiException_WhenUserKeywordDoesNotExist()
        {
            _userKeywordRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserKeyword)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.DeleteAsync(999));
        }

        #endregion

        #region GetByIdAsync Tests

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUserKeyword_WhenUserKeywordExists()
        {
            var entity = new UserKeyword
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 2,
                Keyword = "Technology",
                IsEnabled = true
            };

            var dto = new UserKeywordDto
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 2,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(x => x.Map<UserKeywordDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.AreEqual(dto, result);
            Assert.AreEqual(1, result.UserKeywordId);
            Assert.AreEqual("Technology", result.Keyword);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldThrowApiException_WhenUserKeywordDoesNotExist()
        {
            _userKeywordRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((UserKeyword)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetByIdAsync(999));
        }

        #endregion

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnUserKeywords_WhenUserKeywordsExist()
        {
            var entities = new List<UserKeyword>
            {
                new UserKeyword { UserKeywordId = 1, UserId = 1, CategoryId = 1, Keyword = "Technology", IsEnabled = true },
                new UserKeyword { UserKeywordId = 2, UserId = 1, CategoryId = 2, Keyword = "Science", IsEnabled = false }
            };

            var dtos = new List<UserKeywordDto>
            {
                new UserKeywordDto { UserKeywordId = 1, UserId = 1, CategoryId = 1, Keyword = "Technology", IsEnabled = true },
                new UserKeywordDto { UserKeywordId = 2, UserId = 1, CategoryId = 2, Keyword = "Science", IsEnabled = false }
            };

            _userKeywordRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserKeywordDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Technology", result.First().Keyword);
            Assert.AreEqual("Science", result.Last().Keyword);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldThrowApiException_WhenNoUserKeywordsExist()
        {
            _userKeywordRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync((IEnumerable<UserKeyword>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllAsync());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldThrowApiException_WhenEmptyUserKeywordsList()
        {
            _userKeywordRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<UserKeyword>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllAsync());
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateService_WhenValidParameters()
        {
            var service = new UserKeywordService(_mapperMock.Object, _userKeywordRepoMock.Object);
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserKeywordService(null, _userKeywordRepoMock.Object));
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserKeywordService(_mapperMock.Object, null));
        }

        #endregion
    }
} 