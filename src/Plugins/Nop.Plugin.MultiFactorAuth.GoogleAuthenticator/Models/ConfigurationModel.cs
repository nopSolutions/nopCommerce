using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Ctor

    public ConfigurationModel()
    {
        GoogleAuthenticatorSearchModel = new GoogleAuthenticatorSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.QRPixelsPerModule")]
    public int QRPixelsPerModule { get; set; }

    [NopResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.BusinessPrefix")]
    public string BusinessPrefix { get; set; }

    public GoogleAuthenticatorSearchModel GoogleAuthenticatorSearchModel { get; set; }

    #endregion
}