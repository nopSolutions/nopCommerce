using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class TaxSettingsModel
    {
        public TaxSettingsModel()
        {
            PaymentMethodAdditionalFeeTaxCategories = new List<SelectListItem>();
            ShippingTaxCategories = new List<SelectListItem>();
            EuVatShopCountries = new List<SelectListItem>();
            DefaultTaxAddress = new AddressModel();
        }

        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.PricesIncludeTax")]
        public bool PricesIncludeTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.AllowCustomersToSelectTaxDisplayType")]
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.TaxDisplayType")]
        public TaxDisplayType TaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.DisplayTaxSuffix")]
        public bool DisplayTaxSuffix { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.DisplayTaxRates")]
        public bool DisplayTaxRates { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.HideZeroTax")]
        public bool HideZeroTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.HideTaxInOrderSummary")]
        public bool HideTaxInOrderSummary { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.TaxBasedOn")]
        public TaxBasedOn TaxBasedOn { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.DefaultTaxAddress")]
        public AddressModel DefaultTaxAddress { get; set; }
       
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.ShippingIsTaxable")]
        public bool ShippingIsTaxable { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.ShippingPriceIncludesTax")]
        public bool ShippingPriceIncludesTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.ShippingTaxClass")]
        public int ShippingTaxClassId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.PaymentMethodAdditionalFeeIsTaxable")]
        public bool PaymentMethodAdditionalFeeIsTaxable { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.PaymentMethodAdditionalFeeIncludesTax")]
        public bool PaymentMethodAdditionalFeeIncludesTax { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.PaymentMethodAdditionalFeeTaxClass")]
        public int PaymentMethodAdditionalFeeTaxClassId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.EuVatEnabled")]
        public bool EuVatEnabled { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.EuVatShopCountry")]
        public int EuVatShopCountryId { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.EuVatAllowVatExemption")]
        public bool EuVatAllowVatExemption { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.EuVatUseWebService")]
        public bool EuVatUseWebService { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Tax.Settings.Fields.EuVatEmailAdminWhenNewVatSubmitted")]
        public bool EuVatEmailAdminWhenNewVatSubmitted { get; set; }

        public SelectList TaxDisplayTypeValues { get; set; }
        public SelectList TaxBasedOnValues { get; set; }
        public IList<SelectListItem> PaymentMethodAdditionalFeeTaxCategories { get; set; }
        public IList<SelectListItem> ShippingTaxCategories { get; set; }
        public IList<SelectListItem> EuVatShopCountries { get; set; }
    }
}