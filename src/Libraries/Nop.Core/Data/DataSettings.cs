using System;
using System.Collections.Generic;

namespace Nop.Core.Data
{
    public partial class DataSettings
    {
        public DataSettings()
        {
            RawDataSettings=new Dictionary<string, string>();
        }
        public string DataProvider { get; set; }
        public string DataConnectionString { get; set; }
        public IDictionary<string, string> RawDataSettings { get; private set; }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.DataProvider) && !String.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
