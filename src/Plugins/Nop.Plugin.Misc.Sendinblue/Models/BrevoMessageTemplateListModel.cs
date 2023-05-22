using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents message template list model
    /// </summary>
    public record SendinblueMessageTemplateListModel : BasePagedListModel<SendinblueMessageTemplateModel>
    {
    }
}