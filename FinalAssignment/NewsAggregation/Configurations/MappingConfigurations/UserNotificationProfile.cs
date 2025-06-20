using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Configurations.MappingConfigurations
{
    public class UserNotificationProfile : Profile
    {
        public UserNotificationProfile()
        {
            CreateMap<UserNotification, UserNotificationDto>().ReverseMap();
        }
    }
}
