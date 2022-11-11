using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a message template list model
    /// </summary>
    public partial record MessageTemplateListModel : BasePagedListModel<MessageTemplateModel>
    {
    }
}