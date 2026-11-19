using System.Net;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Security;
using Nop.Web.Framework.Infrastructure.Extensions;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Infrastructure.Extensions;

[TestFixture]
public class HttpClientBuilderExtensionsTests
{
    private const string CLIENT_NAME = "proxy-test";

    [Test]
    public void WithProxyConfiguresTheProxyFromSettings()
    {
        var handler = GetPrimaryHandler(() => new ProxySettings
        {
            Enabled = true,
            Address = "http://127.0.0.1",
            Port = "8888"
        });

        handler.Proxy.Should().BeOfType<WebProxy>()
            .Which.Address.Should().Be(new Uri("http://127.0.0.1:8888"));
    }

    [Test]
    public void WithProxyUsesADirectConnectionWhenProxySettingsCannotBeLoaded()
    {
        var handler = GetPrimaryHandler(() => throw new InvalidOperationException("The database is not available"));

        handler.Proxy.Should().BeNull();
    }

    private static HttpClientHandler GetPrimaryHandler(Func<ProxySettings> loadProxySettings)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => loadProxySettings());
        services.AddHttpClient(CLIENT_NAME).WithProxy();

        var handler = services.BuildServiceProvider()
            .GetRequiredService<IHttpMessageHandlerFactory>()
            .CreateHandler(CLIENT_NAME);

        while (handler is DelegatingHandler delegatingHandler)
            handler = delegatingHandler.InnerHandler;

        return handler.Should().BeOfType<HttpClientHandler>().Subject;
    }
}
