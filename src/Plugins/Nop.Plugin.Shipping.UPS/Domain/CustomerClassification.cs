namespace Nop.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// Represents customer classification
    /// </summary>
    /// <remarks>
    /// Updated at January 7, 2019
    /// </remarks>
    public enum CustomerClassification
    {
        /// <summary>
        /// Rates Associated with Shipper Number
        /// </summary>
        [UPSCode("00")]
        RatesAssociatedWithShipperNumber,

        /// <summary>
        /// Daily Rates
        /// </summary>
        [UPSCode("01")]
        DailyRates,

        /// <summary>
        /// Retail Rates
        /// </summary>
        [UPSCode("04")]
        RetailRates,

        /// <summary>
        /// Regional Rates
        /// </summary>
        [UPSCode("05")]
        RegionalRates,

        /// <summary>
        /// General List Rates
        /// </summary>
        [UPSCode("06")]
        GeneralListRates,

        /// <summary>
        /// Standard List Rates
        /// </summary>
        [UPSCode("53")]
        StandardListRates
    }
}