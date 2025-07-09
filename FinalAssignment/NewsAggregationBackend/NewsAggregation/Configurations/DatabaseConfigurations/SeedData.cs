using NewsAggregation.Entities;

namespace NewsAggregation.Configurations.DatabaseConfigurations
{
    public static class SeedData
    {
        public static List<Category> GetCategories()
        {
            var now = new DateTime(2025, 6, 27, 11, 0, 0, DateTimeKind.Utc);
            const string systemUser = "System";

            return new List<Category>
            {
                new Category
                {
                    CategoryId = 1,
                    Name = "All",
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                },
                new Category
                {
                    CategoryId = 2,
                    Name = "Technology",
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                },
                new Category
                {
                    CategoryId = 3,
                    Name = "Health",
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                },
                new Category
                {
                    CategoryId = 4,
                    Name = "Sports",
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                },
                new Category
                {
                    CategoryId = 5,
                    Name = "Business",
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                },
                new Category
                {
                    CategoryId = 6,
                    Name = "Entertainment",
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                }
            };
        }

        public static List<Keywords> GetKeywords()
        {
            var now = new DateTime(2025, 6, 27, 11, 0, 0, DateTimeKind.Utc);
            const string systemUser = "System";
            var keywords = new List<Keywords>();
            int keywordId = 1;

            var keywordsByCategory = new Dictionary<string, List<string>>
            {
                ["Technology"] = new List<string> { "AI", "Blockchain", "Cloud", "Cybersecurity", "Data Science", "DevOps", "Gadgets", "Innovation", "Programming", "Robotics", "Software", "Tech News", "Virtual Reality", "Web Development", "5G", "Startups", "Mobile", "Hardware", "Networking", "AI Ethics" },
                ["Health"] = new List<string> { "Nutrition", "Mental Health", "Diseases", "Fitness", "Healthcare", "Medicine", "Wellness", "Yoga", "Pharmacy", "Public Health", "Surgery", "Vaccines", "Allergies", "Cardiology", "Dermatology", "Epidemiology", "Neurology", "Pediatrics", "Psychology", "Rehabilitation" },
                ["Sports"] = new List<string> { "Football", "Basketball", "Tennis", "Cricket", "Olympics", "Athletics", "Baseball", "Golf", "Hockey", "Rugby", "Swimming", "Volleyball", "Wrestling", "Boxing", "Motorsport", "Cycling", "Gymnastics", "Skiing", "Surfing", "Skateboarding" },
                ["Business"] = new List<string> { "Economy", "Finance", "Markets", "Investing", "Startups", "Real Estate", "Banking", "Business News", "Entrepreneurship", "Trade", "Taxes", "Insurance", "Retail", "Management", "Marketing", "Sales", "Strategy", "Supply Chain", "Technology", "Leadership" },
                ["Entertainment"] = new List<string> { "Movies", "Music", "TV Shows", "Celebrities", "Awards", "Theater", "Streaming", "Pop Culture", "Concerts", "Festivals", "Documentaries", "Animation", "Comedy", "Drama", "Fashion", "Gaming", "Hollywood", "Bollywood", "Reviews", "Interviews" }
            };

            var categoryNameToId = new Dictionary<string, int>
            {
                ["Technology"] = 2,
                ["Health"] = 3,
                ["Sports"] = 4,
                ["Business"] = 5,
                ["Entertainment"] = 6
            };

            foreach (var kvp in keywordsByCategory)
            {
                var categoryName = kvp.Key;
                var categoryId = categoryNameToId[categoryName];

                foreach (var kw in kvp.Value)
                {
                    keywords.Add(new Keywords
                    {
                        KeywordId = keywordId++,
                        Keyword = kw,
                        CategoryId = categoryId,
                        CreatedDateTime = now,
                        ModifiedDateTime = now,
                        CreatedBy = systemUser,
                        ModifiedBy = systemUser
                    });
                }
            }

            return keywords;
        }
    }
}
