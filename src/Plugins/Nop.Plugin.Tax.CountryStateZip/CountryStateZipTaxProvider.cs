using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Tax.CountryStateZip.Data;
using Nop.Plugin.Tax.CountryStateZip.Services;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.CountryStateZip
{
    /// <summary>
    /// Fixed rate tax provider
    /// </summary>
    public class CountryStateZipTaxProvider : BasePlugin, ITaxProvider
    {
        private readonly ITaxRateService _taxRateService;
        private readonly TaxRateObjectContext _objectContext;

        public CountryStateZipTaxProvider(ITaxRateService taxRateService,
            TaxRateObjectContext objectContext)
        {
            this._taxRateService = taxRateService;
            this._objectContext = objectContext;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult();

            if (calculateTaxRequest.Address == null)
            {
                result.Errors.Add("Address is not set");
                return result;
            }

            var taxRates = _taxRateService.GetAllTaxRates(calculateTaxRequest.TaxCategoryId,
                calculateTaxRequest.Address.Country != null ? calculateTaxRequest.Address.Country.Id: 0,
                calculateTaxRequest.Address.StateProvince != null ? calculateTaxRequest.Address.StateProvince.Id : 0, 
                calculateTaxRequest.Address.ZipPostalCode);
            if (taxRates.Count > 0)
                result.TaxRate = taxRates[0].Percentage;

            return result;
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
            controllerName = "TaxCountryStateZip";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Tax.CountryStateZip.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _objectContext.Install();
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _objectContext.Uninstall();
            base.Uninstall();
        }
    }
}
