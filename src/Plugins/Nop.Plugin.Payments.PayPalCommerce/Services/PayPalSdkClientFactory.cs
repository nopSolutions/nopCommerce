using System.Collections.Concurrent;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Creates and caches PayPal Server SDK client instances per configuration
/// </summary>
public class PayPalSdkClientFactory
{
    private readonly ConcurrentDictionary<string, PaypalServerSdkClient> _clients = new();

    /// <summary>
    /// Get or create a configured PayPal SDK client for the given settings
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Configured PaypalServerSdkClient instance</returns>
    public PaypalServerSdkClient GetClient(PayPalCommerceSettings settings)
    {
        var cacheKey = $"{settings.ClientId}_{settings.UseSandbox}";

        return _clients.GetOrAdd(cacheKey, _ => CreateClient(settings));
    }

    /// <summary>
    /// Remove a cached client (e.g. when credentials change)
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    public void InvalidateClient(PayPalCommerceSettings settings)
    {
        var cacheKey = $"{settings.ClientId}_{settings.UseSandbox}";
        _clients.TryRemove(cacheKey, out _);
    }

    private static PaypalServerSdkClient CreateClient(PayPalCommerceSettings settings)
    {
        var timeout = TimeSpan.FromSeconds(settings.RequestTimeout ?? PayPalCommerceDefaults.RequestTimeout);

        return new PaypalServerSdkClient.Builder()
            .ClientCredentialsAuth(
                new ClientCredentialsAuthModel.Builder(
                    settings.ClientId,
                    settings.SecretKey
                ).Build())
            .Environment(settings.UseSandbox
                ? PaypalServerSdk.Standard.Environment.Sandbox
                : PaypalServerSdk.Standard.Environment.Production)
            .HttpClientConfig(config => config.Timeout(timeout))
            .Build();
    }
}
