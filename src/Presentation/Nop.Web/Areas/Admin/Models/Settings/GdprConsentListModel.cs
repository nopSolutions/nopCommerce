using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a GDPR consent list model
    /// </summary>
    public partial record GdprConsentListModel : BasePagedListModel<GdprConsentModel>
    {
    }
}