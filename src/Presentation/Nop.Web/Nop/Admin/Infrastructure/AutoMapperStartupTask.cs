using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Tasks;
using Nop.Core.Domain.Directory;

namespace Nop.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //language
            ViceVersa<Language, LanguageModel>();
            //email account
            ViceVersa<EmailAccount, EmailAccountModel>();
            //category
            ViceVersa<Category, CategoryModel>();
            //category product
            ViceVersa<ProductCategory, CategoryProductModel>();
            //logs
            ViceVersa<Log, LogModel>();
            //currencies
            ViceVersa<Currency, CurrencyModel>();
            //locale resource
            Mapper.CreateMap<LocaleStringResource, LanguageResourceModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ResourceName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ResourceValue))
                .ForMember(dest => dest.LanguageName,
                           opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));
            Mapper.CreateMap<LanguageResourceModel, LocaleStringResource>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceValue, opt => opt.MapFrom(src => src.Value));
        }

        public static void ViceVersa<T1, T2>()
        {
            Mapper.CreateMap<T1, T2>();
            Mapper.CreateMap<T2, T1>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}