
using Nop.Core.Configuration;

namespace Nop.Plugin.Feed.PriceGrabber
{
    public class PriceGrabberSettings : ISettings
    {
        public int ProductPictureSize { get; set; }

        public int CurrencyId { get; set; }
    }
}