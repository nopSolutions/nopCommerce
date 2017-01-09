using Nop.Core.Configuration;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip
{
    public class FixedOrByCountryStateZipTaxSettings : ISettings
    {
        public bool CountryStateZipEnabled { get; set; }
    }
}