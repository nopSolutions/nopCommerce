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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute search model
        /// </returns>
        Task<VendorAttributeSearchModel> PrepareVendorAttributeSearchModelAsync(VendorAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged vendor attribute list model
        /// </summary>
        /// <param name="searchModel">Vendor attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute list model
        /// </returns>
        Task<VendorAttributeListModel> PrepareVendorAttributeListModelAsync(VendorAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare vendor attribute model
        /// </summary>
        /// <param name="model">Vendor attribute model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute model
        /// </returns>
        Task<VendorAttributeModel> PrepareVendorAttributeModelAsync(VendorAttributeModel model,
            VendorAttribute vendorAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged vendor attribute value list model
        /// </summary>
        /// <param name="searchModel">Vendor attribute value search model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute value list model
        /// </returns>
        Task<VendorAttributeValueListModel> PrepareVendorAttributeValueListModelAsync(VendorAttributeValueSearchModel searchModel,
            VendorAttribute vendorAttribute);

        /// <summary>
        /// Prepare vendor attribute value model
        /// </summary>
        /// <param name="model">Vendor attribute value model</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute value model
        /// </returns>
        Task<VendorAttributeValueModel> PrepareVendorAttributeValueModelAsync(VendorAttributeValueModel model,
            VendorAttribute vendorAttribute, VendorAttributeValue vendorAttributeValue, bool excludeProperties = false);
    }
}