namespace Nop.Core.Domain.Messages;

/// <summary>
/// Represents an email account
/// </summary>
public partial class EmailAccount : BaseEntity
{
    /// <summary>
    /// Gets or sets an email address
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets an email display name
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets an email host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets an email port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets an email user name
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets an email password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of emails sent at one time
    /// </summary>
    public int MaxNumberOfEmails { get; set; }

    /// <summary>
    /// Gets or sets an identifier of the email authentication method
    /// </summary>
    public int EmailAuthenticationMethodId { get; set; }

    /// <summary>
    /// Gets or sets an authentication method
    /// </summary>
    public EmailAuthenticationMethod EmailAuthenticationMethod
    {
        get => (EmailAuthenticationMethod)EmailAuthenticationMethodId;
        set => EmailAuthenticationMethodId = (int)value;
    }

    /// <summary>
    /// Gets or sets the client identifier (OAuth2)
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client Secret
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID of the organization from which the application will let users sign-in
    /// </summary>
    public string TenantId { get; set; }
}