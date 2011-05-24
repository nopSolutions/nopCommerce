using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Feed.Froogle.Models
{
    public class FeedFroogleModel
    {
        public FeedFroogleModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
        }

        //TODO set default values - 125
        [NopResourceDisplayName("Plugins.Feed.Froogle.ProductPictureSize")]
        public int ProductPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.Currency")]
        public int CurrencyId { get; set; }

        public IList<SelectListItem> AvailableCurrencies { get; set; }

        //TODO set default values - ftp://uploads.google.com
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpHostname")]
        public string FtpHostname { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpFilename")]
        public string FtpFilename { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpUsername")]
        public string FtpUsername { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpPassword")]
        public string FtpPassword { get; set; }

        public string GenerateFeedResult { get; set; }
    }
}