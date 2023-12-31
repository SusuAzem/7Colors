using _7Colors.Models;
using _7Colors.ViewModels;

using AutoMapper;

namespace _7Colors.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
