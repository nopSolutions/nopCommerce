using Nop.Core.Configuration;
using AbcWarehouse.Plugin.Misc.SearchSpring.Models;
using System.Configuration;
using System.Data.Odbc;

namespace AbcWarehouse.Plugin.Misc.SearchSpring
{
    public class SearchSpringSettings : ISettings
    {
        public bool IsDebugMode { get; private set; }

        public static SearchSpringSettings FromModel(ConfigurationModel model)
        {
            return new SearchSpringSettings()
            {
                IsDebugMode = model.IsDebugMode,
            };
        }

        public ConfigurationModel ToModel()
        {
            return new ConfigurationModel
            {
                IsDebugMode = IsDebugMode,
            };
        }
    }
}