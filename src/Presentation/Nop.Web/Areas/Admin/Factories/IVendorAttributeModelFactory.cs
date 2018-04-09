using Nop.Core.Domain.Vendors;
using Nop.Web.Areas.Admin.Models.Vendors;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor attribute model factory
    /// </summary>
    public partial interface IVendorAttributeModelFactory
    {
        /// <summary>
        /// Prepare vendor attribute search model
        /// </summary>
        /// <param name="searchModel">Vendor attribute search model</param>
        /// <returns>Vendor attribute search model</returns>
        VendorAttributeSearchModel PrepareVendorAttributeSearchModel(VendorAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged vendor attribute list model
        /// </summary>
        /// <param name="searchModel">Vendor attribute search model</param>
        /// <returns>Vendor attribute list model</returns>
        VendorAttributeListModel PrepareVendorAttributeListModel(VendorAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare vendor attribute model
        /// </summary>
        /// <param name="model">Vendor attribute model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Vendor attribute model</returns>
        VendorAttributeModel PrepareVendorAttributeModel(VendorAttributeModel model,
            VendorAttribute vendorAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged vendor attribute value list model
        /// </summary>
        /// <param name="searchModel">Vendor attribute value search model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>Vendor attribute value list model</returns>
        VendorAttributeValueListModel PrepareVendorAttributeValueListModel(VendorAttributeValueSearchModel searchModel,
            VendorAttribute vendorAttribute);

        /// <summary>
        /// Prepare vendor attribute value model
        /// </summary>
        /// <param name="model">Vendor attribute value model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Vendor attribute value model</returns>
        VendorAttributeValueModel PrepareVendorAttributeValueModel(VendorAttributeValueModel model,
            VendorAttribute vendorAttribute, VendorAttributeValue vendorAttributeValue, bool excludeProperties = false);
    }
}