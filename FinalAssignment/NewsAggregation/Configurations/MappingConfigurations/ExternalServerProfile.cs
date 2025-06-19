using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Configurations.MappingConfigurations
{
    public class ExternalServerProfile : Profile
    {
        public ExternalServerProfile()
        {
            CreateMap<ExternalServer, ExternalServerDto>().ReverseMap();
        }
    }
}
