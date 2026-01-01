using AliExpressSdk.Services;
using Xunit;

namespace AliExpressSdk.Tests;

public class SignatureServiceTests
{
    private const string TestAppSecret = "helloworld";
    
    [Fact]
    public void Sign_WithSystemApiPath_ProducesSameOutputAsDocumentation()
    {
        // This test case is from the official AliExpress documentation
        // https://openservice.aliexpress.com/doc/doc.htm?docId=1367
        var service = new SignatureService(TestAppSecret);
        var parameters = new Dictionary<string, string>
        {
            ["app_key"] = "12345678",
            ["code"] = "3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26",
            ["sign_method"] = "sha256",
            ["timestamp"] = "1517820392000"
        };
        
        var signature = service.Sign("/auth/token/create", parameters);
        
        // Expected signature from documentation
        Assert.Equal("35607762342831B6A417A0DED84B79C05FEFBF116969C48AD6DC00279A9F4D81", signature);
    }
    
    [Fact]
    public void Sign_WithEmptyApiPath_ThrowsArgumentException()
    {
        var service = new SignatureService(TestAppSecret);
        var parameters = new Dictionary<string, string>
        {
            ["app_key"] = "test"
        };
        
        Assert.Throws<ArgumentException>(() => service.Sign("", parameters));
    }
    
    [Fact]
    public void Constructor_WithEmptySecret_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SignatureService(""));
    }
    
    [Fact]
    public void Sign_SortsParametersAlphabetically()
    {
        var service = new SignatureService("secret");
        var parameters = new Dictionary<string, string>
        {
            ["zebra"] = "last",
            ["alpha"] = "first",
            ["middle"] = "middle"
        };
        
        // Should sort as: alpha, middle, zebra
        var signature1 = service.Sign("/test", parameters);
        
        // Reverse order input should produce same signature
        var parametersReversed = new Dictionary<string, string>
        {
            ["alpha"] = "first",
            ["middle"] = "middle",
            ["zebra"] = "last"
        };
        
        var signature2 = service.Sign("/test", parametersReversed);
        
        Assert.Equal(signature1, signature2);
    }
    
    [Fact]
    public void Sign_SkipsEmptyValues()
    {
        var service = new SignatureService("secret");
        var parametersWithEmpty = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "",
            ["key3"] = "value3"
        };
        
        var parametersWithoutEmpty = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key3"] = "value3"
        };
        
        var signature1 = service.Sign("/test", parametersWithEmpty);
        var signature2 = service.Sign("/test", parametersWithoutEmpty);
        
        Assert.Equal(signature1, signature2);
    }
    
    [Fact]
    public void Sign_WithBusinessApi_DoesNotPrependPath()
    {
        // Business APIs use method names like "aliexpress.affiliate.link.generate"
        // These should not have the method name prepended to the signature base string
        var service = new SignatureService("secret");
        var parameters = new Dictionary<string, string>
        {
            ["app_key"] = "test",
            ["param1"] = "value1"
        };
        
        // Should only concatenate parameters, not prepend method name
        var signature = service.Sign("aliexpress.affiliate.link.generate", parameters);
        
        Assert.NotEmpty(signature);
        Assert.Equal(64, signature.Length); // SHA256 hex = 64 chars
    }
}
