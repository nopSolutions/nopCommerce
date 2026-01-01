using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AliExpressSdk.ConsoleHarness.Commands;
using AliExpressSdk.ConsoleHarness.Configuration;
using AliExpressSdk.ConsoleHarness.Services;

var configuration = BuildConfiguration();
var services = BuildServiceProvider(configuration);

if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
{
    PrintUsage();
    return 0;
}

var command = args[0].ToLowerInvariant();

return command switch
{
    "authorize" => await ExecuteAuthorizeCommand(services, args.Skip(1).ToArray()),
    "search-product" => await ExecuteSearchProductCommand(services, args.Skip(1).ToArray()),
    "create-order" => await ExecuteCreateOrderCommand(services, args.Skip(1).ToArray()),
    _ => await ExecuteApiCallCommand(services, args)
};

static IConfiguration BuildConfiguration()
{
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
    
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables(prefix: "AE_")
        .Build();
}

static ServiceProvider BuildServiceProvider(IConfiguration configuration)
{
    var services = new ServiceCollection();
    
    services.AddSingleton(configuration);
    services.AddAppConfiguration(configuration);
    services.AddApplicationServices();
    
    return services.BuildServiceProvider();
}

static async Task<int> ExecuteAuthorizeCommand(ServiceProvider services, string[] args)
{
    var authCode = args.Length > 0 ? args[0] : null;
    var command = services.GetRequiredService<AuthorizeCommand>();
    return await command.ExecuteAsync(authCode);
}

static async Task<int> ExecuteSearchProductCommand(ServiceProvider services, string[] args)
{
    var searchKeyword = args.Length > 0 ? string.Join(" ", args) : null;
    
    if (string.IsNullOrWhiteSpace(searchKeyword))
    {
        Console.Error.WriteLine("Search keyword is required.");
        Console.Error.WriteLine("Usage: dotnet run search-product <keyword>");
        return 1;
    }
    
    var command = services.GetRequiredService<ProductSearchCommand>();
    return await command.ExecuteAsync(searchKeyword);
}

static async Task<int> ExecuteCreateOrderCommand(ServiceProvider services, string[] args)
{
    var searchKeyword = args.Length > 0 ? string.Join(" ", args) : null;
    
    if (string.IsNullOrWhiteSpace(searchKeyword))
    {
        Console.Error.WriteLine("Search keyword is required.");
        Console.Error.WriteLine("Usage: dotnet run create-order <keyword>");
        return 1;
    }
    
    var command = services.GetRequiredService<CreateOrderWorkflowCommand>();
    return await command.ExecuteAsync(searchKeyword);
}

static async Task<int> ExecuteApiCallCommand(ServiceProvider services, string[] args)
{
    var method = args[0];
    var parameters = ParseParameters(args.Skip(1).ToArray());
    
    var command = services.GetRequiredService<ApiCallCommand>();
    return await command.ExecuteAsync(method, parameters);
}

static Dictionary<string, string> ParseParameters(string[] args)
{
    var parameters = new Dictionary<string, string>();
    
    foreach (var arg in args)
    {
        var separatorIndex = arg.IndexOf('=');
        if (separatorIndex < 1 || separatorIndex == arg.Length - 1)
        {
            Console.Error.WriteLine($"Warning: Could not parse parameter '{arg}'. Use the format key=value.");
            continue;
        }
        
        var key = arg[..separatorIndex];
        var value = arg[(separatorIndex + 1)..];
        parameters[key] = value;
    }
    
    return parameters;
}

static void PrintUsage()
{
    Console.WriteLine("AliExpress SDK Console Harness");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run authorize [code]             - Start OAuth authorization flow");
    Console.WriteLine("                                            Optionally provide auth code to skip prompt");
    Console.WriteLine("  dotnet run search-product <keyword>     - Search for products and get details");
    Console.WriteLine("  dotnet run create-order <keyword>       - Full workflow: search, details, freight, order");
    Console.WriteLine("  dotnet run <method> [key=value ...]     - Call an API method directly");
    Console.WriteLine();
    Console.WriteLine("Configuration:");
    Console.WriteLine("  Configure credentials in appsettings.json or use environment variables:");
    Console.WriteLine("  AE_APPKEY, AE_APPSECRET, AE_SESSION");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run authorize");
    Console.WriteLine("  dotnet run authorize 3_524166_VESbGkCME9HcQoDrWO0uWFQ4191");
    Console.WriteLine("  dotnet run search-product Canvas Kung Fu Shoes");
    Console.WriteLine("  dotnet run create-order Canvas Kung Fu Shoes");
    Console.WriteLine("  dotnet run /auth/token/refresh refresh_token=abc123");
}
