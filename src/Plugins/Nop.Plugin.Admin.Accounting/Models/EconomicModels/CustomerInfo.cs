using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class CustomerInfo
    {
        [JsonProperty("barred")]
        public bool Barred { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("corporateIdentificationNumber")]
        public string CorporateIdentificationNumber { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("creditLimit")]
        public decimal CreditLimit { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("customerGroup")]
        public CustomerGroup CustomerGroup { get; set; }

        [JsonProperty("customerNumber")]
        public int CustomerNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("vatZone")]
        public VatZone VatZone { get; set; }

        [JsonProperty("layout")]
        public Layout Layout { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("paymentTerms")]
        public PaymentTerms PaymentTerms { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("publicEntryNumber")]
        public string PublicEntryNumber { get; set; }

        [JsonProperty("telephoneAndFaxNumber")]
        public string TelephoneAndFaxNumber { get; set; }

        [JsonProperty("mobilePhone")]
        public string MobilePhone { get; set; }

        [JsonProperty("eInvoicingDisabledByDefault")]
        public bool EInvoicingDisabledByDefault { get; set; }

        [JsonProperty("vatNumber")]
        public string VatNumber { get; set; }
    }
}