using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Directory;

namespace Nop.Admin
{
    public static class MappingExtensions
    {
        #region Category

        public static CategoryModel ToModel(this Category category)
        {
            return AutoMapper.Mapper.Map<Category, CategoryModel>(category);
        }

        public static Category ToEntity(this CategoryModel model)
        {
            return AutoMapper.Mapper.Map<CategoryModel, Category>(model);
        }

        public static Category ToEntity(this CategoryModel model, Category destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Category product

        public static CategoryProductModel ToModel(this ProductCategory productCategory)
        {
            return AutoMapper.Mapper.Map<ProductCategory, CategoryProductModel>(productCategory);
        }

        public static ProductCategory ToEntity(this CategoryProductModel model)
        {
            return AutoMapper.Mapper.Map<CategoryProductModel, ProductCategory>(model);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language language)
        {
            return AutoMapper.Mapper.Map<Language, LanguageModel>(language);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return AutoMapper.Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #region Resources

        public static LanguageResourceModel ToModel(this LocaleStringResource localeStringResource)
        {
            return AutoMapper.Mapper.Map<LocaleStringResource, LanguageResourceModel>(localeStringResource);
        }

        public static LocaleStringResource ToEntity(this LanguageResourceModel model)
        {
            return AutoMapper.Mapper.Map<LanguageResourceModel, LocaleStringResource>(model);
        }

        public static LocaleStringResource ToEntity(this LanguageResourceModel model, LocaleStringResource destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount emailAccount)
        {
            return AutoMapper.Mapper.Map<EmailAccount, EmailAccountModel>(emailAccount);
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return AutoMapper.Mapper.Map<EmailAccountModel, EmailAccount>(model);
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Log

        public static LogModel ToModel(this Log logItem)
        {
            return AutoMapper.Mapper.Map<Log, LogModel>(logItem);
        }

        public static Log ToEntity(this LogModel model)
        {
            return AutoMapper.Mapper.Map<LogModel, Log>(model);
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Catalog

        public static TDestination To<TDestination>(this Product product)
        {
            return AutoMapper.Mapper.Map<Product, TDestination>(product);
        }

        //TODO:Make all mapping use To<T>()
        public static TDestination To<TDestination>(this Category category)
        {
            return AutoMapper.Mapper.Map<Category, TDestination>(category);
        }

        #endregion

        #region Currencies

        public static CurrencyModel ToModel(this Currency currency)
        {
            return AutoMapper.Mapper.Map<Currency, CurrencyModel>(currency);
        }

        public static Currency ToEntity(this CurrencyModel model)
        {
            return AutoMapper.Mapper.Map<CurrencyModel, Currency>(model);
        }

        public static Currency ToEntity(this CurrencyModel model, Currency destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }
        #endregion
    }
}