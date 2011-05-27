using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.PromotionFeed;
using Nop.Web.Framework;

namespace Nop.Plugin.Feed.Froogle
{
    public class FroogleService : BasePlugin,  IPromotionFeed
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly FroogleSettings _froogleSettings;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor
        public FroogleService(IProductService productService,
            IPictureService pictureService,
            ICurrencyService currencyService,
            IWebHelper webHelper,
            StoreInformationSettings storeInformationSettings,
            FroogleSettings froogleSettings,
            CurrencySettings currencySettings)
        {
            this._productService = productService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._storeInformationSettings = storeInformationSettings;
            this._froogleSettings = froogleSettings;
            this._currencySettings = currencySettings;
        }

        #endregion

        #region Utilities

        private Nop.Core.Domain.Directory.Currency GetUsedCurrency()
        {
            var currency = _currencyService.GetCurrencyById(_froogleSettings.CurrencyId);
            if (currency == null || !currency.Published)
                currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            return currency;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "FeedFroogle";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Feed.Froogle.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Generate a feed
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Generated feed</returns>
        public void GenerateFeed(Stream stream)
        {
            const string googleBaseNamespace = "http://base.google.com/ns/1.0";

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("rss");
                writer.WriteAttributeString("version", "2.0");
                writer.WriteAttributeString("xmlns", "g", null, googleBaseNamespace);
                writer.WriteStartElement("channel");
                writer.WriteElementString("title", string.Format("{0} Google Base", _storeInformationSettings.StoreName));
                writer.WriteElementString("link", "http://base.google.com/base/");
                writer.WriteElementString("description", "Information about products");


                var products = _productService.GetAllProducts(false);
                foreach (var product in products)
                {
                    var productVariants = _productService.GetProductVariantsByProductId(product.Id, false);

                    foreach (var productVariant in productVariants)
                    {
                        //TODO add a method for getting product URL (e.g. SEOHelper.GetProductUrl)
                        var productUrl = string.Format("{0}p/{1}/{2}", _webHelper.GetStoreLocation(false), product.Id, product.GetSeName());
                        writer.WriteStartElement("item");
                        writer.WriteElementString("link", productUrl);
                        writer.WriteElementString("title", productVariant.FullProductName);
                        writer.WriteStartElement("description");
                        string description = productVariant.Description;
                        if (String.IsNullOrEmpty(description))
                            description = product.FullDescription;
                        if (String.IsNullOrEmpty(description))
                            description = product.ShortDescription;
                        if (String.IsNullOrEmpty(description))
                            description = product.Name;
                        writer.WriteCData(description);
                        writer.WriteEndElement(); // description
                        writer.WriteStartElement("g", "brand", googleBaseNamespace);
                        writer.WriteFullEndElement(); // g:brand
                        writer.WriteElementString("g", "condition", googleBaseNamespace, "new");
                        writer.WriteElementString("g", "expiration_date", googleBaseNamespace, DateTime.Now.AddDays(28).ToString("yyyy-MM-dd"));
                        writer.WriteElementString("g", "id", googleBaseNamespace, productVariant.Id.ToString());
                        string imageUrl = string.Empty;
                        var pictures = _pictureService.GetPicturesByProductId(product.Id, 1);
                        if (pictures.Count > 0)
                            imageUrl = _pictureService.GetPictureUrl(pictures[0], _froogleSettings.ProductPictureSize, true);
                        writer.WriteElementString("g", "image_link", googleBaseNamespace, imageUrl);
                        var currency = GetUsedCurrency();
                        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(productVariant.Price, currency);
                        //UNDONE should we round product prices?
                        writer.WriteElementString("g", "price", googleBaseNamespace, price.ToString(new CultureInfo("en-US", false).NumberFormat));

                        //uncomment and set your product_type attribute
                        //writer.WriteStartElement("g", "product_type", googleBaseNamespace);
                        //writer.WriteCData("Clothing & Accessories > Clothing Accessories > Hair Accessories > Hair Pins & Clips");
                        //writer.WriteFullEndElement(); // g:brand


                        //if (productVariant.Weight != decimal.Zero)
                        //{
                        //    writer.WriteElementString("g", "weight", googleBaseNamespace, string.Format(CultureInfo.InvariantCulture, "{0} {1}", productVariant.Weight.ToString(new CultureInfo("en-US", false).NumberFormat), IoC.Resolve<IMeasureService>().BaseWeightIn.SystemKeyword));
                        //}
                        writer.WriteEndElement(); // item
                    }
                }

                writer.WriteEndElement(); // channel
                writer.WriteEndElement(); // rss
                writer.WriteEndDocument();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get { return "Froogle"; }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public override string SystemName
        {
            get { return "PromotionFeed.Froogle"; }
        }

        /// <summary>
        /// Gets the author
        /// </summary>
        public override string Author
        {
            get { return "nopCommerce team"; }
        }

        /// <summary>
        /// Gets the version
        /// </summary>
        public override string Version
        {
            get { return "1.00"; }
        }
        
        #endregion
    }
}
