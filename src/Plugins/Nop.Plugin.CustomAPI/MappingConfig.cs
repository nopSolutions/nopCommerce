using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.CustomAPI.Models.DTO;
using Nop.Web.Models.ShoppingCart;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ShoppingCartModel, ShoppingCartDTO>().ReverseMap();
        }
    }
}
