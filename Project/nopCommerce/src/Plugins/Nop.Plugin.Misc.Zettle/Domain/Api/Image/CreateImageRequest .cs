using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Image;

/// <summary>
/// Represents request to upload single image from a URL
/// </summary>
public class CreateImageRequest : ImageApiRequest
{
    /// <summary>
    /// Gets or sets the image format (JPEG, PNG, GIF)
    /// </summary>
    [JsonProperty(PropertyName = "imageFormat")]
    public string ImageFormat { get; set; }

    /// <summary>
    /// Gets or sets the image URL. Image URL must use the HTTPS protocol, and must be properly encoded (only ASCII characters in the path)
    /// </summary>
    [JsonProperty(PropertyName = "imageUrl")]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "v2/images/organizations/self/products";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}