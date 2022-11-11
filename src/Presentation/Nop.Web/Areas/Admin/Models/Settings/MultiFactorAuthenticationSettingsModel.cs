using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a multi-factor authentication settings model
    /// </summary>
    public partial record MultiFactorAuthenticationSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ForceMultifactorAuthentication")]
        public bool ForceMultifactorAuthentication { get; set; }

        #endregion
    }
}
