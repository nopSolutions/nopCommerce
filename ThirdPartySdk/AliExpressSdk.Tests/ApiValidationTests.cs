using System.Text.Json;
using AliExpressSdk.Clients;
using AliExpressSdk.Services;
using AliExpressSdk.Models;
using Xunit;

namespace AliExpressSdk.Tests;

/// <summary>
/// Tests for API parameter validation.
/// These tests verify that parameters are correctly included and validated.
/// </summary>
public class ApiValidationTests
{
    [Fact]
    public async Task CallApiDirectly_WithNoMethod_ReturnsError()
    {
        // Arrange
        var client = new AEBaseClient("test_key", "test_secret", "test_session");
        var parameters = new Dictionary<string, string>();

        // Act
        var result = await client.CallApiDirectly("", parameters);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Method parameter is required", result.Message);
    }

    [Fact]
    public void TokenRequest_ToParameters_IncludesRequiredFields()
    {
        // Arrange
        var request = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code",
            ApiPath = "/auth/token/create"
        };

        // Act
        var parameters = request.ToParameters();

        // Assert
        Assert.Equal(4, parameters.Count);
        Assert.Contains("app_key", parameters.Keys);
        Assert.Contains("timestamp", parameters.Keys);
        Assert.Contains("sign_method", parameters.Keys);
        Assert.Contains("code", parameters.Keys);
    }

    [Fact]
    public void TokenRequest_WithUuid_IncludesUuidParameter()
    {
        // Arrange
        var request = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code",
            Uuid = "test-uuid",
            ApiPath = "/auth/token/create"
        };

        // Act
        var parameters = request.ToParameters();

        // Assert
        Assert.Equal(5, parameters.Count);
        Assert.Contains("uuid", parameters.Keys);
        Assert.Equal("test-uuid", parameters["uuid"]);
    }

    [Fact]
    public void TokenRefreshRequest_ToParameters_IncludesRefreshToken()
    {
        // Arrange
        var request = new TokenRefreshRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            RefreshToken = "refresh_token_value",
            ApiPath = "/auth/token/refresh"
        };

        // Act
        var parameters = request.ToParameters();

        // Assert
        Assert.Equal(4, parameters.Count);
        Assert.Contains("refresh_token", parameters.Keys);
        Assert.Equal("refresh_token_value", parameters["refresh_token"]);
    }

    [Fact]
    public void SignedTokenRequest_ToParameters_IncludesSignature()
    {
        // Arrange
        var tokenRequest = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code",
            ApiPath = "/auth/token/create"
        };

        var signedRequest = new SignedTokenRequest
        {
            Request = tokenRequest,
            Signature = "ABCDEF1234567890"
        };

        // Act
        var parameters = signedRequest.ToParameters();

        // Assert
        Assert.Contains("sign", parameters.Keys);
        Assert.Equal("ABCDEF1234567890", parameters["sign"]);
        Assert.Contains("code", parameters.Keys);
        Assert.Contains("app_key", parameters.Keys);
    }

    [Fact]
    public void SignatureService_WithEmptyParameters_StillProducesValidSignature()
    {
        // Arrange
        var service = new SignatureService("secret");
        var parameters = new Dictionary<string, string>();

        // Act
        var signature = service.Sign("/test/api", parameters);

        // Assert
        Assert.NotEmpty(signature);
        Assert.Equal(64, signature.Length); // SHA256 hex = 64 chars
    }

    [Fact]
    public void SignatureService_WithSingleParameter_IncludesInSignature()
    {
        // Arrange
        var service = new SignatureService("secret");
        var parametersEmpty = new Dictionary<string, string>();
        var parametersSingle = new Dictionary<string, string> { ["key1"] = "value1" };

        // Act
        var signatureEmpty = service.Sign("/test/api", parametersEmpty);
        var signatureSingle = service.Sign("/test/api", parametersSingle);

        // Assert
        Assert.NotEqual(signatureEmpty, signatureSingle);
    }
}

