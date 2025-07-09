using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NewsAggregation.Controllers;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsAggregation.Constants;
using NewsAggregation.Utilities;

namespace UnitTests.Controllers
{
    [TestClass]
    public class UserKeywordControllerTests
    {
        #region Private Fields

        private Mock<IUserKeywordService> _userKeywordServiceMock;
        private Mock<ILogger<UserKeywordController>> _loggerMock;
        private UserKeywordController _controller;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _userKeywordServiceMock = new Mock<IUserKeywordService>();
            _loggerMock = new Mock<ILogger<UserKeywordController>>();
            _controller = new UserKeywordController(_loggerMock.Object, _userKeywordServiceMock.Object);
        }

        #endregion

        #region AddUserKeyword Tests

        [TestMethod]
        public async Task AddUserKeyword_ShouldReturnCreatedAtAction_WhenValidUserKeyword()
        {
            var userKeywordDto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 1,
                Keyword = "Technology",
                IsEnabled = true
            };

            var expectedResult = new UserKeywordDto
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 1,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordServiceMock.Setup(x => x.AddAsync(userKeywordDto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.AddUserKeyword(userKeywordDto);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(UserKeywordController.AddUserKeyword), createdAtActionResult.ActionName);
            Assert.AreEqual(expectedResult, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task AddUserKeyword_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var userKeywordDto = new UserKeywordDto();
            _controller.ModelState.AddModelError("Keyword", "Keyword is required");

            var result = await _controller.AddUserKeyword(userKeywordDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task AddUserKeyword_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var userKeywordDto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 1,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordServiceMock.Setup(x => x.AddAsync(userKeywordDto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.AddUserKeyword(userKeywordDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetAllUserKeywords Tests

        [TestMethod]
        public async Task GetAllUserKeywords_ShouldReturnOkResult_WhenUserKeywordsExist()
        {
            var expectedUserKeywords = new List<UserKeywordDto>
            {
                new UserKeywordDto { UserKeywordId = 1, UserId = 1, CategoryId = 1, Keyword = "Technology" },
                new UserKeywordDto { UserKeywordId = 2, UserId = 1, CategoryId = 2, Keyword = "Science" }
            };

            _userKeywordServiceMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedUserKeywords);

            var result = await _controller.GetAllUserKeywords();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUserKeywords, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllUserKeywords_ShouldReturnEmptyList_WhenNoUserKeywordsExist()
        {
            var expectedUserKeywords = new List<UserKeywordDto>();

            _userKeywordServiceMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedUserKeywords);

            var result = await _controller.GetAllUserKeywords();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUserKeywords, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllUserKeywords_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _userKeywordServiceMock.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetAllUserKeywords();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetUserKeywordById Tests

        [TestMethod]
        public async Task GetUserKeywordById_ShouldReturnOkResult_WhenUserKeywordExists()
        {
            var expectedUserKeyword = new UserKeywordDto
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 1,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordServiceMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(expectedUserKeyword);

            var result = await _controller.GetUserKeywordById(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedUserKeyword, okResult.Value);
        }

        [TestMethod]
        public async Task GetUserKeywordById_ShouldReturnNotFound_WhenUserKeywordDoesNotExist()
        {
            _userKeywordServiceMock.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((UserKeywordDto)null);

            var result = await _controller.GetUserKeywordById(999);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task GetUserKeywordById_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _userKeywordServiceMock.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetUserKeywordById(1);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region UpdateUserKeyword Tests

        [TestMethod]
        public async Task UpdateUserKeyword_ShouldReturnOkResult_WhenUserKeywordUpdated()
        {
            var userKeywordDto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 1,
                Keyword = "Updated Technology",
                IsEnabled = true
            };

            var expectedResult = new UserKeywordDto
            {
                UserKeywordId = 1,
                UserId = 1,
                CategoryId = 1,
                Keyword = "Updated Technology",
                IsEnabled = true
            };

            _userKeywordServiceMock.Setup(x => x.UpdateAsync(1, userKeywordDto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.UpdateUserKeyword(1, userKeywordDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateUserKeyword_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var userKeywordDto = new UserKeywordDto();
            _controller.ModelState.AddModelError("Keyword", "Keyword is required");

            var result = await _controller.UpdateUserKeyword(1, userKeywordDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateUserKeyword_ShouldReturnNotFound_WhenUserKeywordDoesNotExist()
        {
            var userKeywordDto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 1,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordServiceMock.Setup(x => x.UpdateAsync(999, userKeywordDto))
                .ReturnsAsync((UserKeywordDto)null);

            var result = await _controller.UpdateUserKeyword(999, userKeywordDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateUserKeyword_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var userKeywordDto = new UserKeywordDto
            {
                UserId = 1,
                CategoryId = 1,
                Keyword = "Technology",
                IsEnabled = true
            };

            _userKeywordServiceMock.Setup(x => x.UpdateAsync(1, userKeywordDto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.UpdateUserKeyword(1, userKeywordDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region DeleteUserKeyword Tests

        [TestMethod]
        public async Task DeleteUserKeyword_ShouldReturnOkResult_WhenUserKeywordDeleted()
        {
            _userKeywordServiceMock.Setup(x => x.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteUserKeyword(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value;
            var message = value.GetType().GetProperty("message")?.GetValue(value, null) as string;
            Assert.AreEqual(SuccessConstants.UserKeywordDeleted, message);
        }

        [TestMethod]
        public async Task DeleteUserKeyword_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _userKeywordServiceMock.Setup(x => x.DeleteAsync(1))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.DeleteUserKeyword(1);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion
    }
} 