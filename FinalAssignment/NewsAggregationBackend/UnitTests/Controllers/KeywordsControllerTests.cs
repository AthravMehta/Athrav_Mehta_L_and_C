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
using NewsAggregation.Entities;
using AutoMapper;

namespace UnitTests.Controllers
{
    [TestClass]
    public class KeywordsControllerTests
    {
        #region Private Fields

        private Mock<IKeywordService> _keywordServiceMock;
        private Mock<ICrudBaseService<Keywords>> _crudBaseServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<KeywordsController>> _loggerMock;
        private KeywordsController _controller;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _keywordServiceMock = new Mock<IKeywordService>();
            _crudBaseServiceMock = new Mock<ICrudBaseService<Keywords>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<KeywordsController>>();
            _controller = new KeywordsController(_keywordServiceMock.Object, _crudBaseServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        #endregion

        #region GetAllKeywords Tests

        [TestMethod]
        public async Task GetAllKeywords_ShouldReturnOkResult_WhenKeywordsExist()
        {
            var keywords = new List<Keywords>
            {
                new Keywords { KeywordId = 1, Keyword = "Technology", CategoryId = 1 },
                new Keywords { KeywordId = 2, Keyword = "Science", CategoryId = 2 }
            };

            var keywordDtos = new List<KeywordDto>
            {
                new KeywordDto { KeywordId = 1, Keyword = "Technology", CategoryId = 1 },
                new KeywordDto { KeywordId = 2, Keyword = "Science", CategoryId = 2 }
            };

            _keywordServiceMock.Setup(x => x.GetAllKeywordsAsync())
                .ReturnsAsync(keywords);

            _mapperMock.Setup(x => x.Map<List<KeywordDto>>(keywords))
                .Returns(keywordDtos);

            var result = await _controller.GetAllKeywords();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(keywordDtos, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllKeywords_ShouldReturnEmptyList_WhenNoKeywordsExist()
        {
            var keywords = new List<Keywords>();
            var keywordDtos = new List<KeywordDto>();

            _keywordServiceMock.Setup(x => x.GetAllKeywordsAsync())
                .ReturnsAsync(keywords);

            _mapperMock.Setup(x => x.Map<List<KeywordDto>>(keywords))
                .Returns(keywordDtos);

            var result = await _controller.GetAllKeywords();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(keywordDtos, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllKeywords_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _keywordServiceMock.Setup(x => x.GetAllKeywordsAsync())
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetAllKeywords();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region AddKeywords Tests

        [TestMethod]
        public async Task AddKeywords_ShouldReturnOkResult_WhenValidKeywords()
        {
            var keywordDtos = new List<KeywordDto>
            {
                new KeywordDto { Keyword = "Technology", CategoryId = 1 },
                new KeywordDto { Keyword = "Science", CategoryId = 2 }
            };

            var keywords = new List<Keywords>
            {
                new Keywords { Keyword = "Technology", CategoryId = 1 },
                new Keywords { Keyword = "Science", CategoryId = 2 }
            };

            _mapperMock.Setup(x => x.Map<List<Keywords>>(keywordDtos))
                .Returns(keywords);

            _crudBaseServiceMock.Setup(x => x.AddRangeAsync(keywords))
                .Returns(Task.CompletedTask);

            var result = await _controller.AddKeywords(keywordDtos);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var messageResponse = okResult.Value as MessageResponseDto;
            Assert.IsNotNull(messageResponse);
            Assert.IsTrue(messageResponse.IsSuccess);
            Assert.AreEqual(SuccessConstants.KeywordsAdded, messageResponse.Message);
        }

        [TestMethod]
        public async Task AddKeywords_ShouldReturnBadRequest_WhenKeywordDtosIsNull()
        {
            var result = await _controller.AddKeywords(null);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task AddKeywords_ShouldReturnBadRequest_WhenKeywordDtosIsEmpty()
        {
            var keywordDtos = new List<KeywordDto>();

            var result = await _controller.AddKeywords(keywordDtos);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task AddKeywords_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var keywordDtos = new List<KeywordDto>
            {
                new KeywordDto { Keyword = "Technology", CategoryId = 1 }
            };

            var keywords = new List<Keywords>
            {
                new Keywords { Keyword = "Technology", CategoryId = 1 }
            };

            _mapperMock.Setup(x => x.Map<List<Keywords>>(keywordDtos))
                .Returns(keywords);

            _crudBaseServiceMock.Setup(x => x.AddRangeAsync(keywords))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.AddKeywords(keywordDtos);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region HideKeyword Tests

        [TestMethod]
        public async Task HideKeyword_ShouldReturnOkResult_WhenKeywordHidden()
        {
            var keywordId = 1;
            var reason = "Inappropriate content";

            _keywordServiceMock.Setup(x => x.HideKeywordAsync(keywordId, reason))
                .ReturnsAsync(true);

            var result = await _controller.HideKeyword(keywordId, reason);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var messageResponse = okResult.Value as MessageResponseDto;
            Assert.IsNotNull(messageResponse);
            Assert.AreEqual(SuccessConstants.KeywordHidden, messageResponse.Message);
        }

        [TestMethod]
        public async Task HideKeyword_ShouldReturnNotFound_WhenKeywordDoesNotExist()
        {
            var keywordId = 999;
            var reason = "Inappropriate content";

            _keywordServiceMock.Setup(x => x.HideKeywordAsync(keywordId, reason))
                .ReturnsAsync(false);

            var result = await _controller.HideKeyword(keywordId, reason);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task HideKeyword_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var keywordId = 1;
            var reason = "Inappropriate content";

            _keywordServiceMock.Setup(x => x.HideKeywordAsync(keywordId, reason))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.HideKeyword(keywordId, reason);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region UnhideKeyword Tests

        [TestMethod]
        public async Task UnhideKeyword_ShouldReturnOkResult_WhenKeywordUnhidden()
        {
            var keywordId = 1;

            _keywordServiceMock.Setup(x => x.UnhideKeywordAsync(keywordId))
                .ReturnsAsync(true);

            var result = await _controller.UnhideKeyword(keywordId);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var messageResponse = okResult.Value as MessageResponseDto;
            Assert.IsNotNull(messageResponse);
            Assert.AreEqual(SuccessConstants.KeywordUnhidden, messageResponse.Message);
        }

        [TestMethod]
        public async Task UnhideKeyword_ShouldReturnNotFound_WhenKeywordDoesNotExist()
        {
            var keywordId = 999;

            _keywordServiceMock.Setup(x => x.UnhideKeywordAsync(keywordId))
                .ReturnsAsync(false);

            var result = await _controller.UnhideKeyword(keywordId);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UnhideKeyword_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var keywordId = 1;

            _keywordServiceMock.Setup(x => x.UnhideKeywordAsync(keywordId))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.UnhideKeyword(keywordId);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion
    }
} 