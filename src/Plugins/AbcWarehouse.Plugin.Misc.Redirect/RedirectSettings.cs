using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Misc.Redirect.Models;

namespace AbcWarehouse.Plugin.Misc.Redirect
{
    public class RedirectSettings : ISettings
    {
        public bool IsDebugMode { get; private set; }

        public static RedirectSettings FromModel(ConfigModel model)
        {
            return new RedirectSettings()
            {
                IsDebugMode = model.IsDebugMode
            };
        }

        public ConfigModel ToModel()
        {
            var model = new ConfigModel();

            model.IsDebugMode = IsDebugMode;

            return model;
        }
    }
}
