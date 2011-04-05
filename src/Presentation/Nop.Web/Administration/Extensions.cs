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
using Nop.Core.Domain.Tax;
using Nop.Services.Tax;
using Nop.Web.Framework;
namespace Nop.Admin
{
    public static class MappingExtensions
    {
        #region Category

        public static CategoryModel ToModel(this Category entity)
        {
            return AutoMapper.Mapper.Map<Category, CategoryModel>(entity);
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

        public static CategoryProductModel ToModel(this ProductCategory entity)
        {
            return AutoMapper.Mapper.Map<ProductCategory, CategoryProductModel>(entity);
        }

        public static ProductCategory ToEntity(this CategoryProductModel model)
        {
            return AutoMapper.Mapper.Map<CategoryProductModel, ProductCategory>(model);
        }

        #endregion

        #region Products

        public static ProductModel ToModel(this Product entity)
        {
            return AutoMapper.Mapper.Map<Product, ProductModel>(entity);
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

        public static ProductVariantModel ToModel(this ProductVariant entity)
        {
            return AutoMapper.Mapper.Map<ProductVariant, ProductVariantModel>(entity);
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

        public static LanguageModel ToModel(this Language entity)
        {
            return AutoMapper.Mapper.Map<Language, LanguageModel>(entity);
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

        public static LanguageResourceModel ToModel(this LocaleStringResource entity)
        {
            return AutoMapper.Mapper.Map<LocaleStringResource, LanguageResourceModel>(entity);
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

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return AutoMapper.Mapper.Map<EmailAccount, EmailAccountModel>(entity);
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

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return AutoMapper.Mapper.Map<QueuedEmail, QueuedEmailModel>(entity);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return AutoMapper.Mapper.Map<QueuedEmailModel, QueuedEmail>(model);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion
        
        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return AutoMapper.Mapper.Map<Log, LogModel>(entity);
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

        public static TDestination To<TDestination>(this Product entity)
        {
            return AutoMapper.Mapper.Map<Product, TDestination>(entity);
        }

        //TODO:Make all mapping use To<T>()
        public static TDestination To<TDestination>(this Category category)
        {
            return AutoMapper.Mapper.Map<Category, TDestination>(category);
        }

        #endregion

        #region Currencies

        public static CurrencyModel ToModel(this Currency entity)
        {
            return AutoMapper.Mapper.Map<Currency, CurrencyModel>(entity);
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

        #region Measure weights

        public static MeasureWeightModel ToModel(this MeasureWeight entity)
        {
            return AutoMapper.Mapper.Map<MeasureWeight, MeasureWeightModel>(entity);
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model)
        {
            return AutoMapper.Mapper.Map<MeasureWeightModel, MeasureWeight>(model);
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model, MeasureWeight destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Measure dimension

        public static MeasureDimensionModel ToModel(this MeasureDimension entity)
        {
            return AutoMapper.Mapper.Map<MeasureDimension, MeasureDimensionModel>(entity);
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model)
        {
            return AutoMapper.Mapper.Map<MeasureDimensionModel, MeasureDimension>(model);
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model, MeasureDimension destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Tax providers

        public static TaxProviderModel ToModel(this ITaxProvider entity)
        {
            return AutoMapper.Mapper.Map<ITaxProvider, TaxProviderModel>(entity);
        }

        #endregion

        #region Tax categories

        public static TaxCategoryModel ToModel(this TaxCategory entity)
        {
            return AutoMapper.Mapper.Map<TaxCategory, TaxCategoryModel>(entity);
        }

        public static TaxCategory ToEntity(this TaxCategoryModel model)
        {
            return AutoMapper.Mapper.Map<TaxCategoryModel, TaxCategory>(model);
        }

        public static TaxCategory ToEntity(this TaxCategoryModel model, TaxCategory destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Tax settings

        public static TaxSettingsModel ToModel(this TaxSettings entity)
        {
            return AutoMapper.Mapper.Map<TaxSettings, TaxSettingsModel>(entity);
        }

        public static TaxSettings ToEntity(this TaxSettingsModel model)
        {
            return AutoMapper.Mapper.Map<TaxSettingsModel, TaxSettings>(model);
        }

        public static TaxSettings ToEntity(this TaxSettingsModel model, TaxSettings destination)
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