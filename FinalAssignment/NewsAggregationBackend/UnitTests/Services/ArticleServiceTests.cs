using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Models;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;

namespace UnitTests.Services
{
    [TestClass]
    public class ArticleServiceTests
    {
        #region Private Fields

        private Mock<IUserArticleActionService> _userArticleActionServiceMock;
        private Mock<ICrudBaseRepository<Article>> _crudBaseRepositoryMock;
        private Mock<ICrudBaseRepository<Keywords>> _keywordsRepositoryMock;
        private Mock<ICrudBaseRepository<UserArticleReadTracking>> _userArticleReadTrackingRepositoryMock;
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<IUserArticleActionRepository> _userArticleActionRepositoryMock;
        private Mock<IUserArticleReadTrackingRepository> _customUserArticleReadTrackingRepositoryMock;
        private Mock<IUserNotificationRepository> _userNotificationRepositoryMock;
        private Mock<ILogger<ArticleService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private ArticleService _articleService;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _userArticleActionServiceMock = new Mock<IUserArticleActionService>();
            _crudBaseRepositoryMock = new Mock<ICrudBaseRepository<Article>>();
            _keywordsRepositoryMock = new Mock<ICrudBaseRepository<Keywords>>();
            _userArticleReadTrackingRepositoryMock = new Mock<ICrudBaseRepository<UserArticleReadTracking>>();
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _userArticleActionRepositoryMock = new Mock<IUserArticleActionRepository>();
            _customUserArticleReadTrackingRepositoryMock = new Mock<IUserArticleReadTrackingRepository>();
            _userNotificationRepositoryMock = new Mock<IUserNotificationRepository>();
            _loggerMock = new Mock<ILogger<ArticleService>>();
            _mapperMock = new Mock<IMapper>();

            // Create a real RequestContext instance instead of mocking it
            var requestContext = new RequestContext
            {
                UserId = 1,
                Email = "test@example.com",
                Roles = new List<string> { "User" }
            };

            _articleService = new ArticleService(
                _userArticleActionServiceMock.Object,
                _crudBaseRepositoryMock.Object,
                _keywordsRepositoryMock.Object,
                _userArticleReadTrackingRepositoryMock.Object,
                _customUserArticleReadTrackingRepositoryMock.Object,
                _articleRepositoryMock.Object,
                _userArticleActionRepositoryMock.Object,
                _userNotificationRepositoryMock.Object,
                _loggerMock.Object,
                requestContext,
                _mapperMock.Object);
        }

        #endregion

        #region AddAsync Tests

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenArticleDtoIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _articleService.AddAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAsync_ShouldReturnArticleDto_WhenArticleIsValid()
        {
            var articleDto = new ArticleDto { Title = "Test Article", Content = "Test Content" };
            var article = new Article { ArticleId = 1, Title = "Test Article", Content = "Test Content" };

            _mapperMock.Setup(x => x.Map<Article>(articleDto)).Returns(article);
            _crudBaseRepositoryMock.Setup(x => x.AddAsync(article)).Returns(Task.FromResult<object>(null));
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));

            var result = await _articleService.AddAsync(articleDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ArticleId);
            Assert.AreEqual("Test Article", result.Title);
            _crudBaseRepositoryMock.Verify(x => x.AddAsync(article), Times.Once);
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region AddAllArticlesAsync Tests

        [TestMethod]
        public async Task AddAllArticlesAsync_ShouldThrowApiException_WhenArticlesIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _articleService.AddAllArticlesAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAllArticlesAsync_ShouldReturnEmptyEnumerable_WhenAllArticlesExist()
        {
            var articles = new List<Article>
            {
                new Article { Title = "Article 1", Content = "Content 1" },
                new Article { Title = "Article 2", Content = "Content 2" }
            };

            _articleRepositoryMock.Setup(x => x.ArticleExistsAsync(It.IsAny<Article>())).ReturnsAsync(true);

            var result = await _articleService.AddAllArticlesAsync(articles);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task AddAllArticlesAsync_ShouldReturnNewArticles_WhenSomeArticlesDontExist()
        {
            var articles = new List<Article>
            {
                new Article { Title = "Article 1", Content = "Content 1" },
                new Article { Title = "Article 2", Content = "Content 2" }
            };

            _articleRepositoryMock.Setup(x => x.ArticleExistsAsync(articles[0])).ReturnsAsync(true);
            _articleRepositoryMock.Setup(x => x.ArticleExistsAsync(articles[1])).ReturnsAsync(false);
            _articleRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Article>>())).Returns(Task.FromResult<object>(null));
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));

            var result = await _articleService.AddAllArticlesAsync(articles);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            _articleRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Article>>()), Times.Once);
        }

        #endregion

        #region GetByIdWithUserDetailsAsync Tests

        [TestMethod]
        public async Task GetByIdWithUserDetailsAsync_ShouldThrowApiException_WhenArticleNotFound()
        {
            _articleRepositoryMock.Setup(x => x.GetArticleWithUserStatusAsync(1)).ReturnsAsync((ArticleDetailsDto)null);

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _articleService.GetByIdWithUserDetailsAsync(1));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task GetByIdWithUserDetailsAsync_ShouldReturnArticleDetails_WhenArticleExists()
        {
            var articleDetails = new ArticleDetailsDto { ArticleId = 1, Title = "Test Article" };
            _articleRepositoryMock.Setup(x => x.GetArticleWithUserStatusAsync(1)).ReturnsAsync(articleDetails);
            _userArticleReadTrackingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserArticleReadTracking>())).Returns(Task.FromResult<object>(null));
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));

            var result = await _articleService.GetByIdWithUserDetailsAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ArticleId);
            Assert.AreEqual("Test Article", result.Title);
            _userArticleReadTrackingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserArticleReadTracking>()), Times.Once);
        }

        #endregion

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_ShouldThrowApiException_WhenQueryIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _articleService.GetAllAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnArticles_WhenArticlesExist()
        {
            var query = new ArticleQueryDto { CategoryId = 1 };
            var articles = new List<Article>
            {
                new Article { ArticleId = 1, Title = "Article 1" },
                new Article { ArticleId = 2, Title = "Article 2" }
            };
            var articleDtos = new List<ArticleDto>
            {
                new ArticleDto { ArticleId = 1, Title = "Article 1" },
                new ArticleDto { ArticleId = 2, Title = "Article 2" }
            };

            _articleRepositoryMock.Setup(x => x.GetAllAsync(query)).ReturnsAsync(articles);
            _mapperMock.Setup(x => x.Map<IEnumerable<ArticleDto>>(articles)).Returns(articleDtos);

            var result = await _articleService.GetAllAsync(query);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        #endregion

        #region GetRecommendedArticlesAsync Tests

        [TestMethod]
        public async Task GetRecommendedArticlesAsync_ShouldReturnEmptyList_WhenNoSuitableCategoryFound()
        {
            _userArticleActionRepositoryMock.Setup(x => x.GetCategoriesByUserReactionAsync(1, ReactionEnum.Like)).ReturnsAsync(new List<int>());
            _userArticleActionRepositoryMock.Setup(x => x.GetCategoriesByUserReactionAsync(1, ReactionEnum.Dislike)).ReturnsAsync(new List<int>());
            _userArticleActionRepositoryMock.Setup(x => x.GetCategoriesBySavedUserArticleAsync(1)).ReturnsAsync(new List<int>());
            _customUserArticleReadTrackingRepositoryMock.Setup(x => x.GetCategoriesByUserReadSequenceAsync(1)).ReturnsAsync(new List<int>());
            _userNotificationRepositoryMock.Setup(x => x.GetCategoriesByUserNotificationsAsync(1)).ReturnsAsync(new List<int>());
            _userNotificationRepositoryMock.Setup(x => x.GetKeywordsByUserNotificationsAsync(1)).ReturnsAsync(new List<UserKeyword>());
            _userArticleActionRepositoryMock.Setup(x => x.GetReportedArticleCategoriesByUserAsync(1)).ReturnsAsync(new List<int>());

            var result = await _articleService.GetRecommendedArticlesAsync(20);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetRecommendedArticlesAsync_ShouldReturnRecommendedArticles_WhenSuitableCategoryFound()
        {
            var likedCategories = new List<int> { 1, 2 };
            var dislikedCategories = new List<int> { 3 };
            var savedCategories = new List<int> { 1 };
            var readCategories = new List<int> { 1, 2 };
            var notificationCategories = new List<int> { 1 };
            var notificationKeywords = new List<UserKeyword> { new UserKeyword { 
                UserKeywordId = 1,
                Keyword = "New Keyword"
            } };
            var reportedCategories = new List<int> { 4 };

            _userArticleActionRepositoryMock.Setup(x => x.GetCategoriesByUserReactionAsync(1, ReactionEnum.Like)).ReturnsAsync(likedCategories);
            _userArticleActionRepositoryMock.Setup(x => x.GetCategoriesByUserReactionAsync(1, ReactionEnum.Dislike)).ReturnsAsync(dislikedCategories);
            _userArticleActionRepositoryMock.Setup(x => x.GetCategoriesBySavedUserArticleAsync(1)).ReturnsAsync(savedCategories);
            _customUserArticleReadTrackingRepositoryMock.Setup(x => x.GetCategoriesByUserReadSequenceAsync(1)).ReturnsAsync(readCategories);
            _userNotificationRepositoryMock.Setup(x => x.GetCategoriesByUserNotificationsAsync(1)).ReturnsAsync(notificationCategories);
            _userNotificationRepositoryMock.Setup(x => x.GetKeywordsByUserNotificationsAsync(1)).ReturnsAsync(notificationKeywords);
            _userArticleActionRepositoryMock.Setup(x => x.GetReportedArticleCategoriesByUserAsync(1)).ReturnsAsync(reportedCategories);

            var articles = new List<Article>
            {
                new Article { ArticleId = 1, Title = "Recommended 1" },
                new Article { ArticleId = 2, Title = "Recommended 2" }
            };
            var recommendedArticles = new List<ArticleDto>
            {
                new ArticleDto { ArticleId = 1, Title = "Recommended 1" },
                new ArticleDto { ArticleId = 2, Title = "Recommended 2" }
            };

            _articleRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<ArticleQueryDto>())).ReturnsAsync(articles);
            _mapperMock.Setup(x => x.Map<IEnumerable<ArticleDto>>(articles)).Returns(recommendedArticles);

            var result = await _articleService.GetRecommendedArticlesAsync(20);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        #endregion

        #region GetSavedArticlesForCurrentUserAsync Tests

        [TestMethod]
        public async Task GetSavedArticlesForCurrentUserAsync_ShouldReturnSavedArticles()
        {
            var savedArticleIds = new List<int> { 1, 2 };
            var articles = new List<Article>
            {
                new Article { ArticleId = 1, Title = "Saved 1" },
                new Article { ArticleId = 2, Title = "Saved 2" }
            };
            var savedArticleDtos = new List<ArticleDto>
            {
                new ArticleDto { ArticleId = 1, Title = "Saved 1" },
                new ArticleDto { ArticleId = 2, Title = "Saved 2" }
            };

            _userArticleActionServiceMock.Setup(x => x.GetSavedArticleIdsByUserIdAsync()).ReturnsAsync(savedArticleIds);
            _articleRepositoryMock.Setup(x => x.GetArticlesByIdsAsync(savedArticleIds)).ReturnsAsync(articles);
            _mapperMock.Setup(x => x.Map<IEnumerable<ArticleDto>>(articles)).Returns(savedArticleDtos);

            var result = await _articleService.GetSavedArticlesForCurrentUserAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            _userArticleActionServiceMock.Verify(x => x.GetSavedArticleIdsByUserIdAsync(), Times.Once);
            _articleRepositoryMock.Verify(x => x.GetArticlesByIdsAsync(savedArticleIds), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<ArticleDto>>(articles), Times.Once);
        }

        #endregion

        #region ArticleExistsAsync Tests

        [TestMethod]
        public async Task ArticleExistsAsync_ShouldReturnTrue_WhenArticleExists()
        {
            var article = new Article { Title = "Test Article", Content = "Test Content" };
            _articleRepositoryMock.Setup(x => x.ArticleExistsAsync(article)).ReturnsAsync(true);

            var result = await _articleService.ArticleExistsAsync(article);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ArticleExistsAsync_ShouldReturnFalse_WhenArticleDoesNotExist()
        {
            var article = new Article { Title = "Test Article", Content = "Test Content" };
            _articleRepositoryMock.Setup(x => x.ArticleExistsAsync(article)).ReturnsAsync(false);

            var result = await _articleService.ArticleExistsAsync(article);

            Assert.IsFalse(result);
        }

        #endregion

        #region HideArticleAsync Tests

        [TestMethod]
        public async Task HideArticleAsync_ShouldReturnTrue_WhenArticleExists()
        {
            var article = new Article { ArticleId = 1, Title = "Test Article", IsHidden = false };
            _articleRepositoryMock.Setup(x => x.GetArticleByIdAsync(1)).ReturnsAsync(article);
            _articleRepositoryMock.Setup(x => x.UpdateArticle(It.IsAny<Article>())).ReturnsAsync(true);
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));

            var result = await _articleService.HideArticleAsync(1);

            Assert.IsTrue(result);
            _articleRepositoryMock.Verify(x => x.UpdateArticle(It.IsAny<Article>()), Times.Once);
        }

        [TestMethod]
        public async Task HideArticleAsync_ShouldReturnFalse_WhenArticleDoesNotExist()
        {
            _articleRepositoryMock.Setup(x => x.GetArticleByIdAsync(1)).ReturnsAsync((Article)null);

            var result = await _articleService.HideArticleAsync(1);

            Assert.IsFalse(result);
        }

        #endregion

        #region HideArticlesByCategoryAsync Tests

        [TestMethod]
        public async Task HideArticlesByCategoryAsync_ShouldHideArticlesInCategory()
        {
            var query = new ArticleQueryDto { CategoryId = 1, IsHidden = false };
            var articles = new List<Article>
            {
                new Article { ArticleId = 1, Title = "Article 1", IsHidden = false },
                new Article { ArticleId = 2, Title = "Article 2", IsHidden = false }
            };

            _articleRepositoryMock.Setup(x => x.GetAllAsync(query)).ReturnsAsync(articles);
            _crudBaseRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Article>())).Returns(Task.FromResult<object>(null));
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));

            await _articleService.HideArticlesByCategoryAsync(query);

            _crudBaseRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Exactly(2));
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UnhideArticlesByCategoryAsync Tests

        [TestMethod]
        public async Task UnhideArticlesByCategoryAsync_ShouldUnhideArticlesInCategory()
        {
            var query = new ArticleQueryDto { CategoryId = 1, IsHidden = true };
            var articles = new List<Article>
            {
                new Article { ArticleId = 1, Title = "Article 1", IsHidden = true },
                new Article { ArticleId = 2, Title = "Article 2", IsHidden = true }
            };

            _articleRepositoryMock.Setup(x => x.GetAllAsync(query)).ReturnsAsync(articles);
            _userArticleActionServiceMock.Setup(x => x.GetReportCountForArticleAsync(It.IsAny<int>())).ReturnsAsync(0);
            _crudBaseRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Article>())).Returns(Task.FromResult<object>(null));
            _crudBaseRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));

            await _articleService.UnhideArticlesByCategoryAsync(query);

            _crudBaseRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Exactly(2));
            _crudBaseRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _userArticleActionServiceMock.Verify(x => x.GetReportCountForArticleAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        #endregion
    }
} 