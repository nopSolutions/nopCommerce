using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Feed.Froogle.Models
{
    public class FeedFroogleModel
    {
        public FeedFroogleModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
            AvailableGoogleCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Feed.Froogle.ProductPictureSize")]
        public int ProductPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.Currency")]
        public int CurrencyId { get; set; }
        public IList<SelectListItem> AvailableCurrencies { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.DefaultGoogleCategory")]
        public string DefaultGoogleCategory { get; set; }
        public IList<SelectListItem> AvailableGoogleCategories { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpHostname")]
        public string FtpHostname { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpFilename")]
        public string FtpFilename { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpUsername")]
        public string FtpUsername { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.FtpPassword")]
        public string FtpPassword { get; set; }

        public string GenerateFeedResult { get; set; }
        public string SaveResult { get; set; }



        [NopResourceDisplayName("Plugins.Feed.Froogle.TaskEnabled")]
        public bool TaskEnabled { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.GenerateStaticFileEachMinutes")]
        public int GenerateStaticFileEachMinutes { get; set; }
        [NopResourceDisplayName("Plugins.Feed.Froogle.StaticFilePath")]
        public string StaticFilePath { get; set; }

        public class GoogleProductModel : BaseNopModel
        {
            public int ProductVariantId { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.ProductName")]
            public string FullProductVariantName { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.GoogleCategory")]
            public string GoogleCategory { get; set; }
        }
    }
}