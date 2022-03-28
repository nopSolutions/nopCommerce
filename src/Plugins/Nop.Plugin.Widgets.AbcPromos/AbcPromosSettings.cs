using Nop.Core.Configuration;
using Nop.Plugin.Widgets.AbcPromos.Models;

namespace Nop.Plugin.Widgets.AbcPromos
{
    public class AbcPromosSettings : ISettings
    {
        public bool IncludeExpiredPromosOnRebatesPromosPage { get; private set; }

        internal ConfigModel ToModel()
        {
            return new ConfigModel()
            {
                IncludeExpiredPromosOnRebatesPromosPage = IncludeExpiredPromosOnRebatesPromosPage,
            };
        }

        internal static AbcPromosSettings FromModel(ConfigModel model)
        {
            return new AbcPromosSettings()
            {
                IncludeExpiredPromosOnRebatesPromosPage = model.IncludeExpiredPromosOnRebatesPromosPage
            };
        }
    }
}
