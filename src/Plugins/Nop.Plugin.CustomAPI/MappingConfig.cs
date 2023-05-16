using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.CustomAPI.Models.DTO;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();

        }
    }
}
