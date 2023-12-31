﻿using _7Colors.Models;
using _7Colors.ViewModels;

using AutoMapper;

namespace _7Colors.Profiles
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<Image, ImageViewModel>()
                .ReverseMap();
        }
    }
}
