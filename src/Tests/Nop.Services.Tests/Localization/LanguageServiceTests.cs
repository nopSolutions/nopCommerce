using System.Linq;
using FluentAssertions;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Services.Tests.Localization
{
    [TestFixture]
    public class LanguageServiceTests : ServiceTest
    {
        private ILanguageService _languageService;

        [SetUp]
        public void SetUp()
        {
            _languageService = GetService<ILanguageService>();
        }

        [Test]
        public void CanGetAllLanguages()
        {
            var languages = _languageService.GetAllLanguages();
            languages.Should().NotBeNull();
            languages.Any().Should().BeTrue();
        }
    }
}
