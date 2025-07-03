using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client to request artificial intelligence
/// </summary>
public partial class ArtificialIntelligenceHttpClient
{
    #region Fields

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;
    protected readonly HttpClient _httpClient;
    protected readonly IArtificialIntelligenceHttpClientHelper _artificialIntelligenceHttpClientHelper;

    #endregion

    #region Ctor

    public ArtificialIntelligenceHttpClient(ArtificialIntelligenceSettings artificialIntelligenceSettings,
        HttpClient httpClient)
    {
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _httpClient = httpClient;

        //configure client
        httpClient.Timeout = TimeSpan.FromSeconds(artificialIntelligenceSettings.RequestTimeout ??
            ArtificialIntelligenceDefaults.RequestTimeout);

        _artificialIntelligenceHttpClientHelper = _artificialIntelligenceSettings.ProviderType switch
        {
            ArtificialIntelligenceProviderType.Gemini => new GeminiHttpClientHelper(),
            ArtificialIntelligenceProviderType.ChatGpt => new ChatGptHttpClientHelper(),
            ArtificialIntelligenceProviderType.DeepSeek => new DeepSeekHttpClientHelper(),
            _ => throw new ArgumentOutOfRangeException()
        };

        _artificialIntelligenceHttpClientHelper.ConfigureClient(httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Send query to artificial intelligence host
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NopException"></exception>
    public virtual async Task<string> SendQueryAsync(string query)
    {
        var request = _artificialIntelligenceHttpClientHelper.CreateRequest(_artificialIntelligenceSettings, query);

        var httpResponse = await _httpClient.SendAsync(request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
            return _artificialIntelligenceHttpClientHelper.ParseResponse(response);

        throw new NopException(httpResponse.ReasonPhrase, innerException: new Exception(response));
    }

    #endregion
}