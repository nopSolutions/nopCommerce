using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Payments.UniFi.Models;

namespace AbcWarehouse.Plugin.Payments.UniFi
{
    public class UniFiPaymentsSettings : ISettings
    {
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public bool UseIntegration { get; private set; }
        public bool IsDebugMode { get; private set; }

        public static UniFiPaymentsSettings FromModel(ConfigModel model)
        {
            return new UniFiPaymentsSettings()
            {
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret,
                UseIntegration = model.UseIntegration,
                IsDebugMode = model.IsDebugMode
            };
        }

        public ConfigModel ToModel()
        {
            var model = new ConfigModel();

            model.ClientId = ClientId;
            model.ClientSecret = ClientSecret;
            model.UseIntegration = UseIntegration;
            model.IsDebugMode = IsDebugMode;

            return model;
        }
    }
}
