using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the request-related [HATEOAS links](/docs/api/overview/#hateoas-links)
/// </summary>
public class Link
{
    #region Properties

    /// <summary>
    /// Gets or sets the link title.
    /// </summary>
    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the complete target URL. To make the related call, combine the method with this link, in [URI template format](https://tools.ietf.org/html/rfc6570). Include the `$`, `(`, and `)` characters for pre-processing. The `href` is the key HATEOAS component that links a completed call with a subsequent call.
    /// </summary>
    [JsonProperty(PropertyName = "href")]
    public string Href { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method required to make the related call.
    /// </summary>
    [JsonProperty(PropertyName = "method")]
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets the [link relation type](https://tools.ietf.org/html/rfc5988#section-4), which serves as an ID for a link that unambiguously describes the semantics of the link. See [Link Relations](https://www.iana.org/assignments/link-relations/link-relations.xhtml).
    /// </summary>
    [JsonProperty(PropertyName = "rel")]
    public string Rel { get; set; }

    /// <summary>
    /// Gets or sets the encryption type
    /// </summary>
    [JsonProperty(PropertyName = "encType")]
    public string EncType { get; set; }

    /// <summary>
    /// Gets or sets the media type, as defined by [RFC 2046](https://www.ietf.org/rfc/rfc2046.txt). Describes the link target.
    /// </summary>
    [JsonProperty(PropertyName = "mediaType")]
    public string MediaType { get; set; }

    #endregion
}