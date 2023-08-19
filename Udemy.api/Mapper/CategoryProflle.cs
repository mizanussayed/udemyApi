using AutoMapper;
using Udemy.api.Models.Domain;
using Udemy.api.Models.Dto;
namespace Udemy.api.Mapper;
public class CategoryProflle:Profile
{
    public CategoryProflle()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();
    }
}