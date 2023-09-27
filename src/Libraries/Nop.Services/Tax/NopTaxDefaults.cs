namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents default values related to tax services
    /// </summary>
    public static partial class NopTaxDefaults
    {
        /// <summary>
        /// Gets the URL for validate UK VAT number
        /// </summary>
        public static string UKVatValidateUrl => "https://api.service.hmrc.gov.uk/organisations/vat/check-vat-number/lookup/{0}";
    }
}
