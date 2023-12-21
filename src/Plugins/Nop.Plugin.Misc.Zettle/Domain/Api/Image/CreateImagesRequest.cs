using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Image;

/// <summary>
/// Represents request to upload multiple images from URLs
/// </summary>
public class CreateImagesRequest : ImageApiRequest
{
    /// <summary>
    /// Gets or sets the images to upload
    /// </summary>
    [JsonProperty(PropertyName = "imageUploads")]
    public List<CreateImageRequest> ImageUploads { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "v2/images/organizations/self/products/bulk";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}