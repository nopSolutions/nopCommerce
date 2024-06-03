using Nop.Core.Configuration;
using Nop.Plugin.Misc.AbcExportOrder.Models;

namespace Nop.Plugin.Misc.AbcExportOrder
{
    public class ExportOrderSettings : ISettings
    {
        public string OrderIdPrefix { get; set; }
        public string TablePrefix { get; set; }
        public string FailureAlertEmail { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(OrderIdPrefix);
            }
        }
    }
}
