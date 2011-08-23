using System;
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
using Nop.Plugin.Feed.Froogle.Data;
using Nop.Plugin.Feed.Froogle.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.PromotionFeed;
using Nop.Services.Seo;
using Nop.Core.Domain.Catalog;
using System.Web;
using System.Collections.Generic;

namespace Nop.Plugin.Feed.Froogle
{
    public class FroogleService : BasePlugin,  IPromotionFeed
    {
        #region Fields

        private readonly IGoogleService _googleService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly FroogleSettings _froogleSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly GoogleProductObjectContext _objectContext;

        #endregion

        #region Ctor
        public FroogleService(IGoogleService googleService,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            ICurrencyService currencyService,
            IWebHelper webHelper, ISettingService settingService,
            StoreInformationSettings storeInformationSettings,
            FroogleSettings froogleSettings,
            CurrencySettings currencySettings,
            GoogleProductObjectContext objectContext)
        {
            this._googleService = googleService;
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._storeInformationSettings = storeInformationSettings;
            this._froogleSettings = froogleSettings;
            this._currencySettings = currencySettings;
            this._objectContext = objectContext;
        }

        #endregion

        #region Utilities

        private Currency GetUsedCurrency()
        {
            var currency = _currencyService.GetCurrencyById(_froogleSettings.CurrencyId);
            if (currency == null || !currency.Published)
                currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            return currency;
        }

        private IList<Category> GetCategoryBreadCrumb(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var breadCrumb = new List<Category>();

            while (category != null && //category is not null
                !category.Deleted && //category is not deleted
                category.Published) //category is published
            {
                breadCrumb.Add(category);
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            breadCrumb.Reverse();
            return breadCrumb;
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
                //Generate feed according to the following specs: http://www.google.com/support/merchants/bin/answer.py?answer=188494&expand=GB
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
                        writer.WriteStartElement("item");

                        #region Basic Product Information

                        //id [id]- An identifier of the item
                        writer.WriteElementString("g", "id", googleBaseNamespace, productVariant.Id.ToString());

                        //title [title] - Title of the item
                        writer.WriteStartElement("title");
                        var title = productVariant.FullProductName;
                        //title should be not longer than 70 characters
                        if (title.Length > 70)
                            title = title.Substring(0, 70);
                        writer.WriteCData(productVariant.FullProductName);
                        writer.WriteEndElement(); // title

                        //description [description] - Description of the item
                        writer.WriteStartElement("description");
                        string description = productVariant.Description;
                        if (String.IsNullOrEmpty(description))
                            description = product.FullDescription;
                        if (String.IsNullOrEmpty(description))
                            description = product.ShortDescription;
                        if (String.IsNullOrEmpty(description))
                            description = product.Name;
                        if (String.IsNullOrEmpty(description))
                            description = productVariant.FullProductName; //description is required
                        writer.WriteCData(description);
                        writer.WriteEndElement(); // description



                        //google product category [google_product_category] - Google's category of the item
                        //the category of the product according to Google’s product taxonomy. http://www.google.com/support/merchants/bin/answer.py?answer=160081
                        string googleProductCategory = "";
                        var googleProduct = _googleService.GetByProductVariantId(productVariant.Id);
                        if (googleProduct != null)
                            googleProductCategory = googleProduct.Taxonomy;
                        if (String.IsNullOrEmpty(googleProductCategory))
                            googleProductCategory = _froogleSettings.DefaultGoogleCategory;
                        if (String.IsNullOrEmpty(googleProductCategory))
                            throw new NopException("Default Google category is not set");
                        writer.WriteElementString("g", "google_product_category", googleBaseNamespace,
                                                  HttpUtility.HtmlEncode(googleProductCategory));


                        //product type [product_type] - Your category of the item
                        var defaultProductCategory = _categoryService.GetProductCategoriesByProductId(product.Id).FirstOrDefault();
                        if (defaultProductCategory != null)
                        {
                            var categoryBreadCrumb = GetCategoryBreadCrumb(defaultProductCategory.Category);
                            string yourProductCategory = "";
                            for (int i = 0; i < categoryBreadCrumb.Count; i++)
                            {
                                var cat = categoryBreadCrumb[i];
                                yourProductCategory = yourProductCategory + cat.Name;
                                if (i != categoryBreadCrumb.Count - 1)
                                    yourProductCategory = yourProductCategory + " > ";
                            }
                            if (!String.IsNullOrEmpty((yourProductCategory)))
                                writer.WriteElementString("g", "product_type", googleBaseNamespace,
                                                          HttpUtility.HtmlEncode(yourProductCategory));
                        }

                        //link [link] - URL directly linking to your item's page on your website
                        var productUrl = string.Format("{0}p/{1}/{2}", _webHelper.GetStoreLocation(false), product.Id,
                                                       product.GetSeName());
                        writer.WriteElementString("link", productUrl);

                        //image link [image_link] - URL of an image of the item
                        string imageUrl;
                        var pictures = _pictureService.GetPicturesByProductId(product.Id, 1);
                        if (pictures.Count > 0)
                            imageUrl = _pictureService.GetPictureUrl(pictures[0], _froogleSettings.ProductPictureSize,
                                                                     true);
                        else
                            imageUrl = _pictureService.GetDefaultPictureUrl(_froogleSettings.ProductPictureSize,
                                                                            PictureType.Entity);

                        writer.WriteElementString("g", "image_link", googleBaseNamespace, imageUrl);

                        //condition [condition] - Condition or state of the item
                        writer.WriteElementString("g", "condition", googleBaseNamespace, "new");

                        #endregion

                        #region Availability & Price

                        //availability [availability] - Availability status of the item
                        string availability = "in stock"; //in stock by default
                        if (productVariant.ManageInventoryMethod == ManageInventoryMethod.ManageStock
                            && productVariant.StockQuantity <= 0)
                        {
                            switch (productVariant.BackorderMode)
                            {
                                case BackorderMode.NoBackorders:
                                    {
                                        availability = "out of stock";
                                    }
                                    break;
                                case BackorderMode.AllowQtyBelow0:
                                case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
                                    {
                                        availability = "available for order";
                                        //availability = "preorder";
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        writer.WriteElementString("g", "availability", googleBaseNamespace, availability);

                        //price [price] - Price of the item
                        var currency = GetUsedCurrency();
                        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(productVariant.Price, currency);
                        writer.WriteElementString("g", "price", googleBaseNamespace,
                                                  price.ToString(new CultureInfo("en-US", false).NumberFormat) + " " +
                                                  currency.CurrencyCode);

                        #endregion

                        #region Unique Product Identifiers

                        /* Unique product identifiers such as UPC, EAN, JAN or ISBN allow us to show your listing on the appropriate product page. If you don't provide the required unique product identifiers, your store may not appear on product pages, and all your items may be removed from Product Search.
                         * We require unique product identifiers for all products - except for custom made goods. For apparel, you must submit the 'brand' attribute. For media (such as books, movies, music and video games), you must submit the 'gtin' attribute. In all cases, we recommend you submit all three attributes.
                         * You need to submit at least two attributes of 'brand', 'gtin' and 'mpn', but we recommend that you submit all three if available. For media (such as books, movies, music and video games), you must submit the 'gtin' attribute, but we recommend that you include 'brand' and 'mpn' if available.
                        */

                        //brand [brand] - Brand of the item
                        var defaultManufacturer =
                            _manufacturerService.GetProductManufacturersByProductId((product.Id)).FirstOrDefault();
                        if (defaultManufacturer != null)
                        {
                            writer.WriteStartElement("g", "brand", googleBaseNamespace);
                            writer.WriteCData(defaultManufacturer.Manufacturer.Name);
                            writer.WriteFullEndElement(); // g:brand
                        }


                        //mpn [mpn] - Manufacturer Part Number (MPN) of the item
                        writer.WriteStartElement("g", "mpn", googleBaseNamespace);
                        var mpn = productVariant.ManufacturerPartNumber;
                        //at least two are required for Unique product identifiers. So let's set it to a product variant name
                        //if (String.IsNullOrEmpty((mpn)))
                        //    mpn = productVariant.FullProductName;
                        writer.WriteCData(mpn);
                        writer.WriteFullEndElement(); // g:brand

                        #endregion
                        
                        #region Tax & Shipping
                        
                        //tax [tax]
                        //The tax attribute is an item-level override for merchant-level tax settings as defined in your Google Merchant Center account. This attribute is only accepted in the US, if your feed targets a country outside of the US, please do not use this attribute.
                        //IMPORTANT NOTE: Set tax in your Google Merchant Center account settings

                        //IMPORTANT NOTE: Set shipping in your Google Merchant Center account settings

                        //if (productVariant.Weight != decimal.Zero)
                        //{
                        //    writer.WriteElementString("g", "weight", googleBaseNamespace, string.Format(CultureInfo.InvariantCulture, "{0} {1}", productVariant.Weight.ToString(new CultureInfo("en-US", false).NumberFormat), IoC.Resolve<IMeasureService>().BaseWeightIn.SystemKeyword));
                        //}

                        #endregion
                        
                        writer.WriteElementString("g", "expiration_date", googleBaseNamespace, DateTime.Now.AddDays(28).ToString("yyyy-MM-dd"));
                        

                        writer.WriteEndElement(); // item
                    }
                }

                writer.WriteEndElement(); // channel
                writer.WriteEndElement(); // rss
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new FroogleSettings()
            {
                ProductPictureSize = 125,
                FtpHostname = "ftp://uploads.google.com"
            };
            _settingService.SaveSetting(settings);


            _objectContext.Install();

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _objectContext.Uninstall();
            base.Uninstall();
        }

        #endregion
    }
}
