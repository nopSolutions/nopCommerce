using System.Collections.Generic;
using AliExpressSdk.Clients;
using Xunit;

namespace AliExpressSdk.Tests;

public class SigningTests
{
    private class TestClient : AEBaseClient
    {
        public TestClient() : base("test", "secret", "session") { }
        public string DoSign(IDictionary<string, string> p) => Sign(p);
    }

    [Fact]
    public void Sign_ComputesExpectedHash()
    {
        var client = new TestClient();
        var parameters = new Dictionary<string, string>
        {
            ["method"] = "/auth/token/create",
            ["app_key"] = "test",
            ["session"] = "session",
            ["timestamp"] = "12345",
            ["simplify"] = "true",
            ["sign_method"] = "sha256"
        };
        var sign = client.DoSign(parameters);
        Assert.Equal("083482F9A0CE8559B567E46222AA1401B09BBDACC409D0BDA77A9A385A0BD31C", sign);
    }
}
