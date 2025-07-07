using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Controllers;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;

namespace UnitTests.Controllers
{
    [TestClass]
    public class CategoryControllerTests
    {
        #region Private Fields

        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<ILogger<CategoryController>> _loggerMock;
        private CategoryController _controller;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _loggerMock = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_loggerMock.Object, _categoryServiceMock.Object);
        }

        #endregion

        [TestMethod]
        public async Task Add_ShouldReturnOk_WhenCategoryIsValid()
        {
            var categoryDto = new CategoryDto { Name = "Technology" };
            var resultDto = new CategoryDto { CategoryId = 1, Name = "Technology" };
            _categoryServiceMock.Setup(x => x.AddAsync(categoryDto)).ReturnsAsync(resultDto);

            var result = await _controller.Add(categoryDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(resultDto, okResult.Value);
        }

        [TestMethod]
        public async Task Add_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var categoryDto = new CategoryDto();

            var result = await _controller.Add(categoryDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.Validation, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task Add_ShouldReturnError_WhenCategoryServiceThrowsApiException()
        {
            var categoryDto = new CategoryDto { Name = "Technology" };
            _categoryServiceMock.Setup(x => x.AddAsync(categoryDto)).ThrowsAsync(new ApiException(ErrorResponse.ErrorEnum.InternalServerError, "Custom error"));

            var result = await _controller.Add(categoryDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task Update_ShouldReturnOk_WhenCategoryExists()
        {
            var categoryDto = new CategoryDto { Name = "Updated Technology" };
            var resultDto = new CategoryDto { CategoryId = 1, Name = "Updated Technology" };
            _categoryServiceMock.Setup(x => x.UpdateAsync(1, categoryDto)).ReturnsAsync(resultDto);

            var result = await _controller.Update(1, categoryDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(resultDto, okResult.Value);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var categoryDto = new CategoryDto { Name = "Updated Technology" };
            _categoryServiceMock.Setup(x => x.UpdateAsync(1, categoryDto)).ReturnsAsync((CategoryDto)null);

            var result = await _controller.Update(1, categoryDto);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var categoryDto = new CategoryDto();

            var result = await _controller.Update(1, categoryDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)ErrorResponse.ErrorEnum.Validation, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WhenCategoriesExist()
        {
            var categories = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, Name = "Technology" },
                new CategoryDto { CategoryId = 2, Name = "Sports" }
            };
            _categoryServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            var result = await _controller.GetAll();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(categories, okResult.Value);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WhenNoCategoriesExist()
        {
            var categories = Enumerable.Empty<CategoryDto>();
            _categoryServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            var result = await _controller.GetAll();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(categories, okResult.Value);
        }

        [TestMethod]
        public async Task HideCategory_ShouldReturnOk_WhenCategoryExists()
        {
            var categoryId = 1;
            var reason = "Inappropriate content";
            _categoryServiceMock.Setup(x => x.HideCategoryAsync(categoryId, reason)).ReturnsAsync(true);

            var result = await _controller.HideCategory(categoryId, reason);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task HideCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var categoryId = 1;
            var reason = "Inappropriate content";
            _categoryServiceMock.Setup(x => x.HideCategoryAsync(categoryId, reason)).ReturnsAsync(false);

            var result = await _controller.HideCategory(categoryId, reason);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public async Task UnhideCategory_ShouldReturnOk_WhenCategoryExists()
        {
            var categoryId = 1;
            _categoryServiceMock.Setup(x => x.UnhideCategoryAsync(categoryId)).ReturnsAsync(true);

            var result = await _controller.UnhideCategory(categoryId);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task UnhideCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var categoryId = 1;
            _categoryServiceMock.Setup(x => x.UnhideCategoryAsync(categoryId)).ReturnsAsync(false);

            var result = await _controller.UnhideCategory(categoryId);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
        }
    }
} 