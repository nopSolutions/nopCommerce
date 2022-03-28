using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.AbcCore
{

    public static class HtmlHelpers
    {
        private const string PAVILIONPRIMARYCOLOR_KEY = "Nop.pavilion.color.primary";
        private const string PAVILIONACCENTCOLOR_KEY = "Nop.pavilion.color.accent";
        private const string CANONICALURLSENABLED_KEY = "Nop.settings.canonicalurlsenabled";
        private const string BREADCRUMBDELIMITER_KEY = "Nop.settings.breadcrumbdelimiter";
        private const string GIFTCARDURL_KEY = "Nop.settings.giftcardurl";

        public static async Task<string> GetPavilionPrimaryColorAsync()
        {
            var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            return await staticCacheManager.Get(new CacheKey(PAVILIONPRIMARYCOLOR_KEY, "Abc."), async () =>
            {
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                string preset = await settingService.GetSettingByKeyAsync<string>("pavilionthemesettings.preset");
                var presetParts = preset.Split(',');
                return presetParts[1].Trim();
            });

        }

        public static async Task<string> GetPavilionAccentColorAsync()
        {
            var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            return await staticCacheManager.Get(new CacheKey(PAVILIONACCENTCOLOR_KEY, "Abc."), async () =>
            {
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                string preset = await settingService.GetSettingByKeyAsync<string>("pavilionthemesettings.preset");
                var presetParts = preset.Split(',');
                return presetParts[0].Trim();
            });
        }

        public static bool CanonicalUrlsEnabled()
        {
            var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            return staticCacheManager.Get(new CacheKey(CANONICALURLSENABLED_KEY, "Abc."), () =>
            {
                return EngineContext.Current.Resolve<SeoSettings>().CanonicalUrlsEnabled;
            });
        }

        public static string GetBreadcrumbDelimiter()
        {
            var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            return staticCacheManager.Get(new CacheKey(BREADCRUMBDELIMITER_KEY, "Abc."), () =>
            {
                return EngineContext.Current.Resolve<CommonSettings>().BreadcrumbDelimiter;
            });
        }

        public static string GetGiftCardUrl()
        {
            var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            return staticCacheManager.Get(new CacheKey(GIFTCARDURL_KEY, "Abc."), () =>
            {
                return "/GIFT";
            });
        }

        public static string GetSelectedShopClass(int selectedShopId, int shopId)
        {
            return selectedShopId == shopId ? "selected-store" : "";
        }
    }
}
