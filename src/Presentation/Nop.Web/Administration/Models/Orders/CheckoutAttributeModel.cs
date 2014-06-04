using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Stores;
using Nop.Admin.Validators.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    [Validator(typeof(CheckoutAttributeValidator))]
    public partial class CheckoutAttributeModel : BaseNopEntityModel, ILocalizedModel<CheckoutAttributeLocalizedModel>
    {
        public CheckoutAttributeModel()
        {
            Locales = new List<CheckoutAttributeLocalizedModel>();
            AvailableTaxCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TextPrompt")]
        [AllowHtml]
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
        [AllowHtml]
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


        //Store mapping
        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }

    }

    public partial class CheckoutAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }

    }
}