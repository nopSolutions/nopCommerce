using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Core.Domain.Localization;
using Rhino.Mocks;

namespace Nop.Services.Tests.Localization
{
    [TestFixture]
    public class LanguageServiceTests
    {
        IRepository<Language> _languageRepo;
        ILanguageService _languageService;
        ISettingService _settingService;
        LocalizationSettings _localizationSettings;

        [SetUp]
        public void SetUp()
        {
            _languageRepo = MockRepository.GenerateMock<IRepository<Language>>();
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

            _languageRepo.Expect(x => x.Table).Return(new List<Language>() { lang1, lang2 }.AsQueryable());

            var cacheManager = new NopNullCache();

            _settingService = MockRepository.GenerateMock<ISettingService>();

            _localizationSettings = new LocalizationSettings();
            _languageService = new LanguageService(cacheManager, _languageRepo, _settingService, _localizationSettings);
        }

        [Test]
        public void Can_get_all_languages()
        {
            var languages = _languageService.GetAllLanguages();
            languages.ShouldNotBeNull();
            (languages.Count > 0).ShouldBeTrue();
        }
    }
}
