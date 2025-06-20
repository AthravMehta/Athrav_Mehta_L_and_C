using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Configurations.MappingConfigurations
{
    public class UserNotificationConfiurationProfile : Profile
    {
        public UserNotificationConfiurationProfile()
        {

            CreateMap<UserNotificationConfiguration, UserNotificationConfigurationDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ReverseMap();
        }
    }
}
