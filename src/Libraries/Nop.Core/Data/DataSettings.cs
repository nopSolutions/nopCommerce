using System;
using System.Collections.Generic;

namespace Nop.Core.Data
{
    /// <summary>
    /// Data settings (connection string information)
    /// </summary>
    public partial class DataSettings
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DataSettings()
        {
            RawDataSettings=new Dictionary<string, string>();
        }

        /// <summary>
        /// Data provider
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Raw settings file
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; private set; }

        /// <summary>
        /// A value indicating whether entered information is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.DataProvider) && !String.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
