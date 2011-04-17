using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.Directory;

namespace Nop.Admin.Models
{
    public class CurrencyModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CurrencyCode")]
        public string CurrencyCode { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CurrencyCulture")]
        public string DisplayLocale { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.Rate")]
        public decimal Rate { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.CustomFormatting")]
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
        public string CreatedOnStr { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.UpdatedOn")]
        public string UpdatedOnStr { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
        public bool IsPrimaryExchangeRateCurrency { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Location.Currencies.Fields.IsPrimaryStoreCurrency")]
        public bool IsPrimaryStoreCurrency { get; set; }
    }
}