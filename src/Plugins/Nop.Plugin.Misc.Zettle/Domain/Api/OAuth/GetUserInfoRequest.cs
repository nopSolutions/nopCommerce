using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.OAuth;

/// <summary>
/// Represents request to get user info
/// </summary>
public class GetUserInfoRequest : OAuthApiRequest, IAuthorizedRequest
{
    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "users/self";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Get;
}