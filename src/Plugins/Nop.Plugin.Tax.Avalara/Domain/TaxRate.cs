namespace Nop.Plugin.Tax.Avalara.Domain
{
    /// <summary>
    /// Represents a tax rate record
    /// </summary>
    public class TaxRate
    {
        /// <summary>
        /// Gets or sets the five digit zip code
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the two character US state abbreviation
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the county name
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets the city name
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state component of the sales tax rate
        /// </summary>
        public decimal StateTax { get; set; }

        /// <summary>
        /// Gets or sets the county component of the sales tax rate
        /// </summary>
        public decimal CountyTax { get; set; }

        /// <summary>
        /// Gets or sets the city component of the sales tax rate
        /// </summary>
        public decimal CityTax { get; set; }

        /// <summary>
        /// Gets or sets the otal tax rate for sales tax for this postal code. This value may not equal the sum of the state/county/city due to special tax jurisdiction rules
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the shipping is taxable
        /// </summary>
        public bool ShippingTaxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the shipping and handling are taxable when sent together
        /// </summary>
        public bool ShippingAndHadlingTaxable { get; set; }
    }
}