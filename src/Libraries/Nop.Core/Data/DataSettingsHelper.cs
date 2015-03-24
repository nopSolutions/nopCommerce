using System;

namespace Nop.Core.Data
{
    /// <summary>
    /// Data settings helper
    /// </summary>
    public partial class DataSettingsHelper
    {
        private static bool? _databaseIsInstalled;

        /// <summary>
        /// Returns a value indicating whether database is already installed
        /// </summary>
        /// <returns></returns>
        public static bool DatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new DataSettingsManager();
                var settings = manager.LoadSettings();
                _databaseIsInstalled = settings != null && !String.IsNullOrEmpty(settings.DataConnectionString);
            }
            return _databaseIsInstalled.Value;
        }

        //Reset information cached in the "DatabaseIsInstalled" method
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }
    }
}
