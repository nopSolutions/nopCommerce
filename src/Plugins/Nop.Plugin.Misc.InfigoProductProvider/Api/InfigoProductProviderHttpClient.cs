using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.InfigoProductProvider.Api;

public class InfigoProductProviderHttpClient
{
    private readonly HttpClient _httpClient;

    public InfigoProductProviderHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RequestAsync(string url, string userName)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userName);

            var response = await _httpClient.GetAsync(url);

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (Exception e)
        {
            throw e.InnerException;
        }
    }
}