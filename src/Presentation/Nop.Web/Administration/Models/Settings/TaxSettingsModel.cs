using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class TaxSettingsModel : BaseNopModel
    {
        public TaxSettingsModel()
        {
            PaymentMethodAdditionalFeeTaxCategories = new List<SelectListItem>();
            ShippingTaxCategories = new List<SelectListItem>();
            EuVatShopCountries = new List<SelectListItem>();
            DefaultTaxAddress = new AddressModel();
        }

        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PricesIncludeTax")]
        public bool PricesIncludeTax { get; set; }
        public bool PricesIncludeTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.AllowCustomersToSelectTaxDisplayType")]
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        public bool AllowCustomersToSelectTaxDisplayType_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.TaxDisplayType")]
        public int TaxDisplayType { get; set; }
        public bool TaxDisplayType_OverrideForStore { get; set; }
        public SelectList TaxDisplayTypeValues { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.DisplayTaxSuffix")]
        public bool DisplayTaxSuffix { get; set; }
        public bool DisplayTaxSuffix_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.DisplayTaxRates")]
        public bool DisplayTaxRates { get; set; }
        public bool DisplayTaxRates_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.HideZeroTax")]
        public bool HideZeroTax { get; set; }
        public bool HideZeroTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.HideTaxInOrderSummary")]
        public bool HideTaxInOrderSummary { get; set; }
        public bool HideTaxInOrderSummary_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ForceTaxExclusionFromOrderSubtotal")]
        public bool ForceTaxExclusionFromOrderSubtotal { get; set; }
        public bool ForceTaxExclusionFromOrderSubtotal_OverrideForStore { get; set; }





        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.TaxBasedOn")]
        public int TaxBasedOn { get; set; }
        public bool TaxBasedOn_OverrideForStore { get; set; }
        public SelectList TaxBasedOnValues { get; set; }

        public AddressModel DefaultTaxAddress { get; set; }
        public bool DefaultTaxAddress_OverrideForStore { get; set; }
       




        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ShippingIsTaxable")]
        public bool ShippingIsTaxable { get; set; }
        public bool ShippingIsTaxable_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ShippingPriceIncludesTax")]
        public bool ShippingPriceIncludesTax { get; set; }
        public bool ShippingPriceIncludesTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.ShippingTaxClass")]
        public int ShippingTaxClassId { get; set; }
        public bool ShippingTaxClassId_OverrideForStore { get; set; }
        public IList<SelectListItem> ShippingTaxCategories { get; set; }





        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeIsTaxable")]
        public bool PaymentMethodAdditionalFeeIsTaxable { get; set; }
        public bool PaymentMethodAdditionalFeeIsTaxable_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeIncludesTax")]
        public bool PaymentMethodAdditionalFeeIncludesTax { get; set; }
        public bool PaymentMethodAdditionalFeeIncludesTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.PaymentMethodAdditionalFeeTaxClass")]
        public int PaymentMethodAdditionalFeeTaxClassId { get; set; }
        public bool PaymentMethodAdditionalFeeTaxClassId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentMethodAdditionalFeeTaxCategories { get; set; }




        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatEnabled")]
        public bool EuVatEnabled { get; set; }
        public bool EuVatEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatShopCountry")]
        public int EuVatShopCountryId { get; set; }
        public bool EuVatShopCountryId_OverrideForStore { get; set; }
        public IList<SelectListItem> EuVatShopCountries { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatAllowVatExemption")]
        public bool EuVatAllowVatExemption { get; set; }
        public bool EuVatAllowVatExemption_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatUseWebService")]
        public bool EuVatUseWebService { get; set; }
        public bool EuVatUseWebService_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatAssumeValid")]
        public bool EuVatAssumeValid { get; set; }
        public bool EuVatAssumeValid_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Tax.EuVatEmailAdminWhenNewVatSubmitted")]
        public bool EuVatEmailAdminWhenNewVatSubmitted { get; set; }
        public bool EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore { get; set; }
    }
}