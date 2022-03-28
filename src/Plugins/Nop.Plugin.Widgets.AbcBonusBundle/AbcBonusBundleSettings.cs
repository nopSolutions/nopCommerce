using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcBonusBundle
{
    public class AbcBonusBundleSettings : ISettings
    {
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
