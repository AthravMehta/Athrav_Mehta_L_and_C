using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;
using AutoMapper;
using NewsAggregation.Configurations.MappingConfigurations;

namespace UnitTests.Repository
{
    [TestClass]
    public class ArticleRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _context;
        private ArticleRepository _repository;
        private RequestContext _requestContext;
        private IMapper _mapper;

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

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ArticleProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _context = new NewsAggregationDbContext(options, _requestContext);
            _repository = new ArticleRepository(_context, _mapper, _requestContext);
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() =>
                new ArticleRepository(null, _mapper, _requestContext));
            Assert.AreEqual("context", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new NewsAggregationDbContext(options, _requestContext);

            var ex = Assert.ThrowsException<ArgumentNullException>(() =>
                new ArticleRepository(context, null, _requestContext));
            Assert.AreEqual("mapper", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenRequestContextIsNull()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new NewsAggregationDbContext(options, _requestContext);

            var ex = Assert.ThrowsException<ArgumentNullException>(() =>
                new ArticleRepository(context, _mapper, null));
            Assert.AreEqual("requestContext", ex.ParamName);
        }

        #endregion

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnEmptyEnumerable_WhenNoArticlesExist()
        {
            // Arrange
            var query = new ArticleQueryDto { CategoryId = 1 };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnArticles_WhenArticlesExist()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var articles = new List<Article>
            {
                new Article { 
                    Title = "Article 1", 
                    Content = "Content 1", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/article1",
                    PublishedDate = DateTime.UtcNow
                },
                new Article { 
                    Title = "Article 2", 
                    Content = "Content 2", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/article2",
                    PublishedDate = DateTime.UtcNow
                }
            };
            await _context.Articles.AddRangeAsync(articles);
            await _context.SaveChangesAsync();

            var query = new ArticleQueryDto { CategoryId = category.CategoryId };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldFilterByIsHidden_WhenSpecified()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var articles = new List<Article>
            {
                new Article { 
                    Title = "Visible Article", 
                    Content = "Content 1", 
                    CategoryId = category.CategoryId, 
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/visible",
                    PublishedDate = DateTime.UtcNow,
                    IsHidden = false 
                },
                new Article { 
                    Title = "Hidden Article", 
                    Content = "Content 2", 
                    CategoryId = category.CategoryId, 
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/hidden",
                    PublishedDate = DateTime.UtcNow,
                    IsHidden = true 
                }
            };
            await _context.Articles.AddRangeAsync(articles);
            await _context.SaveChangesAsync();

            var query = new ArticleQueryDto { CategoryId = category.CategoryId, IsHidden = false };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Visible Article", result.First().Title);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldFilterBySearchText_WhenSpecified()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var articles = new List<Article>
            {
                new Article { 
                    Title = "Technology Article", 
                    Content = "Content 1", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/tech",
                    PublishedDate = DateTime.UtcNow
                },
                new Article { 
                    Title = "Science Article", 
                    Content = "Content 2", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/science",
                    PublishedDate = DateTime.UtcNow
                }
            };
            await _context.Articles.AddRangeAsync(articles);
            await _context.SaveChangesAsync();

            var query = new ArticleQueryDto { CategoryId = category.CategoryId, SearchText = "Technology" };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Technology Article", result.First().Title);
        }

        #endregion

        #region GetArticleWithUserStatusAsync Tests

        [TestMethod]
        public async Task GetArticleWithUserStatusAsync_ShouldReturnNull_WhenArticleDoesNotExist()
        {
            // Act
            var result = await _repository.GetArticleWithUserStatusAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetArticleWithUserStatusAsync_ShouldReturnArticleDetails_WhenArticleExists()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var article = new Article { 
                Title = "Test Article", 
                Content = "Test Content", 
                CategoryId = category.CategoryId,
                ExternalServerId = externalServer.ExternalServerId,
                Source = "Test Source",
                Url = "https://test.com/article",
                PublishedDate = DateTime.UtcNow
            };
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetArticleWithUserStatusAsync(article.ArticleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(article.ArticleId, result.ArticleId);
            Assert.AreEqual("Test Article", result.Title);
            Assert.AreEqual("Test Content", result.Content);
        }

        #endregion

        #region GetArticleByIdAsync Tests

        [TestMethod]
        public async Task GetArticleByIdAsync_ShouldReturnNull_WhenArticleDoesNotExist()
        {
            // Act
            var result = await _repository.GetArticleByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExists()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var article = new Article { 
                Title = "Test Article", 
                Content = "Test Content", 
                CategoryId = category.CategoryId,
                ExternalServerId = externalServer.ExternalServerId,
                Source = "Test Source",
                Url = "https://test.com/article",
                PublishedDate = DateTime.UtcNow
            };
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetArticleByIdAsync(article.ArticleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(article.ArticleId, result.ArticleId);
            Assert.AreEqual("Test Article", result.Title);
        }

        #endregion

        #region GetArticlesByIdsAsync Tests

        [TestMethod]
        public async Task GetArticlesByIdsAsync_ShouldReturnEmptyEnumerable_WhenNoIdsProvided()
        {
            // Act
            var result = await _repository.GetArticlesByIdsAsync(new List<int>());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetArticlesByIdsAsync_ShouldReturnEmptyEnumerable_WhenIdsIsNull()
        {
            // Act
            var result = await _repository.GetArticlesByIdsAsync(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetArticlesByIdsAsync_ShouldReturnArticles_WhenArticlesExist()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var articles = new List<Article>
            {
                new Article { 
                    Title = "Article 1", 
                    Content = "Content 1", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/article1",
                    PublishedDate = DateTime.UtcNow
                },
                new Article { 
                    Title = "Article 2", 
                    Content = "Content 2", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/article2",
                    PublishedDate = DateTime.UtcNow
                }
            };
            await _context.Articles.AddRangeAsync(articles);
            await _context.SaveChangesAsync();

            var articleIds = articles.Select(a => a.ArticleId).ToList();

            // Act
            var result = await _repository.GetArticlesByIdsAsync(articleIds);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        #endregion

        #region ArticleExistsAsync Tests

        [TestMethod]
        public async Task ArticleExistsAsync_ShouldThrowArgumentNullException_WhenArticleIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _repository.ArticleExistsAsync(null));
        }

        [TestMethod]
        public async Task ArticleExistsAsync_ShouldReturnFalse_WhenArticleDoesNotExist()
        {
            // Arrange
            var article = new Article { Url = "https://example.com/article1" };

            // Act
            var result = await _repository.ArticleExistsAsync(article);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ArticleExistsAsync_ShouldReturnTrue_WhenArticleExists()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var article = new Article { 
                Title = "Test Article", 
                Content = "Test Content", 
                CategoryId = category.CategoryId, 
                ExternalServerId = externalServer.ExternalServerId,
                Source = "Test Source",
                Url = "https://example.com/article1",
                PublishedDate = DateTime.UtcNow
            };
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            var newArticle = new Article { Url = "https://example.com/article1" };

            // Act
            var result = await _repository.ArticleExistsAsync(newArticle);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region AddRangeAsync Tests

        [TestMethod]
        public async Task AddRangeAsync_ShouldThrowArgumentNullException_WhenArticlesIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _repository.AddRangeAsync(null));
        }

        [TestMethod]
        public async Task AddRangeAsync_ShouldAddArticles_WhenArticlesAreValid()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var articles = new List<Article>
            {
                new Article { 
                    Title = "Article 1", 
                    Content = "Content 1", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/article1",
                    PublishedDate = DateTime.UtcNow
                },
                new Article { 
                    Title = "Article 2", 
                    Content = "Content 2", 
                    CategoryId = category.CategoryId,
                    ExternalServerId = externalServer.ExternalServerId,
                    Source = "Test Source",
                    Url = "https://test.com/article2",
                    PublishedDate = DateTime.UtcNow
                }
            };

            // Act
            await _repository.AddRangeAsync(articles);
            await _context.SaveChangesAsync();

            // Assert
            var savedArticles = await _context.Articles.ToListAsync();
            Assert.AreEqual(2, savedArticles.Count);
        }

        #endregion

        #region UpdateArticle Tests

        [TestMethod]
        public async Task UpdateArticle_ShouldThrowArgumentNullException_WhenArticleIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _repository.UpdateArticle(null));
        }

        [TestMethod]
        public async Task UpdateArticle_ShouldReturnTrue_WhenArticleIsUpdated()
        {
            // Arrange
            var externalServer = new ExternalServer 
            { 
                ServerName = "Test Server", 
                BaseUrl = "https://test.com", 
                IsActive = true, 
                ApiKeyHash = "test-key" 
            };
            await _context.ExternalServers.AddAsync(externalServer);

            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var article = new Article { 
                Title = "Original Title", 
                Content = "Original Content", 
                CategoryId = category.CategoryId,
                ExternalServerId = externalServer.ExternalServerId,
                Source = "Test Source",
                Url = "https://test.com/article",
                PublishedDate = DateTime.UtcNow
            };
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            article.Title = "Updated Title";

            // Act
            var result = await _repository.UpdateArticle(article);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region SaveChangesAsync Tests

        [TestMethod]
        public async Task SaveChangesAsync_ShouldReturnTrue_WhenChangesAreSaved()
        {
            // Arrange
            var category = new Category { Name = "Technology" };
            await _context.Categories.AddAsync(category);

            // Act
            var result = await _repository.SaveChangesAsync();

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
} 