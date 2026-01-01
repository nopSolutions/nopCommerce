# AliExpress SDK Console Harness

A command-line test harness for the AliExpress SDK, designed for testing API calls, authentication flows, and exploring the AliExpress Open Platform APIs.

## Features

- **OAuth Authorization Flow**: Interactive authorization with visual URL prompts
- **Direct API Calls**: Test any AliExpress API method with custom parameters
- **Request/Response Persistence**: Automatically saves request and response JSON for debugging
- **Flexible Configuration**: Supports appsettings.json, environment-specific configs, and environment variables
- **Clean Architecture**: Uses dependency injection and follows SOLID principles

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- AliExpress developer account with app credentials

### Configuration

Configure your credentials in one of three ways:

#### 1. appsettings.json (Recommended for development)

```json
{
  "AliExpress": {
    "AppKey": "your_app_key",
    "AppSecret": "your_app_secret",
    "Session": "your_session_token"
  }
}
```

#### 2. Environment Variables

```bash
export AE_APPKEY="your_app_key"
export AE_APPSECRET="your_app_secret"
export AE_SESSION="your_session_token"
```

#### 3. Environment-Specific Configuration

Create `appsettings.Development.json` or `appsettings.Production.json`:

```json
{
  "AliExpress": {
    "AppKey": "524166",
    "AppSecret": "xcRyvBgbGJACtY0Xvwo6HLQkUq9dWoCe"
  }
}
```

Set the environment:
```bash
export DOTNET_ENVIRONMENT=Development
```

### Usage

#### Authorize the Application

Start the OAuth authorization flow:

```bash
# Interactive mode - prompts for auth code
dotnet run authorize

# Non-interactive mode - provide auth code directly
dotnet run authorize 3_524166_VESbGkCME9HcQoDrWO0uWFQ4191
```

**Interactive mode:**
1. Display the authorization URL
2. Prompt you to visit the URL in your browser
3. Ask you to paste the authorization code
4. Exchange the code for an access token
5. Save the request and response to `api-calls/auth-token-create/`

**Non-interactive mode:**
1. Accepts the authorization code as a command-line argument
2. Exchanges the code for an access token immediately
3. Useful for automation, CI/CD pipelines, or when you already have the code

**Note:** Authorization codes expire in 30 minutes. Use them promptly after receiving them from the OAuth callback.

#### Search for Products

Search for products and retrieve detailed information:

```bash
# Search for products by keyword
dotnet run search-product Canvas Kung Fu Shoes
```

This command will:
1. Search for products using `aliexpress.ds.text.search`
2. Extract the first product from the search results
3. Retrieve detailed product information using `aliexpress.ds.product.get`
4. Save both requests and responses to `SampleResponses/product-search/` and `SampleResponses/product-details/`

**Note:** This command requires a valid session token to be configured.

#### Make API Calls

Call any API method with parameters:

```bash
# Refresh a token
dotnet run /auth/token/refresh refresh_token=your_refresh_token

# Get product details (requires session token)
dotnet run aliexpress.ds.product.get product_id=1234567890

# With multiple parameters
dotnet run aliexpress.affiliate.hotproduct.query keywords=phone page_no=1 page_size=20
```

#### View Help

```bash
dotnet run --help
```

## Architecture

The console harness follows clean architecture principles:

### Project Structure

```
AliExpressSdk.ConsoleHarness/
├── Commands/                    # Command handlers
│   ├── ApiCallCommand.cs       # Handles direct API calls
│   └── AuthorizeCommand.cs     # Handles OAuth flow
├── Configuration/               # Configuration and options
│   ├── AliExpressOptions.cs    # AliExpress API settings
│   ├── OutputOptions.cs        # Output persistence settings
│   └── ConfigurationExtensions.cs
├── Services/                    # Business logic services
│   ├── ApiCallPersistence.cs   # Saves requests/responses
│   ├── AuthenticationHandler.cs # Token creation/refresh
│   ├── AuthorizationUrlBuilder.cs
│   ├── ConsolePrompt.cs        # User interaction
│   └── ServiceCollectionExtensions.cs
├── Program.cs                   # Minimal entry point
└── appsettings.json            # Default configuration
```

### Design Principles

- **Single Responsibility**: Each class has one clear purpose
- **Dependency Injection**: All services are injected via constructor
- **Testability**: Services are easy to mock and test
- **Small Methods**: Methods kept under 20 lines for clarity
- **Low Complexity**: Classes limited to 10-12 methods

## Request/Response Persistence

All API calls are automatically saved to the `api-calls/` directory:

```
api-calls/
├── auth-token-create/
│   ├── request.json
│   └── response.json
├── auth-token-refresh/
│   ├── request.json
│   └── response.json
└── aliexpress-ds-product-get/
    ├── request.json
    └── response.json
```

This makes it easy to:
- Debug API issues
- Review request parameters
- Analyze response structures
- Share examples with team members

## Testing

The harness includes comprehensive unit tests:

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect "XPlat Code Coverage"
```

Test coverage includes:
- Configuration loading
- Parameter validation
- Signature generation
- Request model serialization
- Service registration

## Development

### Adding New Commands

1. Create a command class in `Commands/`
2. Inject required services via constructor
3. Implement `ExecuteAsync()` method
4. Register in `ServiceCollectionExtensions.cs`
5. Add routing logic in `Program.cs`

### Adding New Services

1. Create service class in `Services/`
2. Keep methods small and focused
3. Add constructor dependencies
4. Register in `ServiceCollectionExtensions.cs`
5. Add unit tests

## Troubleshooting

### "Configuration file not found"

Make sure you're running from the project directory:
```bash
cd dotnet/AliExpressSdk.ConsoleHarness
dotnet run
```

### "Session token is required"

Set your session token in appsettings.json or via `AE_SESSION` environment variable.

### Authorization URL doesn't work

Ensure:
- Your app key is correct
- Your app is approved by AliExpress
- The redirect URI matches your app configuration

## Reference

### Test Credentials

For development/testing purposes, the following test credentials are configured in `appsettings.Development.json`:

- **Client ID**: 524166
- **Client Secret**: xcRyvBgbGJACtY0Xvwo6HLQkUq9dWoCe

⚠️ **Warning**: These are example credentials for testing the harness structure. Replace with your actual credentials for production use.

### API Documentation

- [AliExpress Open Platform](https://openservice.aliexpress.com/)
- [API Signature Algorithm](https://openservice.aliexpress.com/doc/doc.htm?docId=1367)
- [OAuth Authorization](https://openservice.aliexpress.com/doc/doc.htm?docId=1364)

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.
