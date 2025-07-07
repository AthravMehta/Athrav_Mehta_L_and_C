using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
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
    public class KeywordServiceTests
    {
        #region Private Fields

        private Mock<IArticleService> _articleServiceMock;
        private Mock<ICrudBaseRepository<Keywords>> _keywordRepoMock;
        private KeywordService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _articleServiceMock = new Mock<IArticleService>();
            _keywordRepoMock = new Mock<ICrudBaseRepository<Keywords>>();
            _service = new KeywordService(_articleServiceMock.Object, _keywordRepoMock.Object);
        }

        #endregion

        #region GetAllKeywordsAsync Tests

        [TestMethod]
        public async Task GetAllKeywordsAsync_ShouldReturnKeywordsList_WhenKeywordsExist()
        {
            var keywords = new List<Keywords>
            {
                new Keywords { KeywordId = 1, Keyword = "Technology", IsHidden = false },
                new Keywords { KeywordId = 2, Keyword = "Science", IsHidden = false },
                new Keywords { KeywordId = 3, Keyword = "Sports", IsHidden = true }
            };

            _keywordRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(keywords);

            var result = await _service.GetAllKeywordsAsync();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Technology", result[0].Keyword);
            Assert.AreEqual("Science", result[1].Keyword);
            Assert.AreEqual("Sports", result[2].Keyword);
        }

        [TestMethod]
        public async Task GetAllKeywordsAsync_ShouldThrowApiException_WhenNoKeywordsExist()
        {
            _keywordRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync((IEnumerable<Keywords>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllKeywordsAsync());
        }

        [TestMethod]
        public async Task GetAllKeywordsAsync_ShouldThrowApiException_WhenEmptyKeywordsList()
        {
            _keywordRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Keywords>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllKeywordsAsync());
        }

        #endregion

        #region HideKeywordAsync Tests

        [TestMethod]
        public async Task HideKeywordAsync_ShouldHideKeywordAndReturnTrue_WhenKeywordExists()
        {
            var keyword = new Keywords
            {
                KeywordId = 1,
                Keyword = "Technology",
                IsHidden = false,
                HideReason = null
            };

            _keywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(keyword);
            _keywordRepoMock.Setup(x => x.UpdateAsync(keyword)).Returns(Task.CompletedTask);
            _keywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _articleServiceMock.Setup(x => x.HideArticlesByKeywordAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.HideKeywordAsync(1, "Inappropriate content");

            Assert.IsTrue(result);
            Assert.IsTrue(keyword.IsHidden);
            Assert.AreEqual("Inappropriate content", keyword.HideReason);
            _keywordRepoMock.Verify(x => x.UpdateAsync(keyword), Times.Once);
            _keywordRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _articleServiceMock.Verify(x => x.HideArticlesByKeywordAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task HideKeywordAsync_ShouldThrowApiException_WhenKeywordDoesNotExist()
        {
            _keywordRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Keywords)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.HideKeywordAsync(999, "Test reason"));
        }

        [TestMethod]
        public async Task HideKeywordAsync_ShouldHideKeyword_WhenKeywordIsAlreadyHidden()
        {
            var keyword = new Keywords
            {
                KeywordId = 1,
                Keyword = "Technology",
                IsHidden = true,
                HideReason = "Old reason"
            };

            _keywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(keyword);
            _keywordRepoMock.Setup(x => x.UpdateAsync(keyword)).Returns(Task.CompletedTask);
            _keywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _articleServiceMock.Setup(x => x.HideArticlesByKeywordAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.HideKeywordAsync(1, "New reason");

            Assert.IsTrue(result);
            Assert.IsTrue(keyword.IsHidden);
            Assert.AreEqual("New reason", keyword.HideReason);
        }

        #endregion

        #region UnhideKeywordAsync Tests

        [TestMethod]
        public async Task UnhideKeywordAsync_ShouldUnhideKeywordAndReturnTrue_WhenKeywordExists()
        {
            var keyword = new Keywords
            {
                KeywordId = 1,
                Keyword = "Technology",
                IsHidden = true,
                HideReason = "Inappropriate content"
            };

            _keywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(keyword);
            _keywordRepoMock.Setup(x => x.UpdateAsync(keyword)).Returns(Task.CompletedTask);
            _keywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _articleServiceMock.Setup(x => x.UnhideArticlesByKeywordAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.UnhideKeywordAsync(1);

            Assert.IsTrue(result);
            Assert.IsFalse(keyword.IsHidden);
            Assert.IsNull(keyword.HideReason);
            _keywordRepoMock.Verify(x => x.UpdateAsync(keyword), Times.Once);
            _keywordRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _articleServiceMock.Verify(x => x.UnhideArticlesByKeywordAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task UnhideKeywordAsync_ShouldThrowApiException_WhenKeywordDoesNotExist()
        {
            _keywordRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Keywords)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UnhideKeywordAsync(999));
        }

        [TestMethod]
        public async Task UnhideKeywordAsync_ShouldUnhideKeyword_WhenKeywordIsAlreadyVisible()
        {
            var keyword = new Keywords
            {
                KeywordId = 1,
                Keyword = "Technology",
                IsHidden = false,
                HideReason = null
            };

            _keywordRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(keyword);
            _keywordRepoMock.Setup(x => x.UpdateAsync(keyword)).Returns(Task.CompletedTask);
            _keywordRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _articleServiceMock.Setup(x => x.UnhideArticlesByKeywordAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.UnhideKeywordAsync(1);

            Assert.IsTrue(result);
            Assert.IsFalse(keyword.IsHidden);
            Assert.IsNull(keyword.HideReason);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateService_WhenValidParameters()
        {
            var service = new KeywordService(_articleServiceMock.Object, _keywordRepoMock.Object);
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenArticleServiceIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new KeywordService(null, _keywordRepoMock.Object));
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new KeywordService(_articleServiceMock.Object, null));
        }

        #endregion
    }
} 