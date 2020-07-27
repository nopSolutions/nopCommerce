using Nop.Web.Framework.Models;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents GoogleAuthenticator model
    /// </summary>
    public class GoogleAuthenticatorModel : BaseNopEntityModel
    {
        public string Customer { get; set; }

        public string SecretKey { get; set; }
    }
}
