using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Plugin.Widgets.What3words.Services;

/// <summary>
/// Represents HTTP client to request what3words services
/// </summary>
public class What3wordsHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public What3wordsHttpClient(HttpClient httpClient)
    {
        //configure client
        httpClient.BaseAddress = new Uri("https://www.nopcommerce.com/");
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, What3wordsDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        _httpClient = httpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request client API key
    /// </summary>
    /// <param name="storeUrl">Store url</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    public async Task<string> RequestClientApiAsync(string storeUrl)
    {
        try
        {
            //prepare request parameters
            var requestString = JsonConvert.SerializeObject(new { Url = storeUrl });
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);

            //execute request and get response
            var httpResponse = await _httpClient.PostAsync("what3words/client-api", requestContent);
            httpResponse.EnsureSuccessStatusCode();

            //return result
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var result = JsonConvert
                .DeserializeAnonymousType(responseString, new { Message = string.Empty, ClientApi = string.Empty });

            if (!string.IsNullOrEmpty(result.Message))
                throw new NopException($"Generating client keys error - {result.Message}");

            if (string.IsNullOrEmpty(result.ClientApi))
                throw new NopException($"API key is empty");

            return result.ClientApi;
        }
        catch (AggregateException exception)
        {
            //rethrow actual exception
            throw exception.InnerException;
        }
    }

    #endregion
}