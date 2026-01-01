using System.Text;
using Microsoft.Extensions.Options;
using AliExpressSdk.ConsoleHarness.Configuration;

namespace AliExpressSdk.ConsoleHarness.Services;

/// <summary>
/// Generates authorization URLs for the OAuth flow.
/// </summary>
public class AuthorizationUrlBuilder
{
    private readonly AliExpressOptions _options;

    public AuthorizationUrlBuilder(IOptions<AliExpressOptions> options)
    {
        _options = options.Value;
    }

    public string BuildUrl(string? redirectUri = null)
    {
        var uriBuilder = new UriBuilder(_options.AuthorizationUrl);
        var queryStringBuilder = new StringBuilder();
        queryStringBuilder.Append("response_type=code");
        queryStringBuilder.Append("&force_auth=true");
        queryStringBuilder.Append($"&redirect_uri={redirectUri ?? "https://localhost"}");
        queryStringBuilder.Append($"&client_id={_options.AppKey}");
        var parsedQueryString = System.Web.HttpUtility.ParseQueryString(queryStringBuilder.ToString());
        uriBuilder.Query = queryStringBuilder.ToString();
        var uri = uriBuilder.ToString();
        return uri;
    }
}
