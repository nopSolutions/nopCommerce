using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Directory
{
    [Validator(typeof(CurrencyValidator))]
    public partial class CurrencyModel : BaseNopEntityModel, ILocalizedModel<CurrencyLocalizedModel>
    {
        public CurrencyModel()
        {
            Locales = new List<CurrencyLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyCode")]
        [AllowHtml]
        public string CurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayLocale")]
        [AllowHtml]
        public string DisplayLocale { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Rate")]
        public decimal Rate { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CustomFormatting")]
        [AllowHtml]
        public string CustomFormatting { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
        public bool IsPrimaryExchangeRateCurrency { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryStoreCurrency")]
        public bool IsPrimaryStoreCurrency { get; set; }

        public IList<CurrencyLocalizedModel> Locales { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }

    public partial class CurrencyLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}