using System.Text.Json;
using Microsoft.Extensions.Options;
using AliExpressSdk.ConsoleHarness.Configuration;

namespace AliExpressSdk.ConsoleHarness.Services;

/// <summary>
/// Persists API request and response data to the file system.
/// </summary>
public class ApiCallPersistence
{
    private readonly OutputOptions _options;

    public ApiCallPersistence(IOptions<OutputOptions> options)
    {
        _options = options.Value;
    }

    public async Task SaveRequest(string apiPath, object request)
    {
        if (!_options.SaveRequestsAndResponses)
        {
            return;
        }

        var sanitizedPath = SanitizePath(apiPath);
        var directory = GetDirectory(sanitizedPath);
        var filePath = Path.Combine(directory, "request.json");
        
        await WriteJsonFile(filePath, request);
    }

    public async Task SaveResponse(string apiPath, object response)
    {
        if (!_options.SaveRequestsAndResponses)
        {
            return;
        }

        var sanitizedPath = SanitizePath(apiPath);
        var directory = GetDirectory(sanitizedPath);
        var filePath = Path.Combine(directory, "response.json");
        
        await WriteJsonFile(filePath, response);
    }

    private string GetDirectory(string apiPath)
    {
        var directory = Path.Combine(_options.OutputDirectory, apiPath);
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static string SanitizePath(string apiPath)
    {
        return apiPath.TrimStart('/').Replace('/', '-').Replace('.', '-');
    }

    private static async Task WriteJsonFile(string path, object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync(path, json);
    }
}
