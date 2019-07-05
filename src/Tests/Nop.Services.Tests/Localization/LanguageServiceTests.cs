using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Localization
{
    [TestFixture]
    public class LanguageServiceTests : ServiceTest
    {
        private Mock<IRepository<Language>> _languageRepo;
        private Mock<IStoreMappingService> _storeMappingService;
        private ILanguageService _languageService;
        private Mock<ISettingService> _settingService;
        private Mock<IEventPublisher> _eventPublisher;
        private LocalizationSettings _localizationSettings;

        [SetUp]
        public new void SetUp()
        {
            _languageRepo = new Mock<IRepository<Language>>();
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

            _languageRepo.Setup(x => x.Table).Returns(new List<Language> { lang1, lang2 }.AsQueryable());

            _storeMappingService = new Mock<IStoreMappingService>();

            var cacheManager = new TestCacheManager();

            _settingService = new Mock<ISettingService>();

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _localizationSettings = new LocalizationSettings();
            _languageService = new LanguageService(_eventPublisher.Object, _languageRepo.Object,_settingService.Object, cacheManager, _storeMappingService.Object, _localizationSettings);
        }

        [Test]
        public void Can_get_all_languages()
        {
            var languages = _languageService.GetAllLanguages();
            languages.ShouldNotBeNull();
            languages.Any().ShouldBeTrue();
        }
    }
}
