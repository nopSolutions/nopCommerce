using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AbcWarehouse.Plugin.Widgets.Listrak.Models;

public class ListrakService : IListrakService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ListrakService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.listrak.com/OAuth2/Token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "ao1xkc57sz7t1dw1qawh" },
                { "client_secret", "rDpBSv2PMMrpo2Nso0AAyFqiag1U395bYV4ltx1vhIE" },
                { "grant_type", "client_credentials" }
            })
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Token error: {response.StatusCode} - {content}");

        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
        return tokenResponse?.AccessToken ?? throw new Exception("Access token is null.");
    }

    public async Task<HttpResponseMessage> SubscribePhoneNumberAsync(string phoneNumber)
    {
        var token = await GetTokenAsync();
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listrakData = new
        {
            ShortCodeId = "1026",
            PhoneNumber = phoneNumber,
            PhoneListId = "152"
        };

        return await client.PostAsJsonAsync(
            $"https://api.listrak.com/sms/v1/ShortCode/{listrakData.ShortCodeId}/Contact/{listrakData.PhoneNumber}/PhoneList/{listrakData.PhoneListId}",
            listrakData
        );
    }
}
