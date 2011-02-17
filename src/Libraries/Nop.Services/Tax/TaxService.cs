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
// Contributor(s): Stephen Kennedy - VAT support, _______. 
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.IO;
using System.Collections.ObjectModel;
using System.Web.Compilation;
using Nop.Core.Infrastructure;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class TaxService : ITaxService
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;
        private readonly ITypeFinder _typeFinder;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="addressService">Address service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="typeFinder">Type finder</param>
        public TaxService(IAddressService addressService,
            IWorkContext workContext,
            TaxSettings taxSettings,
            ITypeFinder typeFinder)
        {
            this._addressService = addressService;
            this._workContext = workContext;
            this._taxSettings = taxSettings;
            this._typeFinder = typeFinder;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a value indicating whether tax exempt
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>A value indicating whether tax exempt</returns>
        protected bool IsTaxExempt(ProductVariant productVariant, Customer customer)
        {
            if (customer != null)
            {
                if (customer.IsTaxExempt)
                    return true;

                var customerRoles = customer.CustomerRoles;
                foreach (var customerRole in customerRoles)
                    if (customerRole.TaxExempt)
                        return true;
            }
            
            if (productVariant == null)
            {
                return false;
            }

            if (productVariant.IsTaxExempt)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="customer">Customer</param>
        /// <returns>Result</returns>
        protected bool IsVatExempt(Address address, Customer customer)
        {
            if (!_taxSettings.EuVatEnabled)
            {
                return false;
            }

            if (address == null || address.Country == null || customer == null)
            {
                return false;
            }


            if (!address.Country.SubjectToVat)
            {
                // VAT not chargeable if shipping outside VAT zone:
                return true;
            }
            else
            {
                // VAT not chargeable if address, customer and config meet our VAT exemption requirements:
                // returns true if this customer is VAT exempt because they are shipping within the EU but outside our shop country, they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
                return address.CountryId != _taxSettings.EuVatShopCountryId &&
                    customer.VatNumberStatus == VatNumberStatus.Valid &&
                    _taxSettings.EuVatAllowVatExemption;
            }
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <remarks>Doesn't check the name and address</remarks>
        /// <returns>A value from the VatNumberStatusEnum enumeration</returns>
        protected VatNumberStatus DoVatCheck(string countryCode, string vatNumber, out string name, out string address, out Exception exception)
        {
            name = string.Empty;
            address = string.Empty;

            EuropaCheckVatService.checkVatService s = null;

            try
            {
                bool valid;
                vatNumber = vatNumber.Trim().Replace(" ", "");

                s = new EuropaCheckVatService.checkVatService();
                s.checkVat(ref countryCode, ref vatNumber, out valid, out name, out address);
                exception = null;
                return valid ? VatNumberStatus.Valid : VatNumberStatus.Invalid;
            }
            catch (Exception ex)
            {
                name = address = string.Empty;
                exception = ex;
                return VatNumberStatus.Unknown;
            }
            finally
            {
                if (name == null)
                    name = string.Empty;

                if (address == null)
                    address = string.Empty;

                if (s != null)
                    s.Dispose();
            }
        }
       
        /// <summary>
        /// Create request for tax calculation
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Package for tax calculation</returns>
        protected CalculateTaxRequest CreateCalculateTaxRequest(ProductVariant productVariant, 
            int taxCategoryId, Customer customer)
        {
            var calculateTaxRequest = new CalculateTaxRequest();
            calculateTaxRequest.Customer = customer;
            if (taxCategoryId > 0)
            {
                calculateTaxRequest.TaxCategoryId = taxCategoryId;
            }
            else
            {
                if (productVariant != null)
                    calculateTaxRequest.TaxCategoryId = productVariant.TaxCategoryId;
            }

            var basedOn = _taxSettings.TaxBasedOn;

            if (basedOn == TaxBasedOn.BillingAddress)
            {
                if (customer == null || customer.BillingAddress == null)
                {
                    basedOn = TaxBasedOn.DefaultAddress;
                }
            }
            if (basedOn == TaxBasedOn.ShippingAddress)
            {
                if (customer == null || customer.ShippingAddress == null)
                {
                    basedOn = TaxBasedOn.DefaultAddress;
                }
            }

            Address address = null;

            switch (basedOn)
            {
                case TaxBasedOn.BillingAddress:
                    {
                        address = customer.BillingAddress;
                    }
                    break;
                case TaxBasedOn.ShippingAddress:
                    {
                        address = customer.ShippingAddress;
                    }
                    break;
                case TaxBasedOn.DefaultAddress:
                default:
                    {
                        address = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);
                    }
                    break;
            }

            calculateTaxRequest.Address = address;
            return calculateTaxRequest;
        }

        /// <summary>
        /// Calculated price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="percent">Percent</param>
        /// <param name="increase">Increase</param>
        /// <returns>New price</returns>
        protected decimal CalculatePrice(decimal price, decimal percent, bool increase)
        {
            decimal result = decimal.Zero;
            if (percent == decimal.Zero)
                return price;

            if (increase)
            {
                result = price * (1 + percent / 100);
            }
            else
            {
                result = price - (price) / (100 + percent) * percent;
            }
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active tax provider
        /// </summary>
        /// <returns>Active tax provider</returns>
        public ITaxProvider LoadActiveTaxProvider()
        {
            var taxProvider = LoadTaxProviderBySystemName(_taxSettings.ActiveTaxProviderSystemName);
            if (taxProvider == null)
                taxProvider = LoadAllTaxProviders().FirstOrDefault();
            return taxProvider;
        }

        /// <summary>
        /// Load tax provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found tax provider</returns>
        public ITaxProvider LoadTaxProviderBySystemName(string systemName)
        {
            var providers = LoadAllTaxProviders();
            var provider = providers.SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            return provider;
        }

        /// <summary>
        /// Load all tax providers
        /// </summary>
        /// <returns>Tax providers</returns>
        public IList<ITaxProvider> LoadAllTaxProviders()
        {
            var taxProviders = new List<ITaxProvider>();

            var typesToRegister = _typeFinder.FindClassesOfType<ITaxProvider>();
            foreach (var type in typesToRegister)
            {
                //TODO inject ISettingService into type.SettingService
                dynamic taxProvider = Activator.CreateInstance(type);
                taxProviders.Add(taxProvider);
            }

            //sort and return
            return taxProviders.OrderBy(tp => tp.FriendlyName).ToList();
        }


        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRate(ProductVariant productVariant, Customer customer)
        {
            return GetTaxRate(productVariant, 0, customer);
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRate(int taxCategoryId, Customer customer)
        {
            return GetTaxRate(null, taxCategoryId, customer);
        }
        
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRate(ProductVariant productVariant, int taxCategoryId, 
            Customer customer)
        {
            //tax exempt
            if (IsTaxExempt(productVariant, customer))
            {
                return decimal.Zero;
            }

            //tax request
            var calculateTaxRequest = CreateCalculateTaxRequest(productVariant, taxCategoryId, customer);

            //make EU VAT exempt validation (the European Union Value Added Tax)
            if (_taxSettings.EuVatEnabled)
            {
                if (IsVatExempt(calculateTaxRequest.Address, calculateTaxRequest.Customer))
                {
                    //return zero if VAT is not chargeable
                    return decimal.Zero;
                }
            }

            //active tax provider
            var activeTaxProvider = LoadActiveTaxProvider();

            //get tax rate
            var calculateTaxResult = activeTaxProvider.GetTaxRate(calculateTaxRequest);
            if (calculateTaxResult.Success)
                return calculateTaxResult.TaxRate;
            else
                return decimal.Zero;
        }
        


        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetProductPrice(ProductVariant productVariant, decimal price, 
            out decimal taxRate)
        {
            var customer = _workContext.CurrentCustomer;
            return GetProductPrice(productVariant, price, customer, out taxRate);
        }
        
        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetProductPrice(ProductVariant productVariant, decimal price,
            Customer customer, out decimal taxRate)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetProductPrice(productVariant, price, includingTax, customer, out taxRate);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetProductPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, out decimal taxRate)
        {
            bool priceIncludesTax = _taxSettings.PricesIncludeTax;
            int taxCategoryId = 0;
            return GetProductPrice(productVariant, taxCategoryId, price, includingTax, 
                customer, priceIncludesTax, out taxRate);
        }
        
        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetProductPrice(ProductVariant productVariant, int taxCategoryId,
            decimal price, bool includingTax, Customer customer,
            bool priceIncludesTax, out decimal taxRate)
        {
            taxRate = GetTaxRate(productVariant, taxCategoryId, customer);
            
            if (priceIncludesTax)
            {
                if (!includingTax)
                {
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            else
            {
                if (includingTax)
                {
                    price = CalculatePrice(price, taxRate, true);
                }
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;

            return price;
        }




        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetShippingPrice(decimal price, Customer customer)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetShippingPrice(price, includingTax, customer);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetShippingPrice(decimal price, bool includingTax, Customer customer)
        {
            decimal taxRate = decimal.Zero;
            return GetShippingPrice(price, includingTax, customer, out taxRate);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetShippingPrice(decimal price, bool includingTax, Customer customer, out decimal taxRate)
        {
            taxRate = decimal.Zero;

            if (!_taxSettings.ShippingIsTaxable)
            {
                return price;
            }
            int taxClassId = _taxSettings.ShippingTaxClassId;
            bool priceIncludesTax = _taxSettings.ShippingPriceIncludesTax;
            return GetProductPrice(null, taxClassId, price, includingTax, customer,
                priceIncludesTax, out taxRate);
        }





        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetPaymentMethodAdditionalFee(decimal price, Customer customer)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetPaymentMethodAdditionalFee(price, includingTax, customer);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, Customer customer)
        {
            decimal taxRate = decimal.Zero;
            return GetPaymentMethodAdditionalFee(price, includingTax,
                customer, out taxRate);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, Customer customer, out decimal taxRate)
        {
            taxRate = decimal.Zero;

            if (!_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                return price;
            }
            int taxClassId = _taxSettings.PaymentMethodAdditionalFeeTaxClassId;
            bool priceIncludesTax = _taxSettings.PaymentMethodAdditionalFeeIncludesTax;
            return GetProductPrice(null, taxClassId, price, includingTax, customer,
                priceIncludesTax, out taxRate);
        }





        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav)
        {
            var customer = _workContext.CurrentCustomer;
            return GetCheckoutAttributePrice(cav, customer);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, Customer customer)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetCheckoutAttributePrice(cav, includingTax, customer);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer)
        {
            decimal taxRate = decimal.Zero;
            return GetCheckoutAttributePrice(cav, includingTax, customer, out taxRate);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer, out decimal taxRate)
        {
            if (cav == null)
                throw new ArgumentNullException("cav");

            taxRate = decimal.Zero;

            decimal price = cav.PriceAdjustment;
            if (cav.CheckoutAttribute.IsTaxExempt)
            {
                return price;
            }

            bool priceIncludesTax = _taxSettings.PricesIncludeTax;
            int taxClassId = cav.CheckoutAttribute.TaxCategoryId;
            return GetProductPrice(null, taxClassId, price, includingTax, customer,
                priceIncludesTax, out taxRate);
        }





        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        public VatNumberStatus GetVatNumberStatus(Country country,
            string vatNumber)
        {
            string name = string.Empty;
            string address = string.Empty;
            return GetVatNumberStatus(country, vatNumber, out name, out address);
        }
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        public VatNumberStatus GetVatNumberStatus(Country country,
            string vatNumber, out string name, out string address)
        {
            name = string.Empty;
            address = string.Empty;

            if (vatNumber == null)
                vatNumber = string.Empty;
            vatNumber = vatNumber.Trim();

            if (String.IsNullOrEmpty(vatNumber))
                return VatNumberStatus.Empty;


            if (country == null)
                return VatNumberStatus.Unknown;

            if (!_taxSettings.EuVatUseWebService)
                return VatNumberStatus.Unknown;
            
            try
            {
                Exception exception = null;
                return DoVatCheck(country.TwoLetterIsoCode, vatNumber, out name, out address, out exception);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
                return VatNumberStatus.Unknown;
            }
        }

        #endregion
    }
}
