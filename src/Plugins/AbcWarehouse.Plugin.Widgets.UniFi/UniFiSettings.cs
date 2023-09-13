using AbcWarehouse.Plugin.Widgets.UniFi.Models;
using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.UniFi
{
    public class UniFiSettings : ISettings
    {
        public string ProviderId { get; private set; }

        public static UniFiSettings FromModel(ConfigModel model)
        {
            return new UniFiSettings()
            {
                ProviderId = model.ProviderId,
            };
        }

        public ConfigModel ToModel()
        {
            return new ConfigModel
            {
                ProviderId = ProviderId,
            };
        }
    }
}