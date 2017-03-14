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
        /// <returns>The apply vendor model</returns>
        ApplyVendorModel PrepareApplyVendorModel(ApplyVendorModel model, bool validateVendor,bool excludeProperties);

        /// <summary>
        /// Prepare the vendor info model
        /// </summary>
        /// <param name="model">Vendor info model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Vendor info model</returns>
        VendorInfoModel PrepareVendorInfoModel(VendorInfoModel model, bool excludeProperties);
    }
}
