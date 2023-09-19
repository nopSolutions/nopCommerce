using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Payments.UniFi.Models
{
    public class ConfigModel
    {
        [NopResourceDisplayName(UniFiPaymentsLocales.ClientId)]
        public string ClientId { get; set; }

        [NopResourceDisplayName(UniFiPaymentsLocales.ClientSecret)]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName(UniFiPaymentsLocales.UseIntegration)]
        public bool UseIntegration { get; set; }
    }
}
