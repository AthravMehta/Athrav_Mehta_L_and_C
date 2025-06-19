using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserReadDto>().ReverseMap();
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
    }
}
