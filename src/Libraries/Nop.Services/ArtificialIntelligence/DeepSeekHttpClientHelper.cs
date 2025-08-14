using System.Text;
using Markdig;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create DeepSeek requests
/// </summary>
public partial class DeepSeekHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
    #region Methods

    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.DeepSeekBaseApiUrl);
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
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {settings.DeepSeekApiKey}");

        var data = JsonConvert.SerializeObject(new
        {
            messages = new List<Dictionary<string, string>> { new() { ["content"] = query, ["role"] = "system" } },
            model = ArtificialIntelligenceDefaults.DeepSeekApiModel
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
                choices = new[]
                {
                    new { message =  new { content = string.Empty  } }
                }
            });

        var result = response.choices.FirstOrDefault()?.message?.content;

        return Markdown.ToHtml(result ?? string.Empty);
    }

    #endregion
}