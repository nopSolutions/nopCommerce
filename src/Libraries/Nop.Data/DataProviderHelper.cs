using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;

namespace Nop.Data
{
    public partial class DataProviderHelper
    {
        private static bool? _databaseIsInstalled;
        public static bool DatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new DataProviderManager();
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
