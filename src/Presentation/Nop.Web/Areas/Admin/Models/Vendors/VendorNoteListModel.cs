using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor note list model
    /// </summary>
    public partial record VendorNoteListModel : BasePagedListModel<VendorNoteModel>
    {
    }
}