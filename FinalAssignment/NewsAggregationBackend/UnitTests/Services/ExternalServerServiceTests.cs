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
    public class ExternalServerServiceTests
    {
        #region Private Fields

        private Mock<ICrudBaseRepository<ExternalServer>> _crudBaseRepoMock;
        private Mock<IExternalServerRepository> _externalServerRepoMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IMapper> _mapperMock;
        private ExternalServerService _service;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            _crudBaseRepoMock = new Mock<ICrudBaseRepository<ExternalServer>>();
            _externalServerRepoMock = new Mock<IExternalServerRepository>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _mapperMock = new Mock<IMapper>();
            _service = new ExternalServerService(_crudBaseRepoMock.Object, _externalServerRepoMock.Object, _encryptionServiceMock.Object, _mapperMock.Object);
        }

        #endregion

        #region AddAsync Tests

        [TestMethod]
        public async Task AddAsync_ShouldReturnExternalServer_WhenValidDto()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "Test Server",
                BaseUrl = "https://api.test.com",
                ApiKeyHash = "test-api-key",
                IsActive = true
            };

            var entity = new ExternalServer
            {
                ExternalServerId = 1,
                ServerName = "Test Server",
                BaseUrl = "https://api.test.com",
                ApiKeyHash = "encrypted-api-key",
                IsActive = true
            };

            var resultDto = new ExternalServerDto
            {
                ExternalServerId = 1,
                ServerName = "Test Server",
                BaseUrl = "https://api.test.com",
                ApiKeyHash = "encrypted-api-key",
                IsActive = true
            };

            _mapperMock.Setup(x => x.Map<ExternalServer>(dto)).Returns(entity);
            _encryptionServiceMock.Setup(x => x.Encrypt(dto.ApiKeyHash)).Returns("encrypted-api-key");
            _crudBaseRepoMock.Setup(x => x.AddAsync(entity)).Returns(Task.CompletedTask);
            _crudBaseRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<ExternalServerDto>(entity)).Returns(resultDto);

            var result = await _service.AddAsync(dto);

            Assert.AreEqual(1, result.ExternalServerId);
            Assert.AreEqual(dto.ServerName, result.ServerName);
            Assert.AreEqual(dto.BaseUrl, result.BaseUrl);
            Assert.AreEqual("encrypted-api-key", result.ApiKeyHash);
            Assert.AreEqual(dto.IsActive, result.IsActive);
            _encryptionServiceMock.Verify(x => x.Encrypt(dto.ApiKeyHash), Times.Once);
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.AddAsync(null));
        }

        #endregion

        #region UpdateAsync Tests

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedExternalServer_WhenExternalServerExists()
        {
            var dto = new ExternalServerDto
            {
                ServerName = "Updated Server",
                BaseUrl = "https://api.updated.com",
                ApiKeyHash = "new-api-key",
                IsActive = false
            };

            var entity = new ExternalServer
            {
                ExternalServerId = 1,
                ServerName = "Old Server",
                BaseUrl = "https://api.old.com",
                ApiKeyHash = "old-encrypted-key",
                IsActive = true
            };

            var resultDto = new ExternalServerDto
            {
                ExternalServerId = 1,
                ServerName = "Updated Server",
                BaseUrl = "https://api.updated.com",
                ApiKeyHash = "new-encrypted-key",
                IsActive = false
            };

            _crudBaseRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(x => x.Map(dto, entity)).Returns(entity);
            _encryptionServiceMock.Setup(x => x.Encrypt(dto.ApiKeyHash)).Returns("new-encrypted-key");
            _crudBaseRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<ExternalServerDto>(entity)).Returns(resultDto);

            var result = await _service.UpdateAsync(1, dto);

            Assert.AreEqual(1, result.ExternalServerId);
            Assert.AreEqual(dto.ServerName, result.ServerName);
            Assert.AreEqual(dto.BaseUrl, result.BaseUrl);
            Assert.AreEqual("new-encrypted-key", result.ApiKeyHash);
            Assert.AreEqual(dto.IsActive, result.IsActive);
            _encryptionServiceMock.Verify(x => x.Encrypt(dto.ApiKeyHash), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenDtoIsNull()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(1, null));
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenExternalServerDoesNotExist()
        {
            var dto = new ExternalServerDto { ServerName = "Test Server", BaseUrl = "https://api.test.com", ApiKeyHash = "test-key", IsActive = true };
            _crudBaseRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((ExternalServer)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.UpdateAsync(999, dto));
        }

        #endregion

        #region GetByIdAsync Tests

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnExternalServer_WhenExternalServerExists()
        {
            var entity = new ExternalServer
            {
                ExternalServerId = 1,
                ServerName = "Test Server",
                BaseUrl = "https://api.test.com",
                ApiKeyHash = "encrypted-api-key",
                IsActive = true
            };

            var dto = new ExternalServerDto
            {
                ExternalServerId = 1,
                ServerName = "Test Server",
                BaseUrl = "https://api.test.com",
                ApiKeyHash = "encrypted-api-key",
                IsActive = true
            };

            _crudBaseRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(x => x.Map<ExternalServerDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.AreEqual(dto, result);
            Assert.AreEqual(1, result.ExternalServerId);
            Assert.AreEqual("Test Server", result.ServerName);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldThrowApiException_WhenExternalServerDoesNotExist()
        {
            _crudBaseRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((ExternalServer)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetByIdAsync(999));
        }

        #endregion

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnExternalServers_WhenExternalServersExist()
        {
            var entities = new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Server 1", BaseUrl = "https://api1.com", IsActive = true },
                new ExternalServer { ExternalServerId = 2, ServerName = "Server 2", BaseUrl = "https://api2.com", IsActive = false }
            };

            var dtos = new List<ExternalServerDto>
            {
                new ExternalServerDto { ExternalServerId = 1, ServerName = "Server 1", BaseUrl = "https://api1.com", IsActive = true },
                new ExternalServerDto { ExternalServerId = 2, ServerName = "Server 2", BaseUrl = "https://api2.com", IsActive = false }
            };

            _externalServerRepoMock.Setup(x => x.GetAllAsync(null)).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<ExternalServerDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Server 1", result.First().ServerName);
            Assert.AreEqual("Server 2", result.Last().ServerName);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnActiveExternalServers_WhenActiveFilterIsTrue()
        {
            var entities = new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 1, ServerName = "Active Server", BaseUrl = "https://api1.com", IsActive = true }
            };

            var dtos = new List<ExternalServerDto>
            {
                new ExternalServerDto { ExternalServerId = 1, ServerName = "Active Server", BaseUrl = "https://api1.com", IsActive = true }
            };

            _externalServerRepoMock.Setup(x => x.GetAllAsync(true)).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<ExternalServerDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllAsync(true);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Active Server", result.First().ServerName);
            Assert.IsTrue(result.First().IsActive);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnInactiveExternalServers_WhenActiveFilterIsFalse()
        {
            var entities = new List<ExternalServer>
            {
                new ExternalServer { ExternalServerId = 2, ServerName = "Inactive Server", BaseUrl = "https://api2.com", IsActive = false }
            };

            var dtos = new List<ExternalServerDto>
            {
                new ExternalServerDto { ExternalServerId = 2, ServerName = "Inactive Server", BaseUrl = "https://api2.com", IsActive = false }
            };

            _externalServerRepoMock.Setup(x => x.GetAllAsync(false)).ReturnsAsync(entities);
            _mapperMock.Setup(x => x.Map<IEnumerable<ExternalServerDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllAsync(false);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Inactive Server", result.First().ServerName);
            Assert.IsFalse(result.First().IsActive);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldThrowApiException_WhenNoExternalServersExist()
        {
            _externalServerRepoMock.Setup(x => x.GetAllAsync(null)).ReturnsAsync((IEnumerable<ExternalServer>)null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllAsync());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldThrowApiException_WhenEmptyExternalServersList()
        {
            _externalServerRepoMock.Setup(x => x.GetAllAsync(null)).ReturnsAsync(new List<ExternalServer>());

            await Assert.ThrowsExceptionAsync<ApiException>(() => _service.GetAllAsync());
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateService_WhenValidParameters()
        {
            var service = new ExternalServerService(_crudBaseRepoMock.Object, _externalServerRepoMock.Object, _encryptionServiceMock.Object, _mapperMock.Object);
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenCrudBaseRepositoryIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalServerService(null, _externalServerRepoMock.Object, _encryptionServiceMock.Object, _mapperMock.Object));
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenExternalServerRepositoryIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalServerService(_crudBaseRepoMock.Object, null, _encryptionServiceMock.Object, _mapperMock.Object));
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenEncryptionServiceIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalServerService(_crudBaseRepoMock.Object, _externalServerRepoMock.Object, null, _mapperMock.Object));
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalServerService(_crudBaseRepoMock.Object, _externalServerRepoMock.Object, _encryptionServiceMock.Object, null));
        }

        #endregion
    }
} 