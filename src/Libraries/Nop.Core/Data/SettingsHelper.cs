using System;
using Nop.Core.Infrastructure;

namespace Nop.Core.Data
{
    public partial class SettingsHelper
    {
        private static bool? _databaseIsInstalled;
        public static bool DatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new SettingsManager();
                var settings = manager.LoadSettings();
                _databaseIsInstalled = settings != null && !String.IsNullOrEmpty(settings.DataConnectionString);
            }
            return _databaseIsInstalled.Value;
        }

        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }
    }
}
