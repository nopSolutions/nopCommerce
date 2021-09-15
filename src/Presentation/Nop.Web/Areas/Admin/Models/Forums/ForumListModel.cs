using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Forums
{
    /// <summary>
    /// Represents a forum list model
    /// </summary>
    public partial record ForumListModel : BasePagedListModel<ForumModel>
    {
    }
}