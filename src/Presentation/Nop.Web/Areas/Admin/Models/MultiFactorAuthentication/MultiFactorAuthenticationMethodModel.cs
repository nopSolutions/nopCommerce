using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.MultiFactorAuthentication
{
    /// <summary>
    /// Represents an multi-factor authentication method model
    /// </summary>
    public partial record MultiFactorAuthenticationMethodModel : BaseNopModel, IPluginModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.SystemName")]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Configure")]
        public string ConfigurationUrl { get; set; }

        public string LogoUrl { get; set; }

        #endregion
    }
}
