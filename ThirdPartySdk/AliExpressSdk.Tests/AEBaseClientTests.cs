using AliExpressSdk.Clients;
using Xunit;

namespace AliExpressSdk.Tests;

/// <summary>
/// Tests for AEBaseClient to ensure signature and URL generation work correctly.
/// </summary>
public class AEBaseClientTests
{
    [Fact]
    public void Sign_WithEmptySessionParameter_ExcludesFromSignature()
    {
        // Arrange
        var client = new AESystemClient("testkey", "helloworld", "");
        var parameters = new Dictionary<string, string>
        {
            ["method"] = "/auth/token/create",
            ["app_key"] = "12345678",
            ["code"] = "3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26",
            ["sign_method"] = "sha256",
            ["timestamp"] = "1517820392000",
            ["session"] = ""  // Empty session should be excluded
        };

        // Use reflection to access protected Sign method
        var signMethod = typeof(AEBaseClient).GetMethod("Sign", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var signature = (string)signMethod!.Invoke(client, new object[] { parameters })!;

        // Assert - signature should match the documented example when empty session is excluded
        Assert.Equal("35607762342831B6A417A0DED84B79C05FEFBF116969C48AD6DC00279A9F4D81", signature);
    }

    [Fact]
    public void Sign_WithNullParameter_ExcludesFromSignature()
    {
        // Arrange
        var client = new AESystemClient("testkey", "secret", "");
        var parametersWithNull = new Dictionary<string, string>
        {
            ["method"] = "/test/api",
            ["key1"] = "value1",
            ["key2"] = null!,
            ["key3"] = "value3"
        };

        var parametersWithoutNull = new Dictionary<string, string>
        {
            ["method"] = "/test/api",
            ["key1"] = "value1",
            ["key3"] = "value3"
        };

        // Use reflection to access protected Sign method
        var signMethod = typeof(AEBaseClient).GetMethod("Sign", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        var signature1 = (string)signMethod!.Invoke(client, new object[] { parametersWithNull })!;
        var signature2 = (string)signMethod!.Invoke(client, new object[] { parametersWithoutNull })!;

        // Assert
        Assert.Equal(signature1, signature2);
    }

    [Fact]
    public void Assemble_WithEmptySessionParameter_ExcludesFromUrl()
    {
        // Arrange
        var client = new AESystemClient("testkey", "secret", "");
        var parameters = new Dictionary<string, string>
        {
            ["method"] = "/auth/token/create",
            ["app_key"] = "testkey",
            ["code"] = "authcode",
            ["session"] = "",  // Empty session should be excluded
            ["timestamp"] = "123456"
        };

        // Use reflection to access protected Assemble method
        var assembleMethod = typeof(AEBaseClient).GetMethod("Assemble", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        var url = (string)assembleMethod!.Invoke(client, new object[] { parameters })!;

        // Assert
        Assert.DoesNotContain("session=", url);
        Assert.Contains("app_key=testkey", url);
        Assert.Contains("code=authcode", url);
    }
}
