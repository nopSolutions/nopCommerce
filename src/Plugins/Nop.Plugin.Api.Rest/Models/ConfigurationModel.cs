using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Api.Rest.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugin.Api.Rest.SwaggerUi")]
    public string SwaggerUiUrl { get; set; }

    [NopResourceDisplayName("Plugin.Api.Rest.SwaggerJson")]
    public string SwaggerJsonUrl { get; set; }
}
