using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Tests
{
    public class TestLocalizationService : LocalizationService
    {
        public TestLocalizationService(IStaticCacheManager cacheManager, ILanguageService languageService, ILocalizedEntityService localizedEntityService, ILogger logger, IWorkContext workContext, IRepository<LocaleStringResource> lsrRepository, IDataProvider dataProvider, IDbContext dbContext, ISettingService settingService, CommonSettings commonSettings, LocalizationSettings localizationSettings, IEventPublisher eventPublisher) :
            base(cacheManager, languageService, localizedEntityService, logger, workContext, lsrRepository, dataProvider, dbContext, settingService, commonSettings, localizationSettings, eventPublisher)
        {
        }

        public static TestLocalizationService Init()
        {
            return new TestLocalizationService(new  Mock<IStaticCacheManager>().Object,
                new  Mock<ILanguageService>().Object,
                new  Mock<ILocalizedEntityService>().Object,
                new  Mock<ILogger>().Object,
                new  Mock<IWorkContext>().Object,
                new  Mock<IRepository<LocaleStringResource>>().Object,
                new  Mock<IDataProvider>().Object,
                new  Mock<IDbContext>().Object,
                new  Mock<ISettingService>().Object,
                new CommonSettings(), 
                new LocalizationSettings(), 
                new  Mock<IEventPublisher>().Object);
        }

        public override string GetResource(string resourceKey)
        {
            var rez = string.Empty;

            switch (resourceKey)
            {
                    case "GiftCardAttribute.For.Virtual":
                        rez = "For: {0} <{1}>";
                        break;
                case "GiftCardAttribute.From.Virtual":
                    rez = "From: {0} <{1}>";
                    break;
                case "GiftCardAttribute.For.Physical":
                    rez = "For: {0}";
                    break;
                case "GiftCardAttribute.From.Physical":
                    rez = "From: {0}";
                    break;
            }

            return rez;
        }
    }
}
