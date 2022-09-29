using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
