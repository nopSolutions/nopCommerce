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
            AvailableStores = new List<SelectListItem>();
            AvailableCurrencies = new List<SelectListItem>();
            AvailableGoogleCategories = new List<SelectListItem>();
            GeneratedFiles = new List<GeneratedFileModel>();
        }

        [NopResourceDisplayName("Plugins.Feed.Froogle.ProductPictureSize")]
        public int ProductPictureSize { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.Store")]
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.Currency")]
        public int CurrencyId { get; set; }
        public IList<SelectListItem> AvailableCurrencies { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.DefaultGoogleCategory")]
        public string DefaultGoogleCategory { get; set; }
        public IList<SelectListItem> AvailableGoogleCategories { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.PassShippingInfoWeight")]
        public bool PassShippingInfoWeight { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.PassShippingInfoDimensions")]
        public bool PassShippingInfoDimensions { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.PricesConsiderPromotions")]
        public bool PricesConsiderPromotions { get; set; }

        [NopResourceDisplayName("Plugins.Feed.Froogle.StaticFilePath")]
        public IList<GeneratedFileModel> GeneratedFiles { get; set; }
        
        public class GeneratedFileModel : BaseNopModel
        {
            public string StoreName { get; set; }
            public string FileUrl { get; set; }
        }

        public class GoogleProductModel : BaseNopModel
        {
            public int ProductId { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.ProductName")]
            public string ProductName { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.GoogleCategory")]
            public string GoogleCategory { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.Gender")]
            public string Gender { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.AgeGroup")]
            public string AgeGroup { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.Color")]
            public string Color { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.Size")]
            public string GoogleSize { get; set; }

            [NopResourceDisplayName("Plugins.Feed.Froogle.Products.CustomGoods")]
            public bool CustomGoods { get; set; }
        }
    }
}