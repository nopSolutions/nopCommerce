using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor search model
    /// </summary>
    public partial class VendorSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Vendors.List.SearchName")]
        public string SearchName { get; set; }

        #endregion
    }
}