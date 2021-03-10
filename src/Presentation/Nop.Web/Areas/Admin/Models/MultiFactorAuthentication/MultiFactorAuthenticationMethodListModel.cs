using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.MultiFactorAuthentication
{
    /// <summary>
    /// Represents an multi-factor authentication method list model
    /// </summary>
    public partial record MultiFactorAuthenticationMethodListModel : BasePagedListModel<MultiFactorAuthenticationMethodModel>
    {
    }
}
