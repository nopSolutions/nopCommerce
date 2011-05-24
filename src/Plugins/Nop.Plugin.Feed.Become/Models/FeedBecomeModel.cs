using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Feed.Become.Models
{
    public class FeedBecomeModel
    {
        public FeedBecomeModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
        }

        //TODO set default values - 125
        [NopResourceDisplayName("Plugins.Feed.Become.ProductPictureSize")]
        public int ProductPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Become.Currency")]
        public int CurrencyId { get; set; }

        public IList<SelectListItem> AvailableCurrencies { get; set; }

        public string GenerateFeedResult { get; set; }
    }
}