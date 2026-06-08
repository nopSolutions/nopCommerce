using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create ChatGPT requests
/// </summary>
public partial class ChatGptHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
    #region Methods

    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.ChatGptBaseApiUrl);
    }

    /// <summary>
    /// Create HTTP request
    /// </summary>
    /// <param name="settings">Artificial intelligence settings</param>
    /// <param name="query">query to send into artificial intelligence host</param>
    /// <returns>Created HttpRequestMessage</returns>
    public virtual HttpRequestMessage CreateRequest(ArtificialIntelligenceSettings settings, string query)
    {
        var request = new HttpRequestMessage { Method = HttpMethod.Post };
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {settings.ChatGptApiKey}");

        var data = JsonConvert.SerializeObject(new Dictionary<string, string>
        {
            ["model"] = ArtificialIntelligenceDefaults.ChatGptApiModel,
            ["input"] = query
        });

        request.Content = new StringContent(data, Encoding.UTF8, MimeTypes.ApplicationJson);

        return request;
    }

    /// <summary>
    /// Parse response
    /// </summary>
    /// <param name="responseText">Response text to parse</param>
    /// <returns>Generated text from artificial intelligence host</returns>
    public virtual string ParseResponse(string responseText)
    {
        var response = JsonConvert.DeserializeAnonymousType(responseText,
            new
            {
                output = new[]
                {
                    new { content = new [] { new { text = string.Empty } } }
                }
            });

        var result = response.output.Select(o => o.content).FirstOrDefault();

        return result?[0].text ?? string.Empty;
    }

    /// <summary>
    /// Gets tokens usage information
    /// </summary>
    /// <param name="responseText">Text of response from AI service to get token usage information</param>
    /// <returns>Tokens usage information</returns>
    public virtual string GetTokensInfo(string responseText)
    {
        var response = JsonConvert.DeserializeAnonymousType(responseText,
            new
            {
                usage = new
                {
                    input_tokens = 0,
                    output_tokens = 0,
                    total_tokens = 0
                }
            });

        var result = $"Input tokens: {response.usage.input_tokens}{Environment.NewLine}Output tokens: {response.usage.output_tokens}{Environment.NewLine}Total tokens: {response.usage.total_tokens}";

        return result;
    }

    #endregion
}