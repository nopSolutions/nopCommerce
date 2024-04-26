using FluentAssertions;
using Nop.Services.Themes;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Themes;

[TestFixture]
public class ThemeProviderTests : BaseNopTest
{
    private IThemeProvider _themeProvider;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _themeProvider = GetService<IThemeProvider>();
    }

    [Test]
    public void CanGetThemeDescriptorFromText()
    {
        var descriptor = "{ \"SystemName\": \"Test system name\", \"FriendlyName\": \"Test\", \"SupportRTL\": false, \"PreviewImageUrl\": \"~/Themes/Test/preview.jpg\", \"PreviewText\": \"The 'Test' site theme\" }";

        var themeDescriptor = _themeProvider.GetThemeDescriptorFromText(descriptor);

        themeDescriptor.Should().NotBeNull();
        themeDescriptor.SystemName.Should().BeEquivalentTo("Test system name");
    }

    [Test]
    public async Task CanGetThemes()
    {
        var themes = await _themeProvider.GetThemesAsync();

        themes.Count.Should().BeGreaterThan(0);
    }


    [Test]
    public async Task CanGetThemeBySystemName()
    {
        var themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("Test system name");
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(string.Empty);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(null);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("DefaultClean");
        themeDescriptor.Should().NotBeNull();
        themeDescriptor.FriendlyName.Should().BeEquivalentTo("Default clean");
    }


    [Test]
    public async Task CanThemeExists()
    {
        var isExists = await _themeProvider.ThemeExistsAsync("Test system name");
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync(string.Empty);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync(null);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync("DefaultClean");
        isExists.Should().BeTrue();
    }
}