using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.Facebook.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier")]
    public string ClientId { get; set; }

    [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientSecret")]
    public string ClientSecret { get; set; }
}