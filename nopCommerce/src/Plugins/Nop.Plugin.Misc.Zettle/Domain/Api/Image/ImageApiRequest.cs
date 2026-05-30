namespace Nop.Plugin.Misc.Zettle.Domain.Api.Image;

/// <summary>
/// Represents base request to Image API
/// </summary>
public abstract class ImageApiRequest : ApiRequest, IAuthorizedRequest
{
    /// <summary>
    /// Gets the request base URL
    /// </summary>
    public override string BaseUrl => "https://image.izettle.com/";
}