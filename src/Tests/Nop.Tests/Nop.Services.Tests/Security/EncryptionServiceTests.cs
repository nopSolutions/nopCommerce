using FluentAssertions;
using Nop.Core.Domain.Security;
using Nop.Services.Configuration;
using Nop.Services.Security;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Security;

[TestFixture]
public class EncryptionServiceTests : ServiceTest
{
    private IEncryptionService _encryptionService;
    private SecuritySettings _securitySettings;
    private ISettingService _settingService;
    private string _defaultEncryptionKey;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _securitySettings = GetService<SecuritySettings>();
        _settingService = GetService<ISettingService>();

        _defaultEncryptionKey = _securitySettings.EncryptionKey;
        _securitySettings.EncryptionKey = "273ece6f97dd844d";
        await _settingService.SaveSettingAsync(_securitySettings);

        _encryptionService = GetService<IEncryptionService>();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _securitySettings.EncryptionKey = _defaultEncryptionKey;
        await _settingService.SaveSettingAsync(_securitySettings);
    }

    [Test]
    public void CanHashSha1()
    {
        var password = "MyLittleSecret";
        var saltKey = "salt1";
        var hashedPassword = _encryptionService.CreatePasswordHash(password, saltKey, "SHA1");
        hashedPassword.Should().Be("A07A9638CCE93E48E3F26B37EF7BDF979B8124D6");
    }

    [Test]
    public void CanHashSha512()
    {
        var password = "MyLittleSecret";
        var saltKey = "salt1";
        var hashedPassword = _encryptionService.CreatePasswordHash(password, saltKey, "SHA512");
        hashedPassword.Should().Be("4506D65FDB6F3A8CF97278AB7C5C62DEC35EADD474BE1E6243776691D56E1B27F71C1D9085B26BD7513BED89822204D6B8FCBD6E665D46558C48F56D21B2A293");
    }

    [Test]
    public void CanEncryptAndDecrypt()
    {
        var password = "MyLittleSecret";
        var encryptedPassword = _encryptionService.EncryptText(password);
        var decryptedPassword = _encryptionService.DecryptText(encryptedPassword);
        decryptedPassword.Should().Be(password);
    }
}