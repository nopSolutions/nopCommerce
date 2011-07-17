using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Html;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.PromotionFeed;
using Nop.Services.Seo;

namespace Nop.Plugin.Feed.PriceGrabber
{
    public class PriceGrabberService : BasePlugin,  IPromotionFeed
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly PriceGrabberSettings _priceGrabberSettings;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor
        public PriceGrabberService(IProductService productService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService, IPictureService pictureService,
            ICurrencyService currencyService, IWebHelper webHelper,
            ISettingService settingService,
            PriceGrabberSettings priceGrabberSettings, CurrencySettings currencySettings)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._priceGrabberSettings = priceGrabberSettings;
            this._currencySettings = currencySettings;
        }

        #endregion

        #region Utilities

        private Nop.Core.Domain.Directory.Currency GetUsedCurrency()
        {
            var currency = _currencyService.GetCurrencyById(_priceGrabberSettings.CurrencyId);
            if (currency == null || !currency.Published)
                currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            return currency;
        }

        private static string RemoveSpecChars(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            s = s.Replace(';', ',');
            s = s.Replace('\r', ' ');
            s = s.Replace('\n', ' ');
            return s;
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
            controllerName = "FeedPriceGrabber";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Feed.PriceGrabber.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Generate a feed
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Generated feed</returns>
        public void GenerateFeed(Stream stream)
        {

            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("Unique Retailer SKU;Manufacturer Name;Manufacturer Part Number;Product Title;Categorization;Product URL;Image URL;Detailed Description;Selling Price;Condition;Availability");

                foreach (var p in _productService.GetAllProducts(false))
                {
                    foreach (var pv in _productService.GetProductVariantsByProductId(p.Id, false))
                    {
                        string sku = pv.Id.ToString();
                        var productManufacturers = _manufacturerService.GetProductManufacturersByProductId(p.Id, false);
                        string manufacturerName = productManufacturers.Count > 0 ? productManufacturers[0].Manufacturer.Name : String.Empty;
                        string manufacturerPartNumber = pv.ManufacturerPartNumber;
                        string productTitle = pv.FullProductName;
                        //TODO add a method for getting product URL (e.g. SEOHelper.GetProductUrl)
                        var productUrl = string.Format("{0}p/{1}/{2}", _webHelper.GetStoreLocation(false), p.Id, p.GetSeName());

                        string imageUrl = string.Empty;
                        var pictures = _pictureService.GetPicturesByProductId(p.Id, 1);
                        if (pictures.Count > 0)
                            imageUrl = _pictureService.GetPictureUrl(pictures[0], _priceGrabberSettings.ProductPictureSize , true);
                        else
                            imageUrl = _pictureService.GetDefaultPictureUrl(_priceGrabberSettings.ProductPictureSize, PictureType.Entity);

                        string description = pv.Description;
                        var currency = GetUsedCurrency();
                        string price = _currencyService.ConvertFromPrimaryStoreCurrency(pv.Price, currency).ToString(new CultureInfo("en-US", false).NumberFormat);
                        string availability = pv.StockQuantity > 0 ? "Yes" : "No";
                        string categorization = "no category";

                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.FullDescription;
                        }
                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.ShortDescription;
                        }
                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.Name;
                        }

                        var productCategories = _categoryService.GetProductCategoriesByProductId(p.Id);
                        if (productCategories.Count > 0)
                        {
                            var firstCategory = productCategories[0].Category;
                            if (firstCategory != null)
                            {
                                var sb = new StringBuilder();
                                foreach (var cat in GetCategoryBreadCrumb(firstCategory))
                                {
                                    sb.AppendFormat("{0}>", cat.Name);
                                }
                                sb.Length -= 1;
                                categorization = sb.ToString();
                            }
                        }

                        productTitle = RemoveSpecChars(productTitle);
                        manufacturerPartNumber = RemoveSpecChars(manufacturerPartNumber);
                        manufacturerName = RemoveSpecChars(manufacturerName);
                        description = HtmlHelper.StripTags(description);
                        description = RemoveSpecChars(description);
                        categorization = RemoveSpecChars(categorization);

                        writer.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};New;{9}",
                            sku,
                            manufacturerName,
                            manufacturerPartNumber,
                            productTitle,
                            categorization,
                            productUrl,
                            imageUrl,
                            description,
                            price,
                            availability);
                    }
                }
            }
        }


        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new PriceGrabberSettings()
            {
                ProductPictureSize = 125,
            };
            _settingService.SaveSetting(settings);

            base.Install();
        }
        #endregion
    }
}
