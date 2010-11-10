//------------------------------------------------------------------------------------------------------------------------------------
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
// Contributor(s): Bill Eisenman, ALOM Technologies, USA. Upgraded to TaxBasic v5
//------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Tax.TaxDataBasic;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Tax
{
    /// <summary>
    /// Strike Iron tax provider
    /// </summary>
    public class StrikeIronTaxProvider : ITaxProvider
    {
        #region Constants
        private const string TAXRATEUSA_KEY = "Nop.taxrateusa.zipCode-{0}";
        private const string TAXRATECANADA_KEY = "Nop.taxratecanada.province-{0}";
        #endregion

        #region Methods
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <param name="error">Error</param>
        /// <returns>Tax</returns>
        public decimal GetTaxRate(CalculateTaxRequest calculateTaxRequest, ref string error)
        {
            var address = calculateTaxRequest.Address;
            if (address == null)
            {
                error = "Billing address is not set";
                return 0;
            }
            if (address.Country == null)
            {
                error = "Country is not set";
                return 0;
            }

            //**************************************************************************************************************
            // As a Registered StrikeIron user, you can authenticate to a StrikeIron web service with either a 
            // UserID/Password combination or a License Key.  If you wish to use a License Key, 
            // assign this value to the UserID field and set the Password field to null.
            //**************************************************************************************************************
            string userID = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Tax.TaxProvider.StrikeIron.UserID");
            string password = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Tax.TaxProvider.StrikeIron.Password");
            //if (Password == " " || !String.IsNullOrEmpty(Password))
            //    Password = String.Empty;
            //**************************************************************************************************************

            decimal taxRate = decimal.Zero;
            if (address.Country.TwoLetterIsoCode.ToLower() == "us")
            {
                if (String.IsNullOrEmpty(address.ZipPostalCode))
                {
                    error = "Zip is not provided";
                    return 0;
                }
                taxRate = GetTaxRateUSA(address.ZipPostalCode, userID, password, ref error);
            }
            else if (address.Country.TwoLetterIsoCode.ToLower() == "ca")
            {
                if (address.StateProvince == null)
                {
                    error = "Province is not set";
                    return 0;
                }
                taxRate = GetTaxRateCanada(address.StateProvince.Abbreviation, userID, password, ref error);
            }
            else
            {
                error = "Tax can be calculated only for USA zip or Canada province";
                return 0;
            }

            if (String.IsNullOrEmpty(error))
            {
                return taxRate * 100;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="zipCode">zip</param>
        /// <param name="userID">UserID</param>
        /// <param name="password">Password</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRateUSA(string zipCode, string userID, 
            string password, ref string error)
        {
            decimal result = decimal.Zero;
            string key = string.Format(TAXRATEUSA_KEY, zipCode);
            object obj2 = CacheManager.Get(key);
            if (StrikeIronTaxProvider.CacheEnabled && (obj2 != null))
            {
                return (decimal)obj2;
            }

            try
            {
                NopSolutions.NopCommerce.Tax.TaxDataBasic.TaxDataBasic taxService = new NopSolutions.NopCommerce.Tax.TaxDataBasic.TaxDataBasic();
                taxService.LicenseInfoValue = new LicenseInfo();
                taxService.LicenseInfoValue.RegisteredUser = new RegisteredUser();
                taxService.LicenseInfoValue.RegisteredUser.UserID = userID;
                taxService.LicenseInfoValue.RegisteredUser.Password = password;

                // The GetTaxRateUS operation can now be called.  The output type for this operation is SIWSOutputOfTaxRateUSAData.
                // Note that for simplicity, there is no error handling in this sample project.  In a production environment, any
                // web service call should be encapsulated in a try-catch block.
                var wsOutput = taxService.GetTaxRateUS(zipCode);

                // The output objects of this StrikeIron web service contains two sections: ServiceStatus, which stores data
                // indicating the success/failure status of the the web service request; and ServiceResult, which contains the
                // actual data returne as a result of the request.

                // ServiceStatus contains two elements - StatusNbr: a numeric status code, and StatusDescription: a string
                // describing the status of the output object.  As a standard, you can apply the following assumptions for the value of
                // StatusNbr:
                //   200-299: Successful web service call (data found, etc...)
                //   300-399: Nonfatal error (No data found, etc...)
                //   400-499: Error due to invalid input
                //   500+: Unexpected internal error; contact support@strikeiron.com
                if ((wsOutput.ServiceStatus.StatusNbr >= 200) && (wsOutput.ServiceStatus.StatusNbr < 300))
                {
                    //Successfully called SalesTax service...
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("City name: " + wsOutput.ServiceResult.City);
                    sb.AppendLine("Zip Code: " + wsOutput.ServiceResult.ZIPCode);
                    sb.AppendLine("City sales tax: " + wsOutput.ServiceResult.CitySalesTax.ToString());
                    sb.AppendLine("City use tax: " + wsOutput.ServiceResult.CityUseTax.ToString());
                    sb.AppendLine("County name: " + wsOutput.ServiceResult.County);
                    sb.AppendLine("County sales tax: " + wsOutput.ServiceResult.CountySalesTax.ToString());
                    sb.AppendLine("County use tax: " + wsOutput.ServiceResult.CountyUseTax.ToString());
                    sb.AppendLine("State name: " + wsOutput.ServiceResult.State);
                    sb.AppendLine("State sales tax: " + wsOutput.ServiceResult.StateSalesTax.ToString());
                    sb.AppendLine("State use tax: " + wsOutput.ServiceResult.StateUseTax.ToString());
                    sb.AppendLine("State tax shipping alone: " + wsOutput.ServiceResult.TaxShippingAlone);
                    sb.AppendLine("State tax shipping and handling: " + wsOutput.ServiceResult.TaxShippingHandling);
                    sb.AppendLine("State total sales tax: " + wsOutput.ServiceResult.TotalSalesTax.ToString());
                    sb.AppendLine("State total use tax: " + wsOutput.ServiceResult.TotalUseTax.ToString());
                    string debug = sb.ToString();
                    result = Convert.ToDecimal(wsOutput.ServiceResult.TotalUseTax);
                }
                else
                {
                    // StrikeIron does not return SoapFault for invalid data when it cannot find a zipcode. 
                    error = String.Format("[{0}] - {1}", wsOutput.ServiceStatus.StatusNbr.ToString(), wsOutput.ServiceStatus.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (StrikeIronTaxProvider.CacheEnabled)
            {
                CacheManager.Add(key, result);
            }

            return result;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="province">province</param>
        /// <param name="userID">UserID</param>
        /// <param name="password">Password</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRateCanada(string province, string userID, 
            string password, ref string error)
        {
            decimal result = decimal.Zero;
            string key = string.Format(TAXRATECANADA_KEY, province);
            object obj2 = CacheManager.Get(key);
            if (StrikeIronTaxProvider.CacheEnabled && (obj2 != null))
            {
                return (decimal)obj2;
            }

            try
            {
                NopSolutions.NopCommerce.Tax.TaxDataBasic.TaxDataBasic taxService = new NopSolutions.NopCommerce.Tax.TaxDataBasic.TaxDataBasic();
                taxService.LicenseInfoValue = new LicenseInfo();
                taxService.LicenseInfoValue.RegisteredUser = new RegisteredUser();
                taxService.LicenseInfoValue.RegisteredUser.UserID = userID;
                taxService.LicenseInfoValue.RegisteredUser.Password = password;

                // The GetTaxRateCanada operation can now be called.  The output type for this operation is SIWSOutputOfTaxRateCanadaData.
                // Note that for simplicity, there is no error handling in this sample project.  In a production environment, any
                // web service call should be encapsulated in a try-catch block.
                // 
                var wsOutput = taxService.GetTaxRateCanada(province);

                // The output objects of this StrikeIron web service contains two sections: ServiceStatus, which stores data
                // indicating the success/failure status of the the web service request; and ServiceResult, which contains the
                // actual data returne as a result of the request.
                // 
                // ServiceStatus contains two elements - StatusNbr: a numeric status code, and StatusDescription: a string
                // describing the status of the output object.  As a standard, you can apply the following assumptions for the value of
                // StatusNbr:
                //   200-299: Successful web service call (data found, etc...)
                //   300-399: Nonfatal error (No data found, etc...)
                //   400-499: Error due to invalid input
                //   500+: Unexpected internal error; contact support@strikeiron.com
                //     
                if ((wsOutput.ServiceStatus.StatusNbr >= 200) && (wsOutput.ServiceStatus.StatusNbr < 300))
                {
                    //Successfully called SalesTax service...
                    var sb = new StringBuilder();
                    sb.AppendLine("Abbreviation: " + wsOutput.ServiceResult.Abbreviation);
                    sb.AppendLine("GST: " + wsOutput.ServiceResult.GST.ToString());
                    sb.AppendLine("province: " + wsOutput.ServiceResult.Province);
                    sb.AppendLine("PST: " + wsOutput.ServiceResult.PST.ToString());
                    sb.AppendLine("tax_shipping_handling: " + wsOutput.ServiceResult.TaxShippingHandling);
                    sb.AppendLine("total: " + wsOutput.ServiceResult.Total.ToString());
                    string debug = sb.ToString();
                    result = Convert.ToDecimal(wsOutput.ServiceResult.Total);
                }
                else
                {
                    // StrikeIron does not return SoapFault for invalid data when it cannot find a zipcode. 
                    error = String.Format("[{0}] - {1}", wsOutput.ServiceStatus.StatusNbr.ToString(), wsOutput.ServiceStatus.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (StrikeIronTaxProvider.CacheEnabled)
            {
                CacheManager.Add(key, result);
            }

            return result;
        }
        #endregion

        #region Properties
        
        #region Utilities

        public ICacheManager CacheManager 
        {
            get
            {
                return new NopStaticCache();
            }
        }

        #endregion


        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
