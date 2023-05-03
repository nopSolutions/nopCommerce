using AbcWarehouse.Plugin.Widgets.GA4.Models;
using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.GA4
{
    public class GA4Settings : ISettings
    {
        public string GoogleTag { get; private set; }
        public bool IsDebugMode { get; private set; }

        public static GA4Settings FromModel(ConfigModel model)
        {
            return new GA4Settings()
            {
                GoogleTag = model.GoogleTag,
                IsDebugMode = model.IsDebugMode,
            };
        }

        public ConfigModel ToModel()
        {
            return new ConfigModel
            {
                GoogleTag = GoogleTag,
                IsDebugMode = IsDebugMode,
            };
        }
    }
}