using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>().ReverseMap();
    }
}
