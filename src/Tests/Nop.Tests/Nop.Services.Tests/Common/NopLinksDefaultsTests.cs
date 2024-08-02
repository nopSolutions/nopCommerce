using System.Net;
using System.Reflection;
using FluentAssertions;
using Nop.Core.Http;
using NUnit.Framework;
using static Nop.Services.Common.NopLinksDefaults;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
internal class NopLinksDefaultsTests : ServiceTest
{
    private IHttpClientFactory _httpClientFactory;

    [OneTimeSetUp]
    public void SetUp()
    {
        _httpClientFactory = GetService<IHttpClientFactory>();
    }

    protected async Task TestUrlsAsync(IList<PropertyInfo> properties)
    {
        var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);

        foreach (var propertyInfo in properties)
        {
            var url = propertyInfo.GetValue(null)?.ToString();

            if (string.IsNullOrEmpty(url))
                continue;

            var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

            res.StatusCode.Should().BeOneOf(new[]
            {
                HttpStatusCode.OK , HttpStatusCode.Found
            }, $"{url} {res.ReasonPhrase}");
        }
    }

    [Test]
    public async Task TestOfficialSiteLinks()
    {
        var prop = typeof(OfficialSite).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
            .Where(p => p.PropertyType == typeof(string)).ToList();

        prop.Should().NotBeEmpty();

        await TestUrlsAsync(prop);
    }

    [Test]
    public async Task TestDocsLinks()
    {
        var prop = typeof(Docs).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
            .Where(p => p.PropertyType == typeof(string)).ToList();

        prop.Should().NotBeEmpty();

        await TestUrlsAsync(prop);
    }
}