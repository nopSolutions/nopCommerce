using FluentAssertions;
using Nop.Services.Seo;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Seo;

[TestFixture]
public class SeoExtensionsTests : ServiceTest
{
    private IUrlRecordService _urlRecordService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _urlRecordService = GetService<IUrlRecordService>();
    }

    [Test]
    public async Task ShouldReturnLowercase()
    {
        var seName = await _urlRecordService.GetSeNameAsync("tEsT", false, false);
        seName.Should().Be("test");
    }

    [Test]
    public async Task ShouldAllowAllLatinChars()
    {
        var seName = await _urlRecordService.GetSeNameAsync("abcdefghijklmnopqrstuvwxyz1234567890", false, false);
        seName.Should().Be("abcdefghijklmnopqrstuvwxyz1234567890");
    }

    [Test]
    public async Task ShouldRemoveIllegalChars()
    {
        var seName = await _urlRecordService.GetSeNameAsync("test!@#$%^&*()+<>?/", false, false);
        seName.Should().Be("test");
    }

    [Test]
    public async Task ShouldReplaceSpaceWithDash()
    {
        var seName = await _urlRecordService.GetSeNameAsync("test test", false, false);
        seName.Should().Be("test-test");
        seName = await _urlRecordService.GetSeNameAsync("test     test", false, false);
        seName.Should().Be("test-test");
    }

    [Test]
    public async Task CanConvertNonWesternChars()
    {
        //German letters with diacritics
        var seName = await _urlRecordService.GetSeNameAsync("testäöü", true, false);
        seName.Should().Be("testaou");
        seName = await _urlRecordService.GetSeNameAsync("testäöü", false, false);
        seName.Should().Be("test");
    }

    [Test]
    public async Task CanAllowUnicodeChars()
    {
        //Russian letters
        var seName = await _urlRecordService.GetSeNameAsync("testтест", false, true);
        seName.Should().Be("testтест");
        seName = await _urlRecordService.GetSeNameAsync("testтест", false, false);
        seName.Should().Be("test");
    }
}