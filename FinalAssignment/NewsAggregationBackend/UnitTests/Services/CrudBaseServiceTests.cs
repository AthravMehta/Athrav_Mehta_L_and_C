using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using NewsAggregation.Entities;

namespace UnitTests.Services
{
    [TestClass]
    public class CrudBaseServiceTests
    {
        #region Private Fields

        private Mock<ICrudBaseRepository<Category>> _repositoryMock;
        private Mock<ILogger<CrudBaseService<Category>>> _loggerMock;
        private CrudBaseService<Category> _service;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<ICrudBaseRepository<Category>>();
            _loggerMock = new Mock<ILogger<CrudBaseService<Category>>>();
            _service = new CrudBaseService<Category>(_repositoryMock.Object, _loggerMock.Object);
        }

        #endregion

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEntities_WhenEntitiesExist()
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Technology" },
                new Category { CategoryId = 2, Name = "Sports" }
            };
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldThrowApiException_WhenRepositoryThrowsException()
        {
            _repositoryMock.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.GetAllAsync());
            Assert.AreEqual(ErrorResponse.ErrorEnum.DatabaseError, ex.ErrorCode);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            var category = new Category { CategoryId = 1, Name = "Technology" };
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CategoryId);
            Assert.AreEqual("Technology", result.Name);
            _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldThrowApiException_WhenRepositoryThrowsException()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.GetByIdAsync(1));
            Assert.AreEqual(ErrorResponse.ErrorEnum.DatabaseError, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenEntityIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.AddAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddEntity_WhenEntityIsValid()
        {
            var category = new Category { Name = "Technology" };
            _repositoryMock.Setup(x => x.AddAsync(category)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.AddAsync(category);

            _repositoryMock.Verify(x => x.AddAsync(category), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenRepositoryThrowsException()
        {
            var category = new Category { Name = "Technology" };
            _repositoryMock.Setup(x => x.AddAsync(category)).ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.AddAsync(category));
            Assert.AreEqual(ErrorResponse.ErrorEnum.DatabaseError, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowApiException_WhenEntitiesIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.AddRangeAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldAddEntities_WhenEntitiesAreValid()
        {
            var categories = new List<Category>
            {
                new Category { Name = "Technology" },
                new Category { Name = "Sports" }
            };
            _repositoryMock.Setup(x => x.AddRangeAsync(categories)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.AddRangeAsync(categories);

            _repositoryMock.Verify(x => x.AddRangeAsync(categories), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowApiException_WhenRepositoryThrowsException()
        {
            var categories = new List<Category>
            {
                new Category { Name = "Technology" },
                new Category { Name = "Sports" }
            };
            _repositoryMock.Setup(x => x.AddRangeAsync(categories)).ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.AddRangeAsync(categories));
            Assert.AreEqual(ErrorResponse.ErrorEnum.DatabaseError, ex.ErrorCode);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenEntityIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.UpdateAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityIsValid()
        {
            var category = new Category { CategoryId = 1, Name = "Updated Technology" };
            _repositoryMock.Setup(x => x.UpdateAsync(category)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.UpdateAsync(category);

            _repositoryMock.Verify(x => x.UpdateAsync(category), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenRepositoryThrowsException()
        {
            var category = new Category { CategoryId = 1, Name = "Updated Technology" };
            _repositoryMock.Setup(x => x.UpdateAsync(category)).ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.UpdateAsync(category));
            Assert.AreEqual(ErrorResponse.ErrorEnum.DatabaseError, ex.ErrorCode);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteEntity_WhenEntityExists()
        {
            var category = new Category { CategoryId = 1, Name = "Technology" };
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            _repositoryMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _repositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldThrowApiException_WhenRepositoryThrowsException()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(1)).ThrowsAsync(new Exception("Database error"));

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _service.DeleteAsync(1));
            Assert.AreEqual(ErrorResponse.ErrorEnum.DatabaseError, ex.ErrorCode);
        }
    }
} 