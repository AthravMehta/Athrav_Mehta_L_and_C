using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsAggregation.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalServers",
                columns: table => new
                {
                    ExternalServerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ApiKeyHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalServers", x => x.ExternalServerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastLoginDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    KeywordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.KeywordId);
                    table.ForeignKey(
                        name: "FK_Keywords_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    HideReason = table.Column<int>(type: "int", nullable: false),
                    ExternalServerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ArticleId);
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_ExternalServers_ExternalServerId",
                        column: x => x.ExternalServerId,
                        principalTable: "ExternalServers",
                        principalColumn: "ExternalServerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserKeywords",
                columns: table => new
                {
                    UserKeywordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeywords", x => x.UserKeywordId);
                    table.ForeignKey(
                        name: "FK_UserKeywords_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserKeywords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationConfigurations",
                columns: table => new
                {
                    UserNotificationConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationConfigurations", x => x.UserNotificationConfigurationId);
                    table.ForeignKey(
                        name: "FK_UserNotificationConfigurations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotificationConfigurations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserArticleReactions",
                columns: table => new
                {
                    UserArticleReactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reaction = table.Column<int>(type: "int", nullable: false),
                    ActionCreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArticleReactions", x => x.UserArticleReactionId);
                    table.ForeignKey(
                        name: "FK_UserArticleReactions_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserArticleReactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserArticleReports",
                columns: table => new
                {
                    UserArticleReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionCreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArticleReports", x => x.UserArticleReportId);
                    table.ForeignKey(
                        name: "FK_UserArticleReports_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserArticleReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    UserNotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.UserNotificationId);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSavedArticles",
                columns: table => new
                {
                    UserSavedArticleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionCreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedArticles", x => x.UserSavedArticleId);
                    table.ForeignKey(
                        name: "FK_UserSavedArticles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedArticles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedBy", "CreatedDateTime", "ModifiedBy", "ModifiedDateTime", "Name" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "All" },
                    { 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Technology" },
                    { 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Health" },
                    { 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Sports" },
                    { 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Business" },
                    { 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Entertainment" }
                });

            migrationBuilder.InsertData(
                table: "Keywords",
                columns: new[] { "KeywordId", "CategoryId", "CreatedBy", "CreatedDateTime", "Keyword", "ModifiedBy", "ModifiedDateTime" },
                values: new object[,]
                {
                    { 1, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "AI", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Blockchain", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Cloud", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Cybersecurity", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Data Science", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "DevOps", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Gadgets", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Innovation", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Programming", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Robotics", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Software", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Tech News", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Virtual Reality", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Web Development", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "5G", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Startups", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Mobile", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Hardware", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Networking", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 2, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "AI Ethics", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Nutrition", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Mental Health", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Diseases", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Fitness", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Healthcare", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Medicine", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Wellness", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Yoga", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Pharmacy", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Public Health", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Surgery", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Vaccines", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Allergies", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Cardiology", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Dermatology", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 36, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Epidemiology", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 37, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Neurology", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 38, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Pediatrics", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 39, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Psychology", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 40, 3, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Rehabilitation", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 41, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Football", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 42, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Basketball", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 43, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Tennis", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 44, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Cricket", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 45, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Olympics", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 46, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Athletics", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 47, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Baseball", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 48, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Golf", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 49, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Hockey", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 50, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Rugby", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 51, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Swimming", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 52, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Volleyball", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 53, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Wrestling", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 54, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Boxing", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 55, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Motorsport", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 56, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Cycling", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 57, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Gymnastics", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 58, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Skiing", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 59, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Surfing", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 60, 4, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Skateboarding", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 61, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Economy", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 62, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Finance", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 63, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Markets", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 64, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Investing", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 65, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Startups", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 66, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Real Estate", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 67, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Banking", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 68, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Business News", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 69, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Entrepreneurship", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 70, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Trade", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 71, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Taxes", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 72, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Insurance", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 73, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Retail", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 74, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Management", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 75, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Marketing", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 76, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Sales", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 77, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Strategy", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 78, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Supply Chain", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 79, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Technology", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 80, 5, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Leadership", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 81, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Movies", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 82, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Music", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 83, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "TV Shows", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 84, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Celebrities", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 85, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Awards", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 86, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Theater", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 87, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Streaming", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 88, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Pop Culture", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 89, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Concerts", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 90, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Festivals", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 91, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Documentaries", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 92, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Animation", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 93, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Comedy", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 94, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Drama", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 95, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Fashion", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 96, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Gaming", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 97, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Hollywood", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 98, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Bollywood", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 99, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Reviews", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 100, 6, "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc), "Interviews", "System", new DateTime(2025, 6, 27, 11, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ExternalServerId",
                table: "Articles",
                column: "ExternalServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_CategoryId",
                table: "Keywords",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleReactions_ArticleId",
                table: "UserArticleReactions",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleReactions_UserId",
                table: "UserArticleReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleReports_ArticleId",
                table: "UserArticleReports",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleReports_UserId",
                table: "UserArticleReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKeywords_CategoryId",
                table: "UserKeywords",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKeywords_UserId",
                table: "UserKeywords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationConfigurations_CategoryId",
                table: "UserNotificationConfigurations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationConfigurations_UserId",
                table: "UserNotificationConfigurations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ArticleId",
                table: "UserNotifications",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedArticles_ArticleId",
                table: "UserSavedArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedArticles_UserId",
                table: "UserSavedArticles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "UserArticleReactions");

            migrationBuilder.DropTable(
                name: "UserArticleReports");

            migrationBuilder.DropTable(
                name: "UserKeywords");

            migrationBuilder.DropTable(
                name: "UserNotificationConfigurations");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "UserSavedArticles");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ExternalServers");
        }
    }
}
