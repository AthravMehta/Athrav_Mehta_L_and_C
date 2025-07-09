using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Configurations.MappingConfigurations
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
