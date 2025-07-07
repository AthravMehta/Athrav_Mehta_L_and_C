using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Models;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Enums;

namespace UnitTests.Services
{
    [TestClass]
    public class CategoryServiceTests
    {
        #region Private Fields

        private Mock<IMapper> _mapperMock;
        private Mock<IArticleService> _articleServiceMock;
        private Mock<IKeywordService> _keywordServiceMock;
        private Mock<ICrudBaseRepository<Category>> _categoryRepositoryMock;
        private Mock<IUserNotificationConfigurationService> _userNotificationConfigurationServiceMock;
        private CategoryService _categoryService;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _articleServiceMock = new Mock<IArticleService>();
            _keywordServiceMock = new Mock<IKeywordService>();
            _categoryRepositoryMock = new Mock<ICrudBaseRepository<Category>>();
            _userNotificationConfigurationServiceMock = new Mock<IUserNotificationConfigurationService>();
            
            _categoryService = new CategoryService(
                _mapperMock.Object,
                _articleServiceMock.Object,
                _categoryRepositoryMock.Object,
                _keywordServiceMock.Object,
                _userNotificationConfigurationServiceMock.Object);
        }

        #endregion

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenCategoryDtoIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _categoryService.AddAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAsync_ShouldReturnCategoryDto_WhenCategoryIsValid()
        {
            var categoryDto = new CategoryDto { Name = "Technology" };
            var category = new Category { CategoryId = 1, Name = "Technology" };

            _categoryRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Category>())).Returns(Task.FromResult<object>(null));
            _categoryRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));
            _userNotificationConfigurationServiceMock.Setup(x => x.CreateNotificationConfigForAllUsersAsync(It.IsAny<int?>(), null)).Returns(Task.CompletedTask);

            var result = await _categoryService.AddAsync(categoryDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Technology", result.Name);
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _userNotificationConfigurationServiceMock.Verify(x => x.CreateNotificationConfigForAllUsersAsync(It.IsAny<int?>(), null), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenCategoryDtoIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _categoryService.UpdateAsync(1, null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenCategoryNotFound()
        {
            var categoryDto = new CategoryDto { Name = "Updated Technology" };
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Category)null);

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _categoryService.UpdateAsync(1, categoryDto));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedCategoryDto_WhenCategoryExists()
        {
            var categoryDto = new CategoryDto { Name = "Updated Technology" };
            var category = new Category { CategoryId = 1, Name = "Technology" };
            var updatedCategoryDto = new CategoryDto { CategoryId = 1, Name = "Updated Technology" };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            _categoryRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.FromResult<object>(null));
            _categoryRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));
            _mapperMock.Setup(x => x.Map<CategoryDto>(It.IsAny<Category>())).Returns(updatedCategoryDto);

            var result = await _categoryService.UpdateAsync(1, categoryDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Technology", result.Name);
            _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEmptyEnumerable_WhenNoCategoriesExist()
        {
            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Category>());

            var result = await _categoryService.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnCategories_WhenCategoriesExist()
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Technology" },
                new Category { CategoryId = 2, Name = "Sports" }
            };
            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, Name = "Technology" },
                new CategoryDto { CategoryId = 2, Name = "Sports" }
            };

            _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoryDto>>(categories)).Returns(categoryDtos);

            var result = await _categoryService.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetCategoryIdAsync_ShouldThrowApiException_WhenArticleIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _categoryService.GetCategoryIdAsync(null, new List<Keywords>(), new List<CategoryDto>()));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task GetCategoryIdAsync_ShouldReturnKeywordCategoryId_WhenKeywordMatches()
        {
            var article = new Article { Title = "Technology News", Content = "Latest tech updates" };
            var keywords = new List<Keywords> { new Keywords { Keyword = "technology", CategoryId = 1 } };
            var categories = new List<CategoryDto>();

            var result = await _categoryService.GetCategoryIdAsync(article, keywords, categories);

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, article.CategoryId);
        }

        [TestMethod]
        public async Task GetCategoryIdAsync_ShouldReturnAllCategoryId_WhenNoKeywordMatches()
        {
            var article = new Article { Title = "Random News", Content = "Random content" };
            var keywords = new List<Keywords> { new Keywords { Keyword = "technology", CategoryId = 1 } };
            var categories = new List<CategoryDto> { new CategoryDto { Name = "All", CategoryId = 2 } };

            var result = await _categoryService.GetCategoryIdAsync(article, keywords, categories);

            Assert.AreEqual(2, result);
            Assert.AreEqual(2, article.CategoryId);
        }

        [TestMethod]
        public async Task GetCategoryIdAsync_ShouldReturnZero_WhenNoMatches()
        {
            var article = new Article { Title = "Random News", Content = "Random content" };
            var keywords = new List<Keywords> { new Keywords { Keyword = "technology", CategoryId = 1 } };
            var categories = new List<CategoryDto> { new CategoryDto { Name = "Sports", CategoryId = 2 } };

            var result = await _categoryService.GetCategoryIdAsync(article, keywords, categories);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task HideCategoryAsync_ShouldThrowApiException_WhenCategoryNotFound()
        {
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Category)null);

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _categoryService.HideCategoryAsync(1, "Reason"));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task HideCategoryAsync_ShouldReturnTrue_WhenCategoryExists()
        {
            var category = new Category { CategoryId = 1, Name = "Technology", IsHidden = false };
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            _categoryRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.FromResult<object>(null));
            _categoryRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));
            _articleServiceMock.Setup(x => x.HideArticlesByCategoryAsync(It.IsAny<ArticleQueryDto>())).Returns(Task.FromResult<object>(null));

            var result = await _categoryService.HideCategoryAsync(1, "Inappropriate content");

            Assert.IsTrue(result);
            _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _articleServiceMock.Verify(x => x.HideArticlesByCategoryAsync(It.IsAny<ArticleQueryDto>()), Times.Once);
        }

        [TestMethod]
        public async Task UnhideCategoryAsync_ShouldThrowApiException_WhenCategoryNotFound()
        {
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Category)null);

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _categoryService.UnhideCategoryAsync(1));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task UnhideCategoryAsync_ShouldReturnTrue_WhenCategoryExists()
        {
            var category = new Category { CategoryId = 1, Name = "Technology", IsHidden = true };
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            _categoryRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.FromResult<object>(null));
            _categoryRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<object>(null));
            _articleServiceMock.Setup(x => x.UnhideArticlesByCategoryAsync(It.IsAny<ArticleQueryDto>())).Returns(Task.FromResult<object>(null));

            var result = await _categoryService.UnhideCategoryAsync(1);

            Assert.IsTrue(result);
            _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _articleServiceMock.Verify(x => x.UnhideArticlesByCategoryAsync(It.IsAny<ArticleQueryDto>()), Times.Once);
        }
    }
} 