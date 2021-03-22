using System.Threading.Tasks;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the vendor model factory
    /// </summary>
    public partial interface IVendorModelFactory
    {
        /// <summary>
        /// Prepare the apply vendor model
        /// </summary>
        /// <param name="model">The apply vendor model</param>
        /// <param name="validateVendor">Whether to validate that the customer is already a vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="vendorAttributesXml">Vendor attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the apply vendor model
        /// </returns>
        Task<ApplyVendorModel> PrepareApplyVendorModelAsync(ApplyVendorModel model, bool validateVendor, bool excludeProperties, string vendorAttributesXml);

        /// <summary>
        /// Prepare the vendor info model
        /// </summary>
        /// <param name="model">Vendor info model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overriddenVendorAttributesXml">Overridden vendor attributes in XML format; pass null to use VendorAttributes of vendor</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor info model
        /// </returns>
        Task<VendorInfoModel> PrepareVendorInfoModelAsync(VendorInfoModel model, bool excludeProperties, string overriddenVendorAttributesXml = "");
    }
}