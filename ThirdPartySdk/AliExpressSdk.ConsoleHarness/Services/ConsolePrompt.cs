namespace AliExpressSdk.ConsoleHarness.Services;

/// <summary>
/// Handles interactive console prompts.
/// </summary>
public class ConsolePrompt
{
    public string PromptForAuthCode(string authorizationUrl)
    {
        Console.WriteLine();
        Console.WriteLine("=== AliExpress Authorization Required ===");
        Console.WriteLine();
        Console.WriteLine("Please visit the following URL to authorize this application:");
        Console.WriteLine();
        Console.WriteLine(authorizationUrl);
        Console.WriteLine();
        Console.WriteLine("After authorizing, you will be redirected to a URL containing the authorization code.");
        Console.WriteLine("Copy the 'code' parameter from the redirect URL and paste it below.");
        Console.WriteLine();
        Console.Write("Enter authorization code: ");
        
        var code = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("Authorization code is required.");
        }
        
        return code.Trim();
    }
}
