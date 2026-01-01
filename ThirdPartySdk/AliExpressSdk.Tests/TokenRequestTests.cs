using AliExpressSdk.Models;
using Xunit;

namespace AliExpressSdk.Tests;

public class TokenRequestTests
{
    [Fact]
    public void ToParameters_IncludesAllRequiredFields()
    {
        var request = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code_123",
            ApiPath = "/auth/token/create"
        };
        
        var parameters = request.ToParameters();
        
        Assert.Equal("12345678", parameters["app_key"]);
        Assert.Equal("1517820392000", parameters["timestamp"]);
        Assert.Equal("sha256", parameters["sign_method"]);
        Assert.Equal("auth_code_123", parameters["code"]);
    }
    
    [Fact]
    public void ToParameters_IncludesUuidWhenProvided()
    {
        var request = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code_123",
            Uuid = "optional-uuid",
            ApiPath = "/auth/token/create"
        };
        
        var parameters = request.ToParameters();
        
        Assert.Equal("optional-uuid", parameters["uuid"]);
    }
    
    [Fact]
    public void ToParameters_ExcludesUuidWhenEmpty()
    {
        var request = new TokenRequest
        {
            AppKey = "12345678",
            Timestamp = 1517820392000,
            SignMethod = "sha256",
            Code = "auth_code_123",
            ApiPath = "/auth/token/create"
        };
        
        var parameters = request.ToParameters();
        
        Assert.DoesNotContain("uuid", parameters.Keys);
    }
    
    [Fact]
    public void CreateTimestamp_ReturnsValidTimestamp()
    {
        var timestamp = TokenRequestBase.CreateTimestamp();
        
        // Should be roughly current time (within last few seconds)
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Assert.InRange(timestamp, now - 5000, now + 5000);
    }
}
