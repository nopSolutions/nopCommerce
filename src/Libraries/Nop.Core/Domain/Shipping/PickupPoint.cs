namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Pickup point
    /// </summary>
    public partial class PickupPoint
    {
        /// <summary>
        /// Gets or sets an identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a system name of the pickup point provider
        /// </summary>
        public string ProviderSystemName { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a county
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets a state abbreviation
        /// </summary>
        public string StateAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets a two-letter ISO country code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets a zip postal code
        /// </summary>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets a latitude
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets a longitude
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Gets or sets a fee for the pickup
        /// </summary>
        public decimal PickupFee { get; set; }

        /// <summary>
        /// Gets or sets an opening hours
        /// </summary>
        public string OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a transit days
        /// </summary>
        public int? TransitDays { get; set; }
    }
}
