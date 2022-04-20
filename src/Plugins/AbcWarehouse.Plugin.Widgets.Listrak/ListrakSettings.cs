using AbcWarehouse.Plugin.Widgets.Listrak.Models;
using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.Listrak
{
    public class ListrakSettings : ISettings
    {
        public string MerchantId { get; private set; }

        public static ListrakSettings FromModel(ConfigModel model)
        {
            return new ListrakSettings()
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