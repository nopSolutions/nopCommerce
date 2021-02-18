using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.SendinBlue.Models
{
    /// <summary>
    /// Represents message template list model
    /// </summary>
    public record SendinBlueMessageTemplateListModel : BasePagedListModel<SendinBlueMessageTemplateModel>
    {
    }
}