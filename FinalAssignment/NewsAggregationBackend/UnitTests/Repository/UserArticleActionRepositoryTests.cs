using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Repository;

namespace UnitTests.Repository
{
    [TestClass]
    public class UserArticleActionRepositoryTests
    {
        #region Private Fields

        private NewsAggregationDbContext _dbContext;
        private UserArticleActionRepository _repository;

        #endregion

        #region Setup

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var requestContext = new NewsAggregation.Configurations.RequestContext();
            _dbContext = new NewsAggregationDbContext(options, requestContext);
            _repository = new UserArticleActionRepository(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region AddArticleReaction Tests

        [TestMethod]
        public async Task AddArticleReaction_ShouldAddNewReaction_WhenNoExistingReaction()
        {
            var userId = 1;
            var request = new ArticleReactionRequestDto { ArticleId = 1, ArticleReaction = ReactionEnum.Like };

            var result = await _repository.AddArticleReaction(userId, request);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AddArticleReaction_ShouldUpdateExistingReaction_WhenDifferentReactionExists()
        {
            var userId = 1;
            var request = new ArticleReactionRequestDto { ArticleId = 1, ArticleReaction = ReactionEnum.Like };
            var existingReaction = new UserArticleReaction { UserId = userId, ArticleId = 1, Reaction = ReactionEnum.Dislike };

            _dbContext.UserArticleReactions.Add(existingReaction);
            _dbContext.SaveChanges();

            var result = await _repository.AddArticleReaction(userId, request);

            Assert.IsTrue(result);
            Assert.AreEqual(ReactionEnum.Like, existingReaction.Reaction);
        }

        [TestMethod]
        public async Task AddArticleReaction_ShouldReturnFalse_WhenSameReactionExists()
        {
            var userId = 1;
            var request = new ArticleReactionRequestDto { ArticleId = 1, ArticleReaction = ReactionEnum.Like };
            var existingReaction = new UserArticleReaction { UserId = userId, ArticleId = 1, Reaction = ReactionEnum.Like };

            _dbContext.UserArticleReactions.Add(existingReaction);
            _dbContext.SaveChanges();

            var result = await _repository.AddArticleReaction(userId, request);

            Assert.IsFalse(result);
        }

        #endregion

        #region DeleteArticleReaction Tests

        [TestMethod]
        public async Task DeleteArticleReaction_ShouldDeleteReaction_WhenReactionExists()
        {
            var userId = 1;
            var articleId = 1;
            var existingReaction = new UserArticleReaction { UserId = userId, ArticleId = articleId, Reaction = ReactionEnum.Like };

            _dbContext.UserArticleReactions.Add(existingReaction);
            _dbContext.SaveChanges();

            var result = await _repository.DeleteArticleReaction(userId, articleId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteArticleReaction_ShouldReturnFalse_WhenReactionDoesNotExist()
        {
            var userId = 1;
            var articleId = 1;

            var result = await _repository.DeleteArticleReaction(userId, articleId);

            Assert.IsFalse(result);
        }

        #endregion

        #region ToggleSaveAsync Tests

        [TestMethod]
        public async Task ToggleSaveAsync_ShouldSaveArticle_WhenArticleNotSaved()
        {
            var userId = 1;
            var articleId = 1;

            var result = await _repository.ToggleSaveAsync(userId, articleId);

            Assert.IsTrue(result.IsSaved);
            Assert.AreEqual(SuccessConstants.ArticleSaved, result.Message);
        }

        [TestMethod]
        public async Task ToggleSaveAsync_ShouldUnsaveArticle_WhenArticleAlreadySaved()
        {
            var userId = 1;
            var articleId = 1;
            var existingSaved = new UserSavedArticle { UserId = userId, ArticleId = articleId };

            _dbContext.UserSavedArticles.Add(existingSaved);
            _dbContext.SaveChanges();

            var result = await _repository.ToggleSaveAsync(userId, articleId);

            Assert.IsFalse(result.IsSaved);
            Assert.AreEqual(SuccessConstants.ArticleUnsaved, result.Message);
        }

        #endregion

        #region GetSavedArticleIdsByUserIdAsync Tests

        [TestMethod]
        public async Task GetSavedArticleIdsByUserIdAsync_ShouldReturnArticleIds_WhenSavedArticlesExist()
        {
            var userId = 1;
            var savedArticles = new List<UserSavedArticle>
            {
                new UserSavedArticle { UserId = userId, ArticleId = 1 },
                new UserSavedArticle { UserId = userId, ArticleId = 2 },
                new UserSavedArticle { UserId = 2, ArticleId = 3 }
            };

            _dbContext.UserSavedArticles.AddRange(savedArticles);
            _dbContext.SaveChanges();

            var result = await _repository.GetSavedArticleIdsByUserIdAsync(userId);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
        }

        #endregion

        #region HasUserReportedArticleAsync Tests

        [TestMethod]
        public async Task HasUserReportedArticleAsync_ShouldReturnTrue_WhenUserReportedArticle()
        {
            var userId = 1;
            var articleId = 1;
            _dbContext.UserArticleReports.Add(new UserArticleReport { UserId = userId, ArticleId = articleId, ReportReason = "Spam" });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.HasUserReportedArticleAsync(articleId, userId);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task HasUserReportedArticleAsync_ShouldReturnFalse_WhenUserDidNotReportArticle()
        {
            var userId = 1;
            var articleId = 1;
            await _dbContext.SaveChangesAsync();
            var result = await _repository.HasUserReportedArticleAsync(articleId, userId);
            Assert.IsFalse(result);
        }

        #endregion

        #region AddReportAsync Tests

        [TestMethod]
        public async Task AddReportAsync_ShouldAddReport_WhenValidReport()
        {
            var userId = 1;
            var articleId = 1;
            var report = new UserArticleReport { UserId = userId, ArticleId = articleId, ReportReason = "Spam" };
            await _repository.AddReportAsync(report);
            Assert.AreEqual(1, _dbContext.UserArticleReports.Count());
        }

        #endregion

        #region GetReportCountForArticleAsync Tests

        [TestMethod]
        public async Task GetReportCountForArticleAsync_ShouldReturnCount_WhenReportsExist()
        {
            var articleId = 1;
            _dbContext.UserArticleReports.AddRange(new List<UserArticleReport>
            {
                new UserArticleReport { UserId = 1, ArticleId = articleId, ReportReason = "Spam" },
                new UserArticleReport { UserId = 2, ArticleId = articleId, ReportReason = "Spam" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetReportCountForArticleAsync(articleId);
            Assert.AreEqual(2, result);
        }

        #endregion

        #region GetCategoriesByUserReactionAsync Tests

        [TestMethod]
        public async Task GetCategoriesByUserReactionAsync_ShouldReturnCategoryIds_WhenReactionsExist()
        {
            var userId = 1;
            _dbContext.UserArticleReactions.AddRange(new List<UserArticleReaction>
            {
                new UserArticleReaction { UserId = userId, ArticleId = 1, Reaction = ReactionEnum.Like, Article = new Article { CategoryId = 1, Title = "T1", Content = "C1", Source = "S1", Url = "U1", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 } },
                new UserArticleReaction { UserId = userId, ArticleId = 2, Reaction = ReactionEnum.Like, Article = new Article { CategoryId = 2, Title = "T2", Content = "C2", Source = "S2", Url = "U2", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 } },
                new UserArticleReaction { UserId = 2, ArticleId = 3, Reaction = ReactionEnum.Like, Article = new Article { CategoryId = 3, Title = "T3", Content = "C3", Source = "S3", Url = "U3", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 } }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetCategoriesByUserReactionAsync(userId, ReactionEnum.Like);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
        }

        #endregion

        #region GetCategoriesBySavedUserArticleAsync Tests

        [TestMethod]
        public async Task GetCategoriesBySavedUserArticleAsync_ShouldReturnCategoryIds_WhenSavedArticlesExist()
        {
            var userId = 1;
            _dbContext.UserSavedArticles.AddRange(new List<UserSavedArticle>
            {
                new UserSavedArticle { UserId = userId, ArticleId = 1, Article = new Article { CategoryId = 1, Title = "T1", Content = "C1", Source = "S1", Url = "U1", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 } },
                new UserSavedArticle { UserId = userId, ArticleId = 2, Article = new Article { CategoryId = 2, Title = "T2", Content = "C2", Source = "S2", Url = "U2", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 } },
                new UserSavedArticle { UserId = 2, ArticleId = 3, Article = new Article { CategoryId = 3, Title = "T3", Content = "C3", Source = "S3", Url = "U3", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 } }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetCategoriesBySavedUserArticleAsync(userId);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
        }

        #endregion

        #region GetReportedArticleCategoriesByUserAsync Tests

        [TestMethod]
        public async Task GetReportedArticleCategoriesByUserAsync_ShouldReturnCategoryIds_WhenReportsExist()
        {
            var userId = 1;
            _dbContext.UserArticleReports.AddRange(new List<UserArticleReport>
            {
                new UserArticleReport { UserId = userId, ArticleId = 1, Article = new Article { CategoryId = 1, Title = "T1", Content = "C1", Source = "S1", Url = "U1", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 }, ReportReason = "Spam" },
                new UserArticleReport { UserId = userId, ArticleId = 2, Article = new Article { CategoryId = 2, Title = "T2", Content = "C2", Source = "S2", Url = "U2", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 }, ReportReason = "Spam" },
                new UserArticleReport { UserId = 2, ArticleId = 3, Article = new Article { CategoryId = 3, Title = "T3", Content = "C3", Source = "S3", Url = "U3", PublishedDate = DateTime.UtcNow, ExternalServerId = 1 }, ReportReason = "Spam" }
            });
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetReportedArticleCategoriesByUserAsync(userId);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
        }

        #endregion

        #region SaveChangesAsync Tests

        [TestMethod]
        public async Task SaveChangesAsync_ShouldCallDbContextSaveChanges()
        {
            await _repository.SaveChangesAsync();
            Assert.AreEqual(0, _dbContext.ChangeTracker.Entries().Count());
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ShouldCreateRepository_WhenValidContext()
        {
            var repository = new UserArticleActionRepository(_dbContext);
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserArticleActionRepository(null));
        }

        #endregion
    }
} 