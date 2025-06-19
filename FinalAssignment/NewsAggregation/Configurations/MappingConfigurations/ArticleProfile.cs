using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Enums;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.LikeCount,
                opt => opt.MapFrom(src => src.UserArticleReactions.Count(r => r.Reaction == ReactionEnum.Like)))
            .ForMember(dest => dest.DislikeCount,
                opt => opt.MapFrom(src => src.UserArticleReactions.Count(r => r.Reaction == ReactionEnum.Dislike)));

        CreateMap<ArticleDto, Article>();
    }
}
