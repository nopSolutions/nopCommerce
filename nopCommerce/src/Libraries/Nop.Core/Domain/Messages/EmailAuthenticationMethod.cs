namespace Nop.Core.Domain.Messages;

/// <summary>
/// Authentication method
/// </summary>
public enum EmailAuthenticationMethod
{
    /// <summary>
    /// Email account does not require authentication
    /// </summary>
    None = 0,

    /// <summary>
    /// Authenticate through the default network credentials
    /// </summary>
    Ntlm = 5,

    /// <summary>
    /// Authenticate through username and password
    /// </summary>
    Login = 10,

    /// <summary>
    /// Authenticate through Google APIs Client with OAuth2
    /// </summary>
    GmailOAuth2 = 15,

    /// <summary>
    /// Authenticate through Microsoft Authentication Client with OAuth2
    /// </summary>
    MicrosoftOAuth2 = 20
}
