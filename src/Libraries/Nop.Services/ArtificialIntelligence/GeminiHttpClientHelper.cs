using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create Gemini requests
/// </summary>
public partial class GeminiHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
    #region Methods

    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.GeminiBaseApiUrl);
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
        request.Headers.TryAddWithoutValidation(ArtificialIntelligenceDefaults.GeminiApiKeyHeader, settings.GeminiApiKey);

        var data = JsonConvert.SerializeObject(new
        {
            contents = new { parts = new[] { new { text = query } } }
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
                candidates = new[]
                {
                    new { content = new { parts = new [] { new { text = string.Empty } } } }
                }
            });

        var result = response.candidates.Select(c => c.content.parts).FirstOrDefault();

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
                usageMetadata = new
                {
                    promptTokenCount = 0,
                    candidatesTokenCount = 0,
                    thoughtsTokenCount = 0,
                    totalTokenCount = 0
                }
            });

        var result = $"Prompt tokens: {response.usageMetadata.promptTokenCount}{Environment.NewLine}Candidate tokens: {response.usageMetadata.candidatesTokenCount}{Environment.NewLine}Thought tokens: {response.usageMetadata.thoughtsTokenCount}{Environment.NewLine}Total tokens: {response.usageMetadata.totalTokenCount}";

        return result;
    }

    #endregion
}