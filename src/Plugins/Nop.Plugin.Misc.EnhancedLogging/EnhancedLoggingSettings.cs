using Nop.Core.Configuration;
using Nop.Plugin.Misc.EnhancedLogging.Models;

namespace Nop.Plugin.Misc.EnhancedLogging
{
    public class EnhancedLoggingSettings : ISettings
    {
        public int DaysToKeepLogs { get; private set; }

        public EnhancedLoggingConfigModel ToModel()
        {
            return new EnhancedLoggingConfigModel()
            {
                DaysToKeepLogs = DaysToKeepLogs
            };
        }

        public static EnhancedLoggingSettings CreateDefault()
        {
            return new EnhancedLoggingSettings()
            {
                DaysToKeepLogs = 14
            };
        }

        public static EnhancedLoggingSettings FromModel(EnhancedLoggingConfigModel model)
        {
            return new EnhancedLoggingSettings()
            {
                DaysToKeepLogs = model.DaysToKeepLogs
            };
        }
    }
}
