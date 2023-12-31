using _7Colors.Models;
using _7Colors.ViewModels;

using AutoMapper;

namespace _7Colors.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostViewModel>()
                .ReverseMap();
        }
    }
}
