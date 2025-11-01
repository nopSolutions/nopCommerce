using System.Text;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;

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
    protected readonly ILogger _logger;

    #endregion

    #region Ctor

    public ArtificialIntelligenceHttpClient(ArtificialIntelligenceSettings artificialIntelligenceSettings,
        HttpClient httpClient,
        ILogger logger)
    {
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _httpClient = httpClient;
        _logger = logger;

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
    /// <param name="query">Query to artificial intelligence host</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response from the artificial intelligence host
    /// </returns>
    public virtual async Task<string> SendQueryAsync(string query)
    {
        var request = _artificialIntelligenceHttpClientHelper.CreateRequest(_artificialIntelligenceSettings, query);
        
        var httpResponse = await _httpClient.SendAsync(request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        var log = new StringBuilder($"AI {_artificialIntelligenceSettings.ProviderType.ToString()} request: {request}{Environment.NewLine}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            if (_artificialIntelligenceSettings.LogRequests)
            {
                await appendBaseInfo();

                await _logger.InsertLogAsync(LogLevel.Information, $"AI {_artificialIntelligenceSettings.ProviderType.ToString()} request", log.ToString());
            }

            throw new NopException(httpResponse.ReasonPhrase, innerException: new Exception(response));
        }

        var result  = _artificialIntelligenceHttpClientHelper.ParseResponse(response);

        if (!_artificialIntelligenceSettings.LogRequests) 
            return result;

        var tokensInfo = _artificialIntelligenceHttpClientHelper.GetTokensInfo(response);

        log.AppendLine("Tokens info:");
        log.AppendLine(tokensInfo);
        await appendBaseInfo();

        await _logger.InsertLogAsync(LogLevel.Information, $"AI {_artificialIntelligenceSettings.ProviderType.ToString()} request ({tokensInfo.Replace(Environment.NewLine, ", ")})",  log.ToString());

        return result;

        async Task appendBaseInfo()
        {
            if (request.Content != null)
            {
                log.AppendLine("Request content:");
                log.AppendLine(await request.Content.ReadAsStringAsync());
            }

            log.AppendLine($"Response {httpResponse}:");

            if (!string.IsNullOrEmpty(response))
            {
                log.AppendLine("Response content:");
                log.AppendLine(response);
            }
        }
    }

    #endregion
}