using Nop.Core.Domain.Vendors;
using Nop.Web.Areas.Admin.Models.Vendors;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor model factory
    /// </summary>
    public partial interface IVendorModelFactory
    {
        /// <summary>
        /// Prepare vendor search model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor search model
        /// </returns>
        Task<VendorSearchModel> PrepareVendorSearchModelAsync(VendorSearchModel searchModel);

        /// <summary>
        /// Prepare paged vendor list model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor list model
        /// </returns>
        Task<VendorListModel> PrepareVendorListModelAsync(VendorSearchModel searchModel);

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="model">Vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor model
        /// </returns>
        Task<VendorModel> PrepareVendorModelAsync(VendorModel model, Vendor vendor, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged vendor note list model
        /// </summary>
        /// <param name="searchModel">Vendor note search model</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor note list model
        /// </returns>
        Task<VendorNoteListModel> PrepareVendorNoteListModelAsync(VendorNoteSearchModel searchModel, Vendor vendor);
    }
}