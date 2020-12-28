using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents an email account list model
    /// </summary>
    public partial record EmailAccountListModel : BasePagedListModel<EmailAccountModel>
    {
    }
}