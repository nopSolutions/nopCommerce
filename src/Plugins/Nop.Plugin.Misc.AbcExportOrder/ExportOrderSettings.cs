using Nop.Core.Configuration;
using Nop.Plugin.Misc.AbcExportOrder.Models;

namespace Nop.Plugin.Misc.AbcExportOrder
{
    public class ExportOrderSettings : ISettings
    {
        public string OrderIdPrefix { get; private set; }
        public string TablePrefix { get; private set; }
        public string FailureAlertEmail { get; private set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(OrderIdPrefix);
            }
        }

        internal ConfigModel ToModel()
        {
            return new ConfigModel()
            {
                OrderIdPrefix = OrderIdPrefix,
                TablePrefix = TablePrefix,
                FailureAlertEmail = FailureAlertEmail
            };
        }

        internal static ExportOrderSettings FromModel(ConfigModel model)
        {
            return new ExportOrderSettings()
            {
                OrderIdPrefix = model.OrderIdPrefix,
                TablePrefix = model.TablePrefix,
                FailureAlertEmail = model.FailureAlertEmail
            };
        }
    }
}
