using System.Linq;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AliExpressSdk.Models;

namespace AliExpressSdk.Clients;

public class AEBaseClient
{
    private readonly HttpClient _httpClient;
    public string AppKey { get; }
    public string AppSecret { get; }
    public string Session { get; }

    private const string TopApiUrl = "https://api-sg.aliexpress.com/sync";
    private const string OpApiUrl = "https://api-sg.aliexpress.com/rest";
    private const string SignMethod = "sha256";

    public AEBaseClient(string appKey, string appSecret, string session, HttpClient? httpClient = null)
    {
        AppKey = appKey;
        AppSecret = appSecret;
        Session = session;
        _httpClient = httpClient ?? new HttpClient();
    }

    protected string Sign(IDictionary<string, string> parameters)
    {
        var p = new Dictionary<string, string>(parameters);
        var baseString = string.Empty;
        if (p.TryGetValue("method", out var method) && method.Contains('/'))
        {
            baseString = method;
            p.Remove("method");
        }

        foreach (var kv in p.Where(kv => !string.IsNullOrEmpty(kv.Value)).OrderBy(kv => kv.Key))
        {
            baseString += kv.Key + kv.Value;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(AppSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(baseString));
        return string.Concat(hash.Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
    }

    protected string Assemble(IDictionary<string, string> parameters)
    {
        var p = new Dictionary<string, string>(parameters);
        var baseUrl = p["method"].Contains('/') ? $"{OpApiUrl}{p["method"]}" : TopApiUrl;
        if (p["method"].Contains('/'))
        {
            p.Remove("method");
        }

        var query = string.Join("&", p
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .OrderBy(kv => kv.Key)
            .Select((kv, idx) =>
            {
                var prefix = idx == 0 ? "?" : "&";
                return prefix + Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value);
            }));

        return baseUrl + query;
    }

    protected async Task<Result<JsonElement>> Call(IDictionary<string, string> parameters)
    {
        var url = Assemble(parameters);
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new Result<JsonElement> { Ok = false, Message = $"HTTP Error: {(int)response.StatusCode} {response.ReasonPhrase}" };
            }
            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            if (root.TryGetProperty("error_response", out var error))
            {
                return new Result<JsonElement>
                {
                    Ok = false,
                    Message = "Bad request",
                    ErrorResponse = error,
                    RequestId = error.GetProperty("request_id").GetString()
                };
            }
            return new Result<JsonElement> { Ok = true, Data = root };
        }
        catch (Exception ex)
        {
            return new Result<JsonElement> { Ok = false, Message = ex.Message };
        }
    }

    protected async Task<Result<JsonElement>> Execute(string method, IDictionary<string, string> parameters)
    {
        var p = new Dictionary<string, string>(parameters)
        {
            ["method"] = method,
            ["session"] = Session,
            ["app_key"] = AppKey,
            ["simplify"] = "true",
            ["sign_method"] = SignMethod,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)
        };
        p["sign"] = Sign(p);
        return await Call(p);
    }

    public Task<Result<JsonElement>> CallApiDirectly(string method, IDictionary<string, string> parameters)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            return Task.FromResult(new Result<JsonElement> { Ok = false, Message = "Method parameter is required" });
        }
        var p = new Dictionary<string, string>(parameters)
        {
            ["method"] = method,
            ["session"] = Session,
            ["app_key"] = AppKey,
            ["simplify"] = "true",
            ["sign_method"] = SignMethod,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)
        };
        p["sign"] = Sign(p);
        return Call(p);
    }
}
