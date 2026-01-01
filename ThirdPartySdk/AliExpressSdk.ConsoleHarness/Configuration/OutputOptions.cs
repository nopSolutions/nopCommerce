namespace AliExpressSdk.ConsoleHarness.Configuration;

/// <summary>
/// Configuration options for output settings.
/// </summary>
public class OutputOptions
{
    public const string SectionName = "Output";
    
    public bool SaveRequestsAndResponses { get; set; } = true;
    public string OutputDirectory { get; set; } = "api-calls";
}
