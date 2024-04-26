using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Pusher;

/// <summary>
/// Represents request to delete the webhook subscription
/// </summary>
public class DeleteSubscriptionsRequest : PusherApiRequest
{
    /// <summary>
    /// Gets or sets the subscription unique identifier as UUID version 1
    /// </summary>
    [JsonIgnore]
    public string Uuid { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => $"organizations/self/subscriptions/{Uuid}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Delete;
}