using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Tasks;
using Nop.Services.Localization;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
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
            Mapper.CreateMap<Language, LanguageModel>();
            //currency
            Mapper.CreateMap<Currency, CurrencyModel>();
            //product
            Mapper.CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)))
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src => src.GetLocalized(x => x.ShortDescription)))
                .ForMember(dest => dest.FullDescription, opt => opt.MapFrom(src => src.GetLocalized(x => x.FullDescription)))
                .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaKeywords)))
                .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaDescription)))
                .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaTitle)))
                .ForMember(dest => dest.SeName, opt => opt.MapFrom(src => src.GetSeName()));
            //category
            Mapper.CreateMap<Category, CategoryModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GetLocalized(x => x.Description)))
                .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaKeywords)))
                .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaDescription)))
                .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaTitle)))
                .ForMember(dest => dest.SeName, opt => opt.MapFrom(src => src.GetSeName()));
            Mapper.CreateMap<Category, CategoryModel.SubCategoryModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)))
                .ForMember(dest => dest.SeName, opt => opt.MapFrom(src => src.GetSeName()));
            //manufacturer
            Mapper.CreateMap<Manufacturer, ManufacturerModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetLocalized(x => x.Name)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GetLocalized(x => x.Description)))
                .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaKeywords)))
                .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaDescription)))
                .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.GetLocalized(x => x.MetaTitle)))
                .ForMember(dest => dest.SeName, opt => opt.MapFrom(src => src.GetSeName()));
            //address
            Mapper.CreateMap<Address, AddressModel>();
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore());

        }

        protected virtual void ViceVersa<T1, T2>()
        {
            Mapper.CreateMap<T1, T2>();
            Mapper.CreateMap<T2, T1>();
        }
        
		#endregion Methods 
    }
}