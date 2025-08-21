using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using static Nop.Services.Common.NopLinksDefaults;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
internal class NopLinksDefaultsTests : ServiceTest
{
    protected static async Task TestUrlsAsync(IList<PropertyInfo> properties)
    {
        //skip external URL validation in automated test environments
        //instead, just validate that the URLs are well-formed
        foreach (var propertyInfo in properties)
        {
            var url = propertyInfo.GetValue(null)?.ToString();

            if (string.IsNullOrEmpty(url))
                continue;

            //validate that the URL is well-formed
            Uri.IsWellFormedUriString(url, UriKind.Absolute)
                .Should().BeTrue($"URL '{url}' from property '{propertyInfo.Name}' should be well-formed");

            //validate that it's an HTTPS URL for security
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                uri.Scheme.Should().Be("https", $"URL '{url}' should use HTTPS for security");
            }
        }
        await Task.CompletedTask;
    }

    [Test]
    public async Task TestOfficialSiteLinksAsync()
    {
        var prop = typeof(OfficialSite).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
            .Where(p => p.PropertyType == typeof(string)).ToList();

        prop.Should().NotBeEmpty();

        await TestUrlsAsync(prop);
    }

    [Test]
    public async Task TestDocsLinksAsync()
    {
        var prop = typeof(Docs).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
            .Where(p => p.PropertyType == typeof(string)).ToList();

        prop.Should().NotBeEmpty();

        await TestUrlsAsync(prop);
    }
}