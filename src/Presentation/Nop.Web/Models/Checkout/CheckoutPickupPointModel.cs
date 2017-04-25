using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutPickupPointModel : BaseNopModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProviderSystemName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string StateName { get; set; }

        public string CountryName { get; set; }

        public string ZipPostalCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string PickupFee { get; set; }

        public string OpeningHours { get; set; }
    }
}