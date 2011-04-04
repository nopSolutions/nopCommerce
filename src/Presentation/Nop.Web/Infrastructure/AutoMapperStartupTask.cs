using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Tasks;
using Nop.Services.Localization;
using Nop.Web.Models;
using Nop.Web.Models.Home;
namespace Nop.Web.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
		#region Properties 

        public int Order
        {
            get { return 0; }
        }

		#endregion Properties 

		#region Methods 

        public void Execute()
        {
            //language
            ViceVersa<Language, LanguageModel>();
            //catalog product
            Mapper.CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)))
                .ForMember(dest => dest.ShortDescription,
                           opt => opt.MapFrom(src => src.GetLocalized(x => x.ShortDescription)))
                .ForMember(dest => dest.SeName, opt => opt.MapFrom(src => src.GetSeName()));
            //catalog category
            Mapper.CreateMap<Category, CategoryModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)));
            Mapper.CreateMap<Category, CategoryModel.SubCategoryModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)))
                .ForMember(dest => dest.SeName, opt => opt.MapFrom(src => src.GetSeName()));
        }

        public static void ViceVersa<T1, T2>()
        {
            Mapper.CreateMap<T1, T2>();
            Mapper.CreateMap<T2, T1>();
        }
        
		#endregion Methods 
    }
}