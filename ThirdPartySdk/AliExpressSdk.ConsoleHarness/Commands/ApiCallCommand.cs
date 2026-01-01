using System.Text.Json;
using Microsoft.Extensions.Options;
using AliExpressSdk.Clients;
using AliExpressSdk.ConsoleHarness.Configuration;
using AliExpressSdk.ConsoleHarness.Services;

namespace AliExpressSdk.ConsoleHarness.Commands;

/// <summary>
/// Handles direct API method calls.
/// </summary>
public class ApiCallCommand
{
    private readonly AliExpressOptions _options;
    private readonly ApiCallPersistence _persistence;

    public ApiCallCommand(
        IOptions<AliExpressOptions> options,
        ApiCallPersistence persistence)
    {
        _options = options.Value;
        _persistence = persistence;
    }

    public async Task<int> ExecuteAsync(string method, IDictionary<string, string> parameters)
    {
        if (string.IsNullOrWhiteSpace(_options.Session))
        {
            Console.Error.WriteLine("Session token is required. Set AE_SESSION environment variable or configure in appsettings.");
            return 1;
        }

        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        await _persistence.SaveRequest(method, parameters);
        
        var result = await client.CallApiDirectly(method, parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse(method, data);
            
            Console.WriteLine("Request succeeded.");
            Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            return 0;
        }

        Console.Error.WriteLine("Request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("AliExpress error response:");
            Console.Error.WriteLine(JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        return 1;
    }
}
