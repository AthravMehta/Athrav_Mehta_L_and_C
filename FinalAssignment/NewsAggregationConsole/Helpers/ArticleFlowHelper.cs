using NewsAggregationConsole.Enums;
using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Helpers
{
    public static class ArticleFlowHelper
    {
        public static async Task ShowArticlesPage(
            List<ArticleDto> articles,
            UserReadDto user,
            ArticleService articleService,
            bool allowSave = true
        )
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Articles Found: {articles.Count}\n");
                Console.WriteLine("ID | Headline");
                Console.WriteLine("------------------------------");

                if (!articles.Any())
                {
                    Console.WriteLine("No Articles Found!!");
                    Console.WriteLine("Press Enter to go back...");
                    Console.ReadLine();
                    return;
                }

                foreach (var article in articles)
                {
                    Console.WriteLine($"{article.ArticleId} | {article.Title}");
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. View Article Details by ID");
                Console.WriteLine("2. Back");
                Console.WriteLine("3. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                if (choice == 1)
                {
                    int articleId = InputHelper.GetInt("Enter Article ID: ");
                    var selectedArticle = await articleService.GetArticleByIdAsync(articleId);

                    if (selectedArticle == null)
                    {
                        Console.WriteLine("Invalid Article ID. Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    await ShowArticleDetails(selectedArticle, user, articleService, allowSave);
                }
                else if (choice == 2)
                {
                    return;
                }
                else if (choice == 3)
                {
                    Environment.Exit(0);
                }
            }
        }

        public static async Task ShowArticleDetails(
    ArticleDetailsDto article,
    UserReadDto user,
    ArticleService articleService,
    bool allowSave = true
)
        {
            while (true)
            {
                Console.Clear();
                DisplayHelper.DisplayArticleDetails(article);

                Console.WriteLine("\nYour Status:");
                Console.WriteLine($"- Saved: {(article.IsSavedByUser ? "Yes" : "No")}");
                Console.WriteLine($"- Reported: {(article.IsReportedByUser ? "Yes" : "No")}");
                Console.WriteLine($"- Your reaction: {article.UserReaction?.ToString() ?? "None"}");

                Console.WriteLine("\nOptions:");
                int opt = 1;
                if (allowSave)
                {
                    Console.WriteLine($"{opt++}. Save Article");
                }
                Console.WriteLine($"{opt++}. Like Article");
                Console.WriteLine($"{opt++}. Dislike Article");
                Console.WriteLine($"{opt++}. Report Article");
                Console.WriteLine($"{opt++}. Back");
                Console.WriteLine($"{opt++}. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, opt - 1);

                int idx = 1;
                switch (choice)
                {
                    case var c when allowSave && c == idx++:
                        if (article.IsSavedByUser)
                        {
                            Console.WriteLine("You have already saved this article.");
                        }
                        else
                        {
                            ToggleSaveResponseDto saved = await articleService.SaveArticleForUserAsync(article.ArticleId);
                            Console.WriteLine(saved.Message);
                        }
                        break;

                    case var c when c == idx++:
                        await articleService.AddArticleReactionAsync(new ArticleReactionRequestDto
                        {
                            ArticleId = article.ArticleId,
                            ArticleReaction = ReactionEnum.Like
                        });
                        Console.WriteLine("You liked the article!");
                        break;

                    case var c when c == idx++:
                        await articleService.AddArticleReactionAsync(new ArticleReactionRequestDto
                        {
                            ArticleId = article.ArticleId,
                            ArticleReaction = ReactionEnum.Dislike
                        });
                        Console.WriteLine("You disliked the article!");
                        break;

                    case var c when c == idx++:
                        var reportReason = InputHelper.GetString("Enter report reason: ");
                        UserArticleReportResponseDto result = await articleService.ReportArticleAsync(
                            article.ArticleId,
                            reportReason
                        );
                        Console.WriteLine(result.Message);
                        break;

                    case var c when c == idx++:
                        return;

                    case var c when c == idx:
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Press Enter to continue...");
                        break;
                }

                article = await articleService.GetArticleByIdAsync(article.ArticleId);

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

    }
}
