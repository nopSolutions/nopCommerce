using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Brevo.MarketingAutomation;

/// <summary>
/// Represents request to identify a customer
/// </summary>
public class IdentifyRequest : Request
{
    /// <summary>
    /// Gets or sets the email address of the user
    /// </summary>
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the object that contents all custom fields. Keep in mind that those user properties will populate your database on the Marketing Automation platform to create rich scenarios
    /// </summary>
    [JsonProperty(PropertyName = "attributes")]
    public object Attributes { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "identify";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}