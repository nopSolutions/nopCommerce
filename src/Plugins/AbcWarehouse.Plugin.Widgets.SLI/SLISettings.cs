using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.SLI
{
    public class SLISettings : ISettings
    {
        public string ActionUrl { get; private set; }

        public string CookieName { get; private set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(ActionUrl) &&
                       !string.IsNullOrEmpty(CookieName);
            }
        }

        internal static SLISettings FromModel(ConfigModel model)
        {
            return new SLISettings()
            {
                ActionUrl = model.ActionUrl,
                CookieName = model.CookieName,
            };
        }

        internal ConfigModel ToModel()
        {
            return new ConfigModel()
            {
                ActionUrl = ActionUrl,
                CookieName = CookieName,
            };
        }
    }
}
