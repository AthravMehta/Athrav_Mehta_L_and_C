using Microsoft.EntityFrameworkCore;
using NewsAggregation.Repository;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;

namespace UnitTests.Repository
{
    [TestClass]
    public class CrudBaseRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _context;
        private CrudBaseRepository<Category> _repository;
        private RequestContext _requestContext;

        #endregion

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _requestContext = new RequestContext
            {
                UserId = 1,
                Email = "test@example.com",
                Roles = new List<string> { nameof(RoleEnum.User) }
            };

            _context = new NewsAggregationDbContext(options, _requestContext);
            _repository = new CrudBaseRepository<Category>(_context);
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() =>
                new CrudBaseRepository<Category>(null));
            Assert.AreEqual("context", ex.ParamName);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEmptyEnumerable_WhenNoEntitiesExist()
        {
            var result = await _repository.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEntities_WhenEntitiesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Name = "Technology" },
                new Category { Name = "Sports" }
            };
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(category.CategoryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Technology", result.Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldThrowApiException_WhenEntityDoesNotExist()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _repository.GetByIdAsync(999));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowApiException_WhenEntityIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _repository.AddAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddEntity_WhenEntityIsValid()
        {
            // Arrange
            var category = new Category { Name = "Technology" };

            // Act
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            // Assert
            var savedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Technology");
            Assert.IsNotNull(savedCategory);
            Assert.AreEqual("Technology", savedCategory.Name);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowApiException_WhenEntitiesIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _repository.AddRangeAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowApiException_WhenEntitiesIsEmpty()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _repository.AddRangeAsync(new List<Category>()));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldAddEntities_WhenEntitiesAreValid()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Name = "Technology" },
                new Category { Name = "Sports" }
            };

            // Act
            await _repository.AddRangeAsync(categories);
            await _repository.SaveChangesAsync();

            // Assert
            var savedCategories = await _context.Categories.ToListAsync();
            Assert.AreEqual(2, savedCategories.Count);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowApiException_WhenEntityIsNull()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _repository.UpdateAsync(null));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NullObject, ex.ErrorCode);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityIsValid()
        {
            // Arrange
            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            category.Name = "Updated Technology";

            // Act
            await _repository.UpdateAsync(category);
            await _repository.SaveChangesAsync();

            // Assert
            var updatedCategory = await _context.Categories.FindAsync(category.CategoryId);
            Assert.AreEqual("Updated Technology", updatedCategory.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteEntity_WhenEntityExists()
        {
            // Arrange
            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(category.CategoryId);
            await _repository.SaveChangesAsync();

            // Assert
            var deletedCategory = await _context.Categories.FindAsync(category.CategoryId);
            Assert.IsNull(deletedCategory);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldThrowApiException_WhenEntityDoesNotExist()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await _repository.DeleteAsync(999));
            Assert.AreEqual(ErrorResponse.ErrorEnum.NotFound, ex.ErrorCode);
        }

        [TestMethod]
        public async Task SaveChangesAsync_ShouldSaveChanges()
        {
            // Arrange
            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var savedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Technology");
            Assert.IsNotNull(savedCategory);
        }
    }
} 