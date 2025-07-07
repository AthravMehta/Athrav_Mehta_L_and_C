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
    public class ExternalServerControllerTests
    {
        #region Private Fields

        private Mock<IExternalServerService> _externalServerServiceMock;
        private Mock<ILogger<ExternalServerController>> _loggerMock;
        private ExternalServerController _controller;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _externalServerServiceMock = new Mock<IExternalServerService>();
            _loggerMock = new Mock<ILogger<ExternalServerController>>();
            _controller = new ExternalServerController(_loggerMock.Object, _externalServerServiceMock.Object);
        }

        #endregion

        #region AddExternalServer Tests

        [TestMethod]
        public async Task AddExternalServer_ShouldReturnCreatedAtAction_WhenValidExternalServer()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "NewsAPI",
                BaseUrl = "https://newsapi.org",
                ApiKeyHash = "hash123",
                IsActive = true
            };

            var expectedResult = new ExternalServerDto
            {
                ExternalServerId = 1,
                ServerName = "NewsAPI",
                BaseUrl = "https://newsapi.org",
                ApiKeyHash = "hash123",
                IsActive = true
            };

            _externalServerServiceMock.Setup(x => x.AddAsync(dto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.AddExternalServer(dto);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(ExternalServerController.GetExternalServerById), createdAtActionResult.ActionName);
            Assert.AreEqual(expectedResult, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task AddExternalServer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var dto = new ExternalServerDto();
            _controller.ModelState.AddModelError("ServerName", "ServerName is required");

            var result = await _controller.AddExternalServer(dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task AddExternalServer_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "NewsAPI",
                BaseUrl = "https://newsapi.org",
                ApiKeyHash = "hash123",
                IsActive = true
            };

            _externalServerServiceMock.Setup(x => x.AddAsync(dto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.AddExternalServer(dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region UpdateExternalServer Tests

        [TestMethod]
        public async Task UpdateExternalServer_ShouldReturnOkResult_WhenExternalServerUpdated()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "UpdatedAPI",
                BaseUrl = "https://updatedapi.org",
                ApiKeyHash = "hash456",
                IsActive = false
            };

            var expectedResult = new ExternalServerDto
            {
                ExternalServerId = 1,
                ServerName = "UpdatedAPI",
                BaseUrl = "https://updatedapi.org",
                ApiKeyHash = "hash456",
                IsActive = false
            };

            _externalServerServiceMock.Setup(x => x.UpdateAsync(1, dto))
                .ReturnsAsync(expectedResult);

            var result = await _controller.UpdateExternalServer(1, dto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateExternalServer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var dto = new ExternalServerDto();
            _controller.ModelState.AddModelError("ServerName", "ServerName is required");

            var result = await _controller.UpdateExternalServer(1, dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(701, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateExternalServer_ShouldReturnNotFound_WhenExternalServerDoesNotExist()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "NewsAPI",
                BaseUrl = "https://newsapi.org",
                ApiKeyHash = "hash123",
                IsActive = true
            };

            _externalServerServiceMock.Setup(x => x.UpdateAsync(999, dto))
                .ReturnsAsync((ExternalServerDto)null);

            var result = await _controller.UpdateExternalServer(999, dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task UpdateExternalServer_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "NewsAPI",
                BaseUrl = "https://newsapi.org",
                ApiKeyHash = "hash123",
                IsActive = true
            };

            _externalServerServiceMock.Setup(x => x.UpdateAsync(1, dto))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.UpdateExternalServer(1, dto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetExternalServerById Tests

        [TestMethod]
        public async Task GetExternalServerById_ShouldReturnOkResult_WhenExternalServerExists()
        {
            var expectedResult = new ExternalServerDto
            {
                ExternalServerId = 1,
                ServerName = "NewsAPI",
                BaseUrl = "https://newsapi.org",
                ApiKeyHash = "hash123",
                IsActive = true
            };

            _externalServerServiceMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(expectedResult);

            var result = await _controller.GetExternalServerById(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task GetExternalServerById_ShouldReturnNotFound_WhenExternalServerDoesNotExist()
        {
            _externalServerServiceMock.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((ExternalServerDto)null);

            var result = await _controller.GetExternalServerById(999);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        [TestMethod]
        public async Task GetExternalServerById_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _externalServerServiceMock.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetExternalServerById(1);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion

        #region GetAllExternalServers Tests

        [TestMethod]
        public async Task GetAllExternalServers_ShouldReturnOkResult_WhenExternalServersExist()
        {
            var expectedServers = new List<ExternalServerDto>
            {
                new ExternalServerDto { ExternalServerId = 1, ServerName = "NewsAPI", BaseUrl = "https://newsapi.org", ApiKeyHash = "hash123", IsActive = true },
                new ExternalServerDto { ExternalServerId = 2, ServerName = "TheNewsAPI", BaseUrl = "https://thenewsapi.com", ApiKeyHash = "hash456", IsActive = false }
            };

            _externalServerServiceMock.Setup(x => x.GetAllAsync(It.IsAny<bool?>()))
                .ReturnsAsync(expectedServers);

            var result = await _controller.GetAllExternalServers();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedServers, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllExternalServers_ShouldReturnEmptyList_WhenNoExternalServersExist()
        {
            var expectedServers = new List<ExternalServerDto>();

            _externalServerServiceMock.Setup(x => x.GetAllAsync(It.IsAny<bool?>()))
                .ReturnsAsync(expectedServers);

            var result = await _controller.GetAllExternalServers();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedServers, okResult.Value);
        }

        [TestMethod]
        public async Task GetAllExternalServers_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            _externalServerServiceMock.Setup(x => x.GetAllAsync(It.IsAny<bool?>()))
                .ThrowsAsync(new Exception("Service error"));

            var result = await _controller.GetAllExternalServers();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);
        }

        #endregion
    }
} 