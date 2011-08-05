// Contributor(s): Bill Eisenman, ALOM Technologies, USA. Upgraded to TaxBasic v5

using System;
using System.Diagnostics;
using System.Text;
using System.Web.Routing;
using Nop.Core.Caching;
using Nop.Core.Plugins;
using Nop.Plugin.Tax.StrikeIron.TaxDataBasic;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.StrikeIron
{
    /// <summary>
    /// StrikeIron tax provider
    /// </summary>
    public class StrikeIronTaxProvider : BasePlugin, ITaxProvider
    {
        private const string TAXRATEUSA_KEY = "Nop.taxrateusa.zipCode-{0}";
        private const string TAXRATECANADA_KEY = "Nop.taxratecanada.province-{0}";
        
        private readonly StrikeIronTaxSettings _strikeIronTaxSettings;

        public StrikeIronTaxProvider(StrikeIronTaxSettings strikeIronTaxSettings)
        {
            this._strikeIronTaxSettings = strikeIronTaxSettings;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult();

            var address = calculateTaxRequest.Address;
            if (address == null)
            {
                result.AddError("Address is not set");
                return result;
            }
            if (address.Country == null)
            {
                result.AddError("Country is not set");
                return result;
            }

            //**************************************************************************************************************
            // As a Registered StrikeIron user, you can authenticate to a StrikeIron web service with either a 
            // UserID/Password combination or a License Key.  If you wish to use a License Key, 
            // assign this value to the UserID field and set the Password field to null.
            //**************************************************************************************************************
            string userId = _strikeIronTaxSettings.UserId;
            string password = _strikeIronTaxSettings.Password;
            
            decimal taxRate = decimal.Zero;
            if (address.Country.TwoLetterIsoCode.ToLower() == "us")
            {
                if (String.IsNullOrEmpty(address.ZipPostalCode))
                {
                    result.AddError("Zip is not provided");
                    return result;
                }
                string error = "";
                taxRate = GetTaxRateUsa(address.ZipPostalCode, userId, password, ref error);
                if (!String.IsNullOrEmpty(error))
                {
                    result.AddError(error);
                    return result;
                }
            }
            else if (address.Country.TwoLetterIsoCode.ToLower() == "ca")
            {
                if (address.StateProvince == null)
                {
                    result.AddError("Province is not set");
                    return result;
                }
                string error = "";
                taxRate = GetTaxRateCanada(address.StateProvince.Abbreviation, userId, password, ref error);
                if (!String.IsNullOrEmpty(error))
                {
                    result.AddError(error);
                    return result;
                }
            }
            else
            {
                result.AddError("Tax can be calculated only for USA zip or Canada province");
                return result;
            }

            result.TaxRate = taxRate * 100;
            return result;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="zipCode">zip</param>
        /// <param name="userId">User ID</param>
        /// <param name="password">Password</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRateUsa(string zipCode, string userId,
            string password, ref string error)
        {
            decimal result = decimal.Zero;
            string key = string.Format(TAXRATEUSA_KEY, zipCode);
            if (CacheManager.IsSet(key))
                return CacheManager.Get<decimal>(key);

            try
            {
                var taxService = new Nop.Plugin.Tax.StrikeIron.TaxDataBasic.TaxDataBasic();
                taxService.LicenseInfoValue = new LicenseInfo();
                taxService.LicenseInfoValue.RegisteredUser = new RegisteredUser();
                taxService.LicenseInfoValue.RegisteredUser.UserID = userId;
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
                    var sb = new StringBuilder();
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
                    Debug.WriteLine(debug);
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

            CacheManager.Set(key, result, 60);
            
            return result;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="province">province</param>
        /// <param name="userId">User ID</param>
        /// <param name="password">Password</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public decimal GetTaxRateCanada(string province, string userId,
            string password, ref string error)
        {
            decimal result = decimal.Zero;
            string key = string.Format(TAXRATECANADA_KEY, province);
            if (CacheManager.IsSet(key))
                return CacheManager.Get<decimal>(key);

            try
            {
                var taxService = new Nop.Plugin.Tax.StrikeIron.TaxDataBasic.TaxDataBasic();
                taxService.LicenseInfoValue = new LicenseInfo();
                taxService.LicenseInfoValue.RegisteredUser = new RegisteredUser();
                taxService.LicenseInfoValue.RegisteredUser.UserID = userId;
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
                    Debug.WriteLine(debug);
                    result = Convert.ToDecimal(wsOutput.ServiceResult.Total);
                    CacheManager.Set(key, result, 60);
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

            return result;
        }
        
        /// <summary>
        /// Gets a cache manager
        /// </summary>
        public ICacheManager CacheManager
        {
            get
            {
                //TODO inject ICacheManager
                return new MemoryCacheManager();
            }
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "TaxStrikeIron";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Tax.StrikeIron.Controllers" }, { "area", null } };
        }
    }
}
