using System.Net.Http;
using System.Text.Json;
using AliExpressSdk.Models;

namespace AliExpressSdk.Clients;

public class AESystemClient : AEBaseClient
{
    public AESystemClient(string appKey, string appSecret, string session, HttpClient? httpClient = null)
        : base(appKey, appSecret, session, httpClient)
    {
    }

    public Task<Result<JsonElement>> GenerateSecurityToken(IDictionary<string, string> args)
        => Execute("/auth/token/security/create", args);

    public Task<Result<JsonElement>> GenerateToken(IDictionary<string, string> args)
        => Execute("/auth/token/create", args);

    public Task<Result<JsonElement>> RefreshSecurityToken(IDictionary<string, string> args)
        => Execute("/auth/token/security/refresh", args);

    public Task<Result<JsonElement>> RefreshToken(IDictionary<string, string> args)
        => Execute("/auth/token/refresh", args);
}
