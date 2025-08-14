using System.Text;
using Markdig;
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

        return Markdown.ToHtml(result?[0].text.ToString() ?? string.Empty);
    }

    #endregion
}