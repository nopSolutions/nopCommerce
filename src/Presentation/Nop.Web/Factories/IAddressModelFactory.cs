using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the address model factory
    /// </summary>
    public partial interface IAddressModelFactory
    {
        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
        /// <param name="prePopulateWithCustomerFields">Whether to populate model properties with the customer fields (used with the customer entity)</param>
        /// <param name="customer">Customer entity; required if prePopulateWithCustomerFields is true</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareAddressModelAsync(AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            Func<Task<IList<Country>>> loadCountries = null,
            bool prePopulateWithCustomerFields = false,
            Customer customer = null,
            string overrideAttributesXml = "");
    }
}
