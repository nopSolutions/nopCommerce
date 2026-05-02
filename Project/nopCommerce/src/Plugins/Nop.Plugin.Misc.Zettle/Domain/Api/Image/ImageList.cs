using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Image;

/// <summary>
/// Represents uploaded images details
/// </summary>
public class ImageList : ApiResponse
{
    /// <summary>
    /// Gets or sets the uploaded image details
    /// </summary>
    [JsonProperty(PropertyName = "uploaded")]
    public List<Image> Uploaded { get; set; }

    /// <summary>
    /// Gets or sets the invalid image details
    /// </summary>
    [JsonProperty(PropertyName = "invalid")]
    public List<string> Invalid { get; set; }
}