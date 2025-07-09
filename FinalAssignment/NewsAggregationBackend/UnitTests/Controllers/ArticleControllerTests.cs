using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Controllers;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using NewsAggregation.Enums;

namespace UnitTests.Controllers
{
    [TestClass]
    public class ArticleControllerTests
    {
        #region Private Fields

        private Mock<IArticleService> _articleServiceMock;
        private Mock<IUserArticleActionService> _userArticleActionServiceMock;
        private Mock<ILogger<ArticleController>> _loggerMock;
        private ArticleController _controller;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _articleServiceMock = new Mock<IArticleService>();
            _userArticleActionServiceMock = new Mock<IUserArticleActionService>();
            _loggerMock = new Mock<ILogger<ArticleController>>();
            _controller = new ArticleController(_loggerMock.Object, _articleServiceMock.Object, _userArticleActionServiceMock.Object);
        }

        #endregion

        #region AddArticle Tests

        [TestMethod]
        public async Task AddArticle_ShouldReturnCreatedAtAction_WhenArticleIsValid()
        {
            var articleDto = new ArticleDto { Title = "Test Article", Content = "Test Content" };
            var resultDto = new ArticleDto { ArticleId = 1, Title = "Test Article", Content = "Test Content" };
            _articleServiceMock.Setup(x => x.AddAsync(articleDto)).ReturnsAsync(resultDto);

            var result = await _controller.AddArticle(articleDto);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual("GetArticleById", createdAtActionResult.ActionName);
            Assert.AreEqual(resultDto, createdAtActionResult.Value);
            Assert.AreEqual(1, createdAtActionResult.RouteValues["id"]);
        }

        [TestMethod]
        public async Task AddArticle_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Title", "Required");
            var articleDto = new ArticleDto();

            var result = await _controller.AddArticle(articleDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.Validation, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task AddArticle_ShouldReturnError_WhenArticleServiceThrowsApiException()
        {
            var articleDto = new ArticleDto { Title = "Test Article" };
            _articleServiceMock.Setup(x => x.AddAsync(articleDto)).ThrowsAsync(new ApiException(ErrorResponse.ErrorEnum.InternalServerError, "Custom error"));

            var result = await _controller.AddArticle(articleDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        #endregion

        #region GetArticleById Tests

        [TestMethod]
        public async Task GetArticleById_ShouldReturnOk_WhenArticleExists()
        {
            var articleDetailsDto = new ArticleDetailsDto { ArticleId = 1, Title = "Test Article" };
            _articleServiceMock.Setup(x => x.GetByIdWithUserDetailsAsync(1)).ReturnsAsync(articleDetailsDto);

            var result = await _controller.GetArticleById(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(articleDetailsDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetArticleById_ShouldReturnError_WhenArticleDoesNotExist()
        {
            _articleServiceMock.Setup(x => x.GetByIdWithUserDetailsAsync(1)).ReturnsAsync((ArticleDetailsDto)null);

            var result = await _controller.GetArticleById(1);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.NotFound, objectResult.StatusCode);
        }

        #endregion

        #region GetAllArticles Tests

        [TestMethod]
        public async Task GetAllArticles_ShouldReturnOk_WhenArticlesExist()
        {
            var query = new ArticleQueryDto { CategoryId = 1 };
            var articles = new List<ArticleDto>
            {
                new ArticleDto { ArticleId = 1, Title = "Article 1" },
                new ArticleDto { ArticleId = 2, Title = "Article 2" }
            };
            _articleServiceMock.Setup(x => x.GetAllAsync(query)).ReturnsAsync(articles);

            var result = await _controller.GetAllArticles(query);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(articles, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllArticles_ShouldReturnOk_WhenNoArticlesExist()
        {
            var query = new ArticleQueryDto { CategoryId = 999 };
            var articles = Enumerable.Empty<ArticleDto>();
            _articleServiceMock.Setup(x => x.GetAllAsync(query)).ReturnsAsync(articles);

            var result = await _controller.GetAllArticles(query);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(articles, okResult.Value);
        }

        #endregion

        #region ToggleSaveArticle Tests

        [TestMethod]
        public async Task ToggleSaveArticle_ShouldReturnOk_WhenToggleSuccessful()
        {
            var request = new ToggleSaveRequestDto { ArticleId = 1 };
            var result = new ToggleSaveResponseDto { IsSaved = true, Message = "Article saved successfully" };
            _userArticleActionServiceMock.Setup(x => x.ToggleSaveAsync(1)).ReturnsAsync(result);

            var actionResult = await _controller.ToggleSaveArticle(request);

            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(result, okResult.Value);
        }

        [TestMethod]
        public async Task ToggleSaveArticle_ShouldReturnError_WhenServiceThrowsException()
        {
            var request = new ToggleSaveRequestDto { ArticleId = 1 };
            _userArticleActionServiceMock.Setup(x => x.ToggleSaveAsync(1)).ThrowsAsync(new ApiException(ErrorResponse.ErrorEnum.NotFound, "Article not found"));

            var result = await _controller.ToggleSaveArticle(request);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.NotFound, objectResult.StatusCode);
        }

        #endregion

        #region AddArticleReaction Tests

        [TestMethod]
        public async Task AddArticleReaction_ShouldReturnOk_WhenReactionAdded()
        {
            var request = new ArticleReactionRequestDto { ArticleId = 1, ArticleReaction = ReactionEnum.Like };
            _userArticleActionServiceMock.Setup(x => x.AddArticleReaction(request)).ReturnsAsync(true);

            var actionResult = await _controller.AddArticleReaction(request);

            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(true, okResult.Value);
        }

        #endregion

        #region GetRecommendedArticles Tests

        [TestMethod]
        public async Task GetRecommendedArticles_ShouldReturnOk_WhenRecommendationsExist()
        {
            var recommendedArticles = new List<ArticleDto>
            {
                new ArticleDto { ArticleId = 1, Title = "Recommended 1" },
                new ArticleDto { ArticleId = 2, Title = "Recommended 2" }
            };
            _articleServiceMock.Setup(x => x.GetRecommendedArticlesAsync(20)).ReturnsAsync(recommendedArticles);

            var result = await _controller.GetRecommendedArticles(20);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(recommendedArticles, okResult.Value);
        }

        [TestMethod]
        public async Task GetRecommendedArticles_ShouldReturnOk_WhenNoRecommendationsExist()
        {
            var recommendedArticles = new List<ArticleDto>();
            _articleServiceMock.Setup(x => x.GetRecommendedArticlesAsync(10)).ReturnsAsync(recommendedArticles);

            var result = await _controller.GetRecommendedArticles(10);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(recommendedArticles, okResult.Value);
        }

        #endregion

        #region GetSavedArticles Tests

        [TestMethod]
        public async Task GetSavedArticles_ShouldReturnOk_WhenSavedArticlesExist()
        {
            var savedArticles = new List<ArticleDto>
            {
                new ArticleDto { ArticleId = 1, Title = "Saved 1" },
                new ArticleDto { ArticleId = 2, Title = "Saved 2" }
            };
            _articleServiceMock.Setup(x => x.GetSavedArticlesForCurrentUserAsync()).ReturnsAsync(savedArticles);

            var result = await _controller.GetSavedArticles();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(savedArticles, okResult.Value);
        }

        #endregion

        #region DeleteArticleReaction Tests

        [TestMethod]
        public async Task DeleteArticleReaction_ShouldReturnOk_WhenReactionDeleted()
        {
            _userArticleActionServiceMock.Setup(x => x.DeleteArticleReaction(1)).ReturnsAsync(true);

            var actionResult = await _controller.DeleteArticleReaction(1);

            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(true, okResult.Value);
        }

        #endregion

        #region ReportArticle Tests

        [TestMethod]
        public async Task ReportArticle_ShouldReturnOk_WhenArticleReported()
        {
            var reportDto = new UserArticleReportDto { ArticleId = 1, ReportReason = "Inappropriate content" };
            var result = new UserArticleReportResponseDto { Success = true, Message = "Article reported successfully" };
            _userArticleActionServiceMock.Setup(x => x.ReportArticleAsync(reportDto)).ReturnsAsync(result);

            var actionResult = await _controller.ReportArticle(reportDto);

            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(result, okResult.Value);
        }

        #endregion

        #region HideArticle Tests

        [TestMethod]
        public async Task HideArticle_ShouldReturnOk_WhenArticleHidden()
        {
            _articleServiceMock.Setup(x => x.HideArticleAsync(1)).ReturnsAsync(true);

            var result = await _controller.HideArticle(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task HideArticle_ShouldReturnError_WhenArticleDoesNotExist()
        {
            _articleServiceMock.Setup(x => x.HideArticleAsync(999)).ReturnsAsync(false);

            var result = await _controller.HideArticle(999);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.NotFound, objectResult.StatusCode);
        }

        #endregion
    }
} 