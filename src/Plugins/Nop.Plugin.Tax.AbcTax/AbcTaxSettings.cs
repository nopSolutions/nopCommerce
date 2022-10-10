using Nop.Core.Configuration;

namespace Nop.Plugin.Tax.AbcTax
{
    public class AbcTaxSettings : ISettings
    {
        public string TaxJarAPIToken { get; set; }
        public bool IsDebugMode { get; set; }
    }
}