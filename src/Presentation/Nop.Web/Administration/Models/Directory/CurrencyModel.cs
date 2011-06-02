using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Directory
{
    [Validator(typeof(CurrencyValidator))]
    public class CurrencyModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CurrencyCode")]
        [AllowHtml]
        public string CurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.DisplayLocale")]
        [AllowHtml]
        public string DisplayLocale { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.Rate")]
        public decimal Rate { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CustomFormatting")]
        [AllowHtml]
        public string CustomFormatting { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CreatedOn")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.UpdatedOn")]
        public DateTime UpdatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
        public bool IsPrimaryExchangeRateCurrency { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.IsPrimaryStoreCurrency")]
        public bool IsPrimaryStoreCurrency { get; set; }
    }
}