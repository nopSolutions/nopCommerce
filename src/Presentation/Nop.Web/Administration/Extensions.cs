using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework;


namespace Nop.Admin
{
    //TODO separate MappingExtensions and HtmlExtensions to two distinct .cs files
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

        #region Product attributes

        public static ProductAttributeModel ToModel(this ProductAttribute entity)
        {
            return AutoMapper.Mapper.Map<ProductAttribute, ProductAttributeModel>(entity);
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model)
        {
            return AutoMapper.Mapper.Map<ProductAttributeModel, ProductAttribute>(model);
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model, ProductAttribute destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Specification attributes

        //attributes
        public static SpecificationAttributeModel ToModel(this SpecificationAttribute entity)
        {
            return AutoMapper.Mapper.Map<SpecificationAttribute, SpecificationAttributeModel>(entity);
        }

        public static SpecificationAttribute ToEntity(this SpecificationAttributeModel model)
        {
            return AutoMapper.Mapper.Map<SpecificationAttributeModel, SpecificationAttribute>(model);
        }

        public static SpecificationAttribute ToEntity(this SpecificationAttributeModel model, SpecificationAttribute destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        //attribute options
        public static SpecificationAttributeOptionModel ToModel(this SpecificationAttributeOption entity)
        {
            return AutoMapper.Mapper.Map<SpecificationAttributeOption, SpecificationAttributeOptionModel>(entity);
        }

        public static SpecificationAttributeOption ToEntity(this SpecificationAttributeOptionModel model)
        {
            return AutoMapper.Mapper.Map<SpecificationAttributeOptionModel, SpecificationAttributeOption>(model);
        }

        public static SpecificationAttributeOption ToEntity(this SpecificationAttributeOptionModel model, SpecificationAttributeOption destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }
        #endregion

        #region Checkout attributes

        //attributes
        public static CheckoutAttributeModel ToModel(this CheckoutAttribute entity)
        {
            return AutoMapper.Mapper.Map<CheckoutAttribute, CheckoutAttributeModel>(entity);
        }

        public static CheckoutAttribute ToEntity(this CheckoutAttributeModel model)
        {
            return AutoMapper.Mapper.Map<CheckoutAttributeModel, CheckoutAttribute>(model);
        }

        public static CheckoutAttribute ToEntity(this CheckoutAttributeModel model, CheckoutAttribute destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        //checkout attribute values
        public static CheckoutAttributeValueModel ToModel(this CheckoutAttributeValue entity)
        {
            return AutoMapper.Mapper.Map<CheckoutAttributeValue, CheckoutAttributeValueModel>(entity);
        }

        public static CheckoutAttributeValue ToEntity(this CheckoutAttributeValueModel model)
        {
            return AutoMapper.Mapper.Map<CheckoutAttributeValueModel, CheckoutAttributeValue>(model);
        }

        public static CheckoutAttributeValue ToEntity(this CheckoutAttributeValueModel model, CheckoutAttributeValue destination)
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

        #region Shipping rate computation method

        public static ShippingRateComputationMethodModel ToModel(this IShippingRateComputationMethod entity)
        {
            return AutoMapper.Mapper.Map<IShippingRateComputationMethod, ShippingRateComputationMethodModel>(entity);
        }

        #endregion

        #region Shipping methods

        public static ShippingMethodModel ToModel(this ShippingMethod entity)
        {
            return AutoMapper.Mapper.Map<ShippingMethod, ShippingMethodModel>(entity);
        }

        public static ShippingMethod ToEntity(this ShippingMethodModel model)
        {
            return AutoMapper.Mapper.Map<ShippingMethodModel, ShippingMethod>(model);
        }

        public static ShippingMethod ToEntity(this ShippingMethodModel model, ShippingMethod destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Shipping settings

        public static ShippingSettingsModel ToModel(this ShippingSettings entity)
        {
            return AutoMapper.Mapper.Map<ShippingSettings, ShippingSettingsModel>(entity);
        }

        public static ShippingSettings ToEntity(this ShippingSettingsModel model)
        {
            return AutoMapper.Mapper.Map<ShippingSettingsModel, ShippingSettings>(model);
        }

        public static ShippingSettings ToEntity(this ShippingSettingsModel model, ShippingSettings destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Payment methods

        public static PaymentMethodModel ToModel(this IPaymentMethod entity)
        {
            return AutoMapper.Mapper.Map<IPaymentMethod, PaymentMethodModel>(entity);
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return AutoMapper.Mapper.Map<Address, AddressModel>(entity);
        }

        public static Address ToEntity(this AddressModel model)
        {
            return AutoMapper.Mapper.Map<AddressModel, Address>(model);
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region NewsLetter subscriptions

        public static NewsLetterSubscriptionModel ToModel(this NewsLetterSubscription entity)
        {
            return AutoMapper.Mapper.Map<NewsLetterSubscription, NewsLetterSubscriptionModel>(entity);
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model)
        {
            return AutoMapper.Mapper.Map<NewsLetterSubscriptionModel, NewsLetterSubscription>(model);
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model, NewsLetterSubscription destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        #endregion

        #region Forums
        //forum settings
        public static ForumSettingsModel ToModel(this ForumSettings entity)
        {
            return AutoMapper.Mapper.Map<ForumSettings, ForumSettingsModel>(entity);
        }

        public static ForumSettings ToEntity(this ForumSettingsModel model)
        {
            return AutoMapper.Mapper.Map<ForumSettingsModel, ForumSettings>(model);
        }

        public static ForumSettings ToEntity(this ForumSettingsModel model, ForumSettings destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }
        //forum groups
        public static ForumGroupModel ToModel(this ForumGroup entity)
        {
            return AutoMapper.Mapper.Map<ForumGroup, ForumGroupModel>(entity);
        }

        public static ForumGroup ToEntity(this ForumGroupModel model)
        {
            return AutoMapper.Mapper.Map<ForumGroupModel, ForumGroup>(model);
        }

        public static ForumGroup ToEntity(this ForumGroupModel model, ForumGroup destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }
        //forums
        public static ForumModel ToModel(this Forum entity)
        {
            return AutoMapper.Mapper.Map<Forum, ForumModel>(entity);
        }

        public static Forum ToEntity(this ForumModel model)
        {
            return AutoMapper.Mapper.Map<ForumModel, Forum>(model);
        }

        public static Forum ToEntity(this ForumModel model, Forum destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }
        #endregion

        #region Customers/users/customer roles
        //customer roles
        public static CustomerRoleModel ToModel(this CustomerRole entity)
        {
            return AutoMapper.Mapper.Map<CustomerRole, CustomerRoleModel>(entity);
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model)
        {
            return AutoMapper.Mapper.Map<CustomerRoleModel, CustomerRole>(model);
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model, CustomerRole destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }

        //customers
        public static CustomerModel ToModel(this Customer entity)
        {
            return AutoMapper.Mapper.Map<Customer, CustomerModel>(entity);
        }

        public static Customer ToEntity(this CustomerModel model)
        {
            return AutoMapper.Mapper.Map<CustomerModel, Customer>(model);
        }

        public static Customer ToEntity(this CustomerModel model, Customer destination)
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