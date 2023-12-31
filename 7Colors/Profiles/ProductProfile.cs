using _7Colors.Models;
using _7Colors.ViewModels;

using AutoMapper;

namespace _7Colors.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductItemViewModel>()
            .ReverseMap();            
        }
    }
}
