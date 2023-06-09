using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.InfigoProductProvider.Api;

public class InfigoProductProviderHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InfigoProductProviderHttpClient> _logger;

    public InfigoProductProviderHttpClient(HttpClient httpClient, ILogger<InfigoProductProviderHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> RequestAsync(string url, string userName)
    {
        try
        {
            _logger.LogInformation("Performing the request");
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userName);

            var response = await _httpClient.GetAsync(url);

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load data");
            throw;
        }
    }

    public async Task<byte[]> GetPictureBinaryAsync(string url)
    {
        try
        {
            _logger.LogInformation("Performing the request");
            
            var response = await _httpClient.GetAsync(url);

            var imageBinary = await response.Content.ReadAsByteArrayAsync();

            return imageBinary;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load picture");
            throw;
        }
    }
}