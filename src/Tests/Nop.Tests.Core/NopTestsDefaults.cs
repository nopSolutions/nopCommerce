namespace Nop.Tests;

/// <summary>
/// Represents default values related to tests
/// </summary>
public static partial class NopTestsDefaults
{
    /// <summary>
    /// Gets the default admin email
    /// </summary>
    public static string AdminEmail { get; } = "test@nopCommerce.com";

    /// <summary>
    /// Gets the default admin password
    /// </summary>
    public static string AdminPassword { get; } = "test_password";

    /// <summary>
    /// Gets the default host IP address
    /// </summary>
    public static string HostIpAddress { get; } = "127.0.0.1";
}