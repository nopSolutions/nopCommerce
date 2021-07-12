using Nop.Web.Framework.Models;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents GoogleAuthenticator list model
    /// </summary>
    public partial record GoogleAuthenticatorListModel : BasePagedListModel<GoogleAuthenticatorModel>
    {
    }
}
