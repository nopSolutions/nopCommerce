//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using Nop.Core.Configuration;

namespace Nop.Core.Domain.Tax
{
    public class TaxSettings : ISettings
    {
        /// <summary>
        /// Tax based on
        /// </summary>
        public TaxBasedOn TaxBasedOn { get; set; }

        /// <summary>
        /// Tax display type
        /// </summary>
        public TaxDisplayType TaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets an identifier of active tax provider
        /// </summary>
        public int ActiveTaxProviderId { get; set; }

        /// <summary>
        /// Gets or sets default address used for tax calculation
        /// </summary>
        public int DefaultTaxAddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display tax suffix
        /// </summary>
        public bool DisplayTaxSuffix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether each tax rate should be displayed on separate line (shopping cart page)
        /// </summary>
        public bool DisplayTaxRates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prices incude tax
        /// </summary>
        public bool PricesIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select tax display type
        /// </summary>
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide zero tax
        /// </summary>
        public bool HideZeroTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide tax in order summary when prices are shown tax inclusive
        /// </summary>
        public bool HideTaxInOrderSummary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price is taxable
        /// </summary>
        public bool ShippingIsTaxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price incudes tax
        /// </summary>
        public bool ShippingPriceIncludesTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the shipping tax class identifier
        /// </summary>
        public int ShippingTaxClassId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee is taxable
        /// </summary>
        public bool PaymentMethodAdditionalFeeIsTaxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee incudes tax
        /// </summary>
        public bool PaymentMethodAdditionalFeeIncludesTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the payment method additional fee tax class identifier
        /// </summary>
        public int PaymentMethodAdditionalFeeTaxClassId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EU VAT (Eupore Union Value Added Tax) is enabled
        /// </summary>
        public bool EuVatEnabled { get; set; }

        /// <summary>
        /// Gets or sets a shop country identifier
        /// </summary>
        public int EuVatShopCountryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this store will exempt eligible VAT-registered customers from VAT
        /// </summary>
        public bool EuVatAllowVatExemption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should use the EU web service to validate VAT numbers
        /// </summary>
        public bool EuVatUseWebService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should notify a store owner when a new VAT number is submitted
        /// </summary>
        public bool EuVatEmailAdminWhenNewVatSubmitted { get; set; }
    }
}