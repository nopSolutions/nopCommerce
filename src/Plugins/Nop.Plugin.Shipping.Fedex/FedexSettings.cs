
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.Fedex
{
    public class FedexSettings : ISettings
    {
        public string Url { get; set; }

        public string Key { get; set; }

        public string Password { get; set; }

        public string AccountNumber { get; set; }

        public string MeterNumber { get; set; }

        public DropoffType DropoffType { get; set; }

        public bool UseResidentialRates { get; set; }

        public bool ApplyDiscounts { get; set; }

        public decimal AdditionalHandlingCharge { get; set; }

        public string CarrierServicesOffered { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string StateOrProvinceCode { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public bool PassDimensions { get; set; }

        public int PackingPackageVolume { get; set; }

        public PackingType PackingType { get; set; }
    }
}