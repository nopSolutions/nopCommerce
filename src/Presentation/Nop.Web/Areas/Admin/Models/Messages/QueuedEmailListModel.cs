using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a queued email list model
    /// </summary>
    public partial record QueuedEmailListModel : BasePagedListModel<QueuedEmailModel>
    {
    }
}