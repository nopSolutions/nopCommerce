using AbcWarehouse.Plugin.Widgets.PriceSpider.Models;
using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.PriceSpider
{
    public class PriceSpiderSettings : ISettings
    {
        public string MerchantId { get; private set; }

        public static PriceSpiderSettings FromModel(ConfigModel model)
        {
            return new PriceSpiderSettings()
            {
                MerchantId = model.MerchantId,
            };
        }

        public ConfigModel ToModel()
        {
            return new ConfigModel
            {
                MerchantId = MerchantId,
            };
        }
    }
}