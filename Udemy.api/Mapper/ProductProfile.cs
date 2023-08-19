using AutoMapper;
using Udemy.api.Models.Domain;
using Udemy.api.Models.Dto;

namespace Udemy.api.Mapper;
public class ProductProfile :Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, ProductResponse>().ReverseMap();
    }
}