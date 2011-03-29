using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Directory;
using System.Web.Mvc.Html;
using Nop.Web.Framework;
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

        #region Products

        public static ProductModel ToModel(this Product product)
        {
            return AutoMapper.Mapper.Map<Product, ProductModel>(product);
        }

        public static Product ToEntity(this ProductModel model)
        {
            return AutoMapper.Mapper.Map<ProductModel, Product>(model);
        }

        public static Product ToEntity(this ProductModel model, Product destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Product variants

        public static ProductVariantModel ToModel(this ProductVariant variant)
        {
            return AutoMapper.Mapper.Map<ProductVariant, ProductVariantModel>(variant);
        }

        public static ProductVariant ToEntity(this ProductVariantModel model)
        {
            return AutoMapper.Mapper.Map<ProductVariantModel, ProductVariant>(model);
        }

        public static ProductVariant ToEntity(this ProductVariantModel model, ProductVariant destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
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

    public static class HtmlExtensions
    {
        public static MvcHtmlString NopField<TModel>(this HtmlHelper<TModel> helper, 
            System.Linq.Expressions.Expression<Func<TModel, string>> expression)
        {
            return helper.NopCommonField(expression, (x => helper.TextBoxFor(x)));
        }

        private static MvcHtmlString NopCommonField<TModel, TValue>(this HtmlHelper<TModel> helper, 
            System.Linq.Expressions.Expression<Func<TModel, TValue>> expression,
            Func<System.Linq.Expressions.Expression<Func<TModel, TValue>>,MvcHtmlString> editor)
        {
            var sb = new StringBuilder();
            var tr = new TagBuilder("tr");

            sb.Append(tr.ToString(TagRenderMode.StartTag));

            var builder = new TagBuilder("td");
            builder.Attributes.Add("class", "adminTitle");
            sb.Append(builder.ToString(TagRenderMode.StartTag));
            sb.Append(helper.NopLabelFor(expression).ToHtmlString() + ":");
            sb.Append(builder.ToString(TagRenderMode.EndTag));

            builder = new TagBuilder("td");
            builder.Attributes.Add("class", "adminData");
            sb.Append(builder.ToString(TagRenderMode.StartTag));
            sb.Append(editor.Invoke(expression).ToHtmlString());
            sb.Append(helper.ValidationMessageFor(expression).ToHtmlString());
            sb.Append(builder.ToString(TagRenderMode.EndTag));

            sb.Append(tr.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}