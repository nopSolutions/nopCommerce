using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Feed.PriceGrabber.Models
{
    public class FeedPriceGrabberModel
    {
        public FeedPriceGrabberModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Feed.PriceGrabber.ProductPictureSize")]
        public int ProductPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.Feed.PriceGrabber.Currency")]
        public int CurrencyId { get; set; }

        public IList<SelectListItem> AvailableCurrencies { get; set; }

        public string GenerateFeedResult { get; set; }
    }
}