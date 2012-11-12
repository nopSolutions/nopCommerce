using Nop.Core.Configuration;

namespace Nop.Plugin.Feed.Froogle
{
    public class FroogleSettings : ISettings
    {
        public int ProductPictureSize { get; set; }

        public int CurrencyId { get; set; }
        public bool PassShippingInfo { get; set; }

        public string DefaultGoogleCategory { get; set; }
       
        /// <summary>
        /// Static Froogle file name
        /// </summary>
        public string StaticFileName { get; set; }
    }
}