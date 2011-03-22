using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Infrastructure
{
    /// <summary>
    /// This class is used to create automapper mappings of specific types (mainly between models and entities).
    /// </summary>
    public static class AutoMapperInitialization
    {
		#region Methods (2) 

		// Public Methods (2) 

        public static void Initialize()
        {
            //language
            ViceVersa<Language, LanguageModel>();
            //email account
            ViceVersa<EmailAccount, EmailAccountModel>();
            //category
            ViceVersa<Category, CategoryModel>();
            //category product
            ViceVersa<ProductCategory, CategoryProductModel>();
            //locale resource
            AutoMapper.Mapper.CreateMap<LocaleStringResource, LanguageResourceModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ResourceName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ResourceValue))
                .ForMember(dest => dest.LanguageName,
                           opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));
            AutoMapper.Mapper.CreateMap<LanguageResourceModel, LocaleStringResource>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceValue, opt => opt.MapFrom(src => src.Value));
        }

        public static void ViceVersa<T1, T2>()
        {
            AutoMapper.Mapper.CreateMap<T1, T2>();
            AutoMapper.Mapper.CreateMap<T2, T1>();
        }

		#endregion Methods 
    }
}