using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Orders;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    [Validator(typeof(CheckoutAttributeValidator))]
    public partial class CheckoutAttributeModel : BaseNopEntityModel, ILocalizedModel<CheckoutAttributeLocalizedModel>
    {
        public CheckoutAttributeModel()
        {
            Locales = new List<CheckoutAttributeLocalizedModel>();
            AvailableTaxCategories = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TextPrompt")]
        public string TextPrompt { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.ShippableProductRequired")]
        public bool ShippableProductRequired { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.AttributeControlType")]
        public string AttributeControlTypeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.MinLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMinLength { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.MaxLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMaxLength { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileAllowedExtensions")]
        public string ValidationFileAllowedExtensions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileMaximumSize")]
        [UIHint("Int32Nullable")]
        public int? ValidationFileMaximumSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.DefaultValue")]
        public string DefaultValue { get; set; }

        public IList<CheckoutAttributeLocalizedModel> Locales { get; set; }

        //condition
        public bool ConditionAllowed { get; set; }
        public ConditionModel ConditionModel { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }

    public partial class ConditionModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Condition.EnableCondition")]
        public bool EnableCondition { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Condition.Attributes")]
        public int SelectedAttributeId { get; set; }

        public IList<AttributeConditionModel> ConditionAttributes { get; set; }
    }

    public partial class AttributeConditionModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<SelectListItem> Values { get; set; }

        public string SelectedValueId { get; set; }
    }

    public partial class CheckoutAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TextPrompt")]
        public string TextPrompt { get; set; }
    }
}