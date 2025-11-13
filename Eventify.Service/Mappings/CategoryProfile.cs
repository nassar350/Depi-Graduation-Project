using AutoMapper;
using Eventify.Service.DTOs.Categories;
using Eventify.Core.Entities;

namespace Eventify.Service.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.Booked, opt => opt.MapFrom(_ => 0));

            CreateMap<UpdateCategoryDto, Category>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Category, CategoryDetailsDto>();
        }
    }

}
