using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor note search model
    /// </summary>
    public partial class VendorNoteSearchModel : BaseSearchModel
    {
        #region Properties

        public int VendorId { get; set; }
        
        #endregion
    }
}