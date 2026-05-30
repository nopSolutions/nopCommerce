using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Pusher;

/// <summary>
/// Represents request to create the webhook subscriptions
/// </summary>
public class CreateSubscriptionRequest : PusherApiRequest
{
    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "uuid")]
    public string Uuid { get; set; }

    /// <summary>
    /// Gets or sets the message option (currently only WEBHOOK is supported)
    /// </summary>
    [JsonProperty(PropertyName = "transportName")]
    public string TransportName { get; set; }

    /// <summary>
    /// Gets or sets the event names to subscribe
    /// </summary>
    [JsonProperty(PropertyName = "eventNames")]
    public List<string> EventNames { get; set; }

    /// <summary>
    /// Gets or sets the publicly exposed service URL
    /// </summary>
    [JsonProperty(PropertyName = "destination")]
    public string Destination { get; set; }

    /// <summary>
    /// Gets or sets the email address used to notify in case of any errors in subscription or the destination
    /// </summary>
    [JsonProperty(PropertyName = "contactEmail")]
    public string ContactEmail { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "organizations/self/subscriptions";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}