using Nop.Core;

namespace Nop.Services.Common;

/// <summary>
/// Represents the HTTP client to request current store
/// </summary>
public partial class StoreHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public StoreHttpClient(HttpClient client,
        IWebHelper webHelper)
    {
        //configure client
        client.BaseAddress = new Uri(webHelper.GetStoreLocation());

        _httpClient = client;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Keep the current store site alive
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result determines that request completed
    /// </returns>
    public virtual async Task KeepAliveAsync()
    {
        await _httpClient.GetStringAsync(NopCommonDefaults.KeepAlivePath);
    }

    #endregion
}