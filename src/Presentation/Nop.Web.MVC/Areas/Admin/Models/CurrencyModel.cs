using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Core.Domain.Directory;
using Nop.Web.Framework;
using Nop.Web.MVC.Areas.Admin.Validators;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    [Validator(typeof(CurrencyValidator))]
    public class CurrencyModel : BaseNopEntityModel
    {
        public CurrencyModel()
        {
        }

        public CurrencyModel(Currency currency)
            : this()
        {
            Id = currency.Id;
            Name = currency.Name;
            CurrencyCode = currency.CurrencyCode;
            DisplayLocale = currency.DisplayLocale;
            Rate = currency.Rate;
            CustomFormatting = currency.CustomFormatting;
            Published = currency.Published;
            DisplayOrder = currency.DisplayOrder;
            CreatedOnUtc = currency.CreatedOnUtc;
            UpdatedOnUtc = currency.UpdatedOnUtc;
        }
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
    }
}