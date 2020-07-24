using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core.Domain.Localization;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Localization
{
    [TestFixture]
    public class LanguageServiceTests : ServiceTest
    {
        private FakeRepository<Language> _languageRepo;
        private Mock<IStoreMappingService> _storeMappingService;
        private ILanguageService _languageService;
        private Mock<ISettingService> _settingService;
        private LocalizationSettings _localizationSettings;

        [SetUp]
        public new void SetUp()
        {
            var lang1 = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            var lang2 = new Language
            {
                Name = "Russian",
                LanguageCulture = "ru-Ru",
                FlagImageFileName = "ru.png",
                Published = true,
                DisplayOrder = 2
            };

            _languageRepo = new FakeRepository<Language>(new List<Language> { lang1, lang2 });

            _storeMappingService = new Mock<IStoreMappingService>();

            _settingService = new Mock<ISettingService>();

            _localizationSettings = new LocalizationSettings();
            _languageService = new LanguageService(_languageRepo,_settingService.Object, new TestCacheManager(),  _storeMappingService.Object, _localizationSettings);
        }

        [Test]
        public void Can_get_all_languages()
        {
            RunWithTestServiceProvider(() =>
            {
                var languages = _languageService.GetAllLanguages();
                languages.Should().NotBeNull();
                languages.Any().Should().BeTrue();
            });
        }
    }
}
