using AliExpressSdk.Models;
using Xunit;

namespace AliExpressSdk.Tests;

public class SignedTokenRequestTests
{
    [Fact]
    public void ToParameters_IncludesSignature()
    {
        var tokenRequest = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code_123",
            ApiPath = "/auth/token/create"
        };
        
        var signedRequest = new SignedTokenRequest
        {
            Request = tokenRequest,
            Signature = "ABC123DEF456"
        };
        
        var parameters = signedRequest.ToParameters();
        
        Assert.Equal("ABC123DEF456", parameters["sign"]);
        Assert.Equal("12345678", parameters["app_key"]);
        Assert.Equal("auth_code_123", parameters["code"]);
    }
}
