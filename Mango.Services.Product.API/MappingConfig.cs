using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>()
                .ForMember(pDto => pDto.ProductId, opt => opt.MapFrom(p => p.ProductId))
                .ForMember(pDto => pDto.Name, opt => opt.MapFrom(p => p.Name))
                .ForMember(pDto => pDto.Price, opt => opt.MapFrom(p => p.Price))
                .ForMember(pDto => pDto.Description, opt => opt.MapFrom(p => p.Description))
                .ForMember(pDto => pDto.ImageUrl, opt => opt.MapFrom(p => p.ImageUrl))
                .ForMember(pDto => pDto.CatagoryName, opt => opt.MapFrom(p => p.CategoryName))
                .ReverseMap();
            });

            return mappingConfig;
        }
    }
}
