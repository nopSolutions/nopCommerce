using Nop.Core.Configuration;

namespace Nop.Plugin.Feed.Froogle
{
    public class FroogleSettings : ISettings
    {
        /// <summary>
        /// Product picture size
        /// </summary>
        public int ProductPictureSize { get; set; }

        /// <summary>
        /// A value indicating whether we should pass shipping info to Froogle
        /// </summary>
        public bool PassShippingInfo { get; set; }

        /// <summary>
        /// A value indicating whether we should calculate prices considering promotions (tier prices, discounts, special prices, etc)
        /// </summary>
        public bool PricesConsiderPromotions { get; set; }

        /// <summary>
        /// Store identifier for which feed file(s) will be generated
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// Currency identifier for which feed file(s) will be generated
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Default Google category
        /// </summary>
        public string DefaultGoogleCategory { get; set; }
       
        /// <summary>
        /// Static Froogle file name
        /// </summary>
        public string StaticFileName { get; set; }
    }
}