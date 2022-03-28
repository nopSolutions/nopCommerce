using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Synchrony.Models;
using Nop.Services.Configuration;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Synchrony
{
    public class SynchronyPaymentSettings : ISettings
    {
        public string MerchantId { get; private set; }
        public string MerchantPassword { get; private set; }
        public string TokenNumber { get; private set; }
        public bool Integration { get; private set; }
        public string WhitelistDomain { get; private set; }
        public bool IsDebugMode { get; private set; }

        public static SynchronyPaymentSettings FromModel(ConfigurationModel model)
        {
            return new SynchronyPaymentSettings()
            {
                MerchantId = model.MerchantId,
                MerchantPassword = model.MerchantPassword,
                Integration = model.Integration,
                TokenNumber = model.TokenNumber,
                WhitelistDomain = model.WhitelistDomain,
                IsDebugMode = model.IsDebugMode
            };
        }

        public static SynchronyPaymentSettings Default()
        {
            return new SynchronyPaymentSettings
            {
                MerchantId = "",
                MerchantPassword = "",
                Integration = true,
                WhitelistDomain = ""
            };
        }

        public async Task<ConfigurationModel> ToModelAsync(int storeScope)
        {
            var model = new ConfigurationModel();

            model.MerchantId = MerchantId;
            model.MerchantPassword = MerchantPassword;
            model.Integration = Integration;
            model.TokenNumber = TokenNumber;
            model.WhitelistDomain = WhitelistDomain;
            model.IsDebugMode = IsDebugMode;

            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                model.MerchantId_OverrideForStore = await settingService.SettingExistsAsync(this, x => x.MerchantId, storeScope);
                model.MerchantPassword_OverrideForStore = await settingService.SettingExistsAsync(this, x => x.MerchantPassword, storeScope);
                model.Integration_OverrideForStore = await settingService.SettingExistsAsync(this, x => x.Integration, storeScope);
                model.TokenNumber_OverrideForStore = await settingService.SettingExistsAsync(this, x => x.TokenNumber, storeScope);
                model.WhitelistDomain_OverrideForStore = await settingService.SettingExistsAsync(this, x => x.WhitelistDomain, storeScope);
                model.IsDebugMode = await settingService.SettingExistsAsync(this, x => x.IsDebugMode, storeScope);
            }

            return model;
        }
    }
}
