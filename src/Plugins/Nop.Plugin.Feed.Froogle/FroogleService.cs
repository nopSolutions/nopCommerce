using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Feed.Froogle.Data;
using Nop.Plugin.Feed.Froogle.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Tasks;

namespace Nop.Plugin.Feed.Froogle
{
    public class FroogleService : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IGoogleService _googleService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly FroogleSettings _froogleSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly GoogleProductObjectContext _objectContext;

        #endregion

        #region Ctor
        public FroogleService(IScheduleTaskService scheduleTaskService,
            IGoogleService googleService,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            ICurrencyService currencyService,
            IWebHelper webHelper,
            ISettingService settingService,
            IWorkContext workContext,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            StoreInformationSettings storeInformationSettings,
            FroogleSettings froogleSettings,
            CurrencySettings currencySettings,
            GoogleProductObjectContext objectContext)
        {
            this._scheduleTaskService = scheduleTaskService;
            this._googleService = googleService;
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._workContext = workContext;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
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

        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Feed.Froogle.StaticFileGenerationTask, Nop.Plugin.Feed.Froogle");
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
                        writer.WriteCData(title);
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
                                                       product.GetSeName(_workContext.WorkingLanguage.Id));
                        writer.WriteElementString("link", productUrl);

                        //image link [image_link] - URL of an image of the item
                        string imageUrl;
                        var picture = _pictureService.GetPictureById(productVariant.PictureId);
                        if (picture == null)
                            picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();

                        //always use HTTP when getting image URL
                        if (picture != null)
                            imageUrl = _pictureService.GetPictureUrl(picture, _froogleSettings.ProductPictureSize, useSsl: false);
                        else
                            imageUrl = _pictureService.GetDefaultPictureUrl(_froogleSettings.ProductPictureSize, useSsl: false);

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

                        //GTIN [gtin] - GTIN
                        var gtin = productVariant.Gtin;
                        if (!String.IsNullOrEmpty(gtin))
                        {
                            writer.WriteStartElement("g", "gtin", googleBaseNamespace);
                            writer.WriteCData(gtin);
                            writer.WriteFullEndElement(); // g:gtin
                        }

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
                        var mpn = productVariant.ManufacturerPartNumber;
                        if (!String.IsNullOrEmpty(mpn))
                        {
                            writer.WriteStartElement("g", "mpn", googleBaseNamespace);
                            writer.WriteCData(mpn);
                            writer.WriteFullEndElement(); // g:mpn
                        }

                        #endregion
                        
                        #region Tax & Shipping
                        
                        //tax [tax]
                        //The tax attribute is an item-level override for merchant-level tax settings as defined in your Google Merchant Center account. This attribute is only accepted in the US, if your feed targets a country outside of the US, please do not use this attribute.
                        //IMPORTANT NOTE: Set tax in your Google Merchant Center account settings

                        //IMPORTANT NOTE: Set shipping in your Google Merchant Center account settings
                        
                        //shipping weight [shipping_weight] - Weight of the item for shipping
                        //We accept only the following units of weight: lb, oz, g, kg.
                        if (_froogleSettings.PassShippingInfo)
                        {
                            var weightName = "kg";
                            var shippingWeight = productVariant.Weight;
                            switch (_measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).SystemKeyword)
                            {
                                case "ounce":
                                    weightName = "oz";
                                    break;
                                case "lb":
                                    weightName = "lb";
                                    break;
                                case "grams":
                                    weightName = "g";
                                    break;
                                case "kg":
                                    weightName = "kg";
                                    break;
                                default:
                                    //unknown weight 
                                    weightName = "kg";
                                    break;
                            }
                            writer.WriteElementString("g", "shipping_weight", googleBaseNamespace, string.Format(CultureInfo.InvariantCulture, "{0} {1}", shippingWeight.ToString(new CultureInfo("en-US", false).NumberFormat), weightName));
                        }

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
            //settings
            var settings = new FroogleSettings()
            {
                ProductPictureSize = 125,
                PassShippingInfo = false,
                FtpHostname = "ftp://uploads.google.com",
                StaticFileName = string.Format("froogle_{0}.xml", CommonHelper.GenerateRandomDigitCode(10)),
            };
            _settingService.SaveSetting(settings);
            
            //data
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.ClickHere", "Click here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Currency", "Currency");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Currency.Hint", "Select the default currency that will be used to generate the feed.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.DefaultGoogleCategory", "Default Google category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.DefaultGoogleCategory.Hint", "The default Google category will be useds if other one is not specified.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpHostname", "FTP Hostname");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpHostname.Hint", "Google FTP server hostname.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpFilename", "FTP File name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpFilename.Hint", "Feed file name.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpUsername", "FTP Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpUsername.Hint", "Google FTP account username.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpPassword", "FTP Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpPassword.Hint", "Google FTP account password.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.FtpUploadStatus", "Froogle feed upload status: {0}");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.General", "General");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Generate", "Generate feed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Override", "Override product settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.ProductPictureSize", "Product thumbnail image size");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.ProductPictureSize.Hint", "The default size (pixels) for product thumbnail images.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Products.ProductName", "Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Products.GoogleCategory", "Google Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.SuccessResult", "Froogle feed has been successfully generated. {0} to see generated feed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.Upload", "Upload feed to Google FTP server");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.TaskEnabled", "Automatically generate a file");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.TaskEnabled.Hint", "Check if you want a file to be automatically generated.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.GenerateStaticFileEachMinutes", "A task period (minutes)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.GenerateStaticFileEachMinutes.Hint", "Specify a task period in minutes (generation of a new Froogle file).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.TaskRestart", "If a task settings ('Automatically generate a file') have been changed, please restart the application");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.StaticFilePath", "Generated file path (static)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Feed.Froogle.StaticFilePath.Hint", "A file path of the generated Froogle file. It's static for your store and can be shared with the Froogle service.");

            //install a schedule task
            var task = FindScheduledTask();
            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Froogle static file generation",
                    //each 60 minutes
                    Seconds = 3600,
                    Type = "Nop.Plugin.Feed.Froogle.StaticFileGenerationTask, Nop.Plugin.Feed.Froogle",
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FroogleSettings>();

            //data
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.ClickHere");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Currency");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Currency.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.DefaultGoogleCategory");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.DefaultGoogleCategory.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpHostname");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpHostname.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpFilename");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpFilename.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpUsername");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpUsername.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpPassword");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpPassword.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.FtpUploadStatus");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.General");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Generate");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Override");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.ProductPictureSize");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.ProductPictureSize.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Products.ProductName");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Products.GoogleCategory");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.SuccessResult");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.Upload");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.TaskEnabled");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.TaskEnabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.GenerateStaticFileEachMinutes");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.GenerateStaticFileEachMinutes.Hint");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.TaskRestart");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.StaticFilePath");
            this.DeletePluginLocaleResource("Plugins.Feed.Froogle.StaticFilePath.Hint");


            //Remove scheduled task
            var task = FindScheduledTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            base.Uninstall();
        }
        
        /// <summary>
        /// Generate a static file for froogle
        /// </summary>
        public virtual void GenerateStaticFile()
        {
            string filePath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport", _froogleSettings.StaticFileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                GenerateFeed(fs);
            }
        }

        #endregion
    }
}
