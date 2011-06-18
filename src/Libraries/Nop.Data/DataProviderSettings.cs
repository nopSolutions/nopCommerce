using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;

namespace Nop.Data
{
    public partial class DataProviderSettings
    {
        public string DataProvider { get; set; }
        public string DataConnectionString { get; set; }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.DataProvider) && !String.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
