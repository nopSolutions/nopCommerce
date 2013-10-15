using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Core.Plugins;
using Nop.Plugin.Feed.Froogle.Domain;
using Nop.Plugin.Feed.Froogle.Models;
using Nop.Plugin.Feed.Froogle.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Plugin.Feed.Froogle.Controllers
{
    [AdminAuthorize]
    public class FeedFroogleController : Controller
    {
        private readonly IGoogleService _googleService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IStoreService _storeService;
        private readonly FroogleSettings _froogleSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public FeedFroogleController(IGoogleService googleService, 
            IProductService productService, ICurrencyService currencyService,
            ILocalizationService localizationService, IPluginFinder pluginFinder, 
            ILogger logger, IWebHelper webHelper, IStoreService storeService,
            FroogleSettings froogleSettings, ISettingService settingService, 
            IPermissionService permissionService)
        {
            this._googleService = googleService;
            this._productService = productService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._webHelper = webHelper;
            this._storeService = storeService;
            this._froogleSettings = froogleSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new FeedFroogleModel();
            //picture
            model.ProductPictureSize = _froogleSettings.ProductPictureSize;
            //stores
            model.StoreId = _froogleSettings.StoreId;
            model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            //currencies
            model.CurrencyId = _froogleSettings.CurrencyId;
            foreach (var c in _currencyService.GetAllCurrencies())
                model.AvailableCurrencies.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            //Google categories
            model.DefaultGoogleCategory = _froogleSettings.DefaultGoogleCategory;
            model.AvailableGoogleCategories.Add(new SelectListItem() {Text = "Select a category", Value = ""});
            foreach (var gc in _googleService.GetTaxonomyList())
                model.AvailableGoogleCategories.Add(new SelectListItem() {Text = gc, Value = gc});

            //file paths
            foreach (var store in _storeService.GetAllStores())
            {
                var localFilePath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport", store.Id + "-" + _froogleSettings.StaticFileName);
                if (System.IO.File.Exists(localFilePath))
                    model.GeneratedFiles.Add(new FeedFroogleModel.GeneratedFileModel()
                    {
                        StoreName = store.Name,
                        FileUrl = string.Format("{0}content/files/exportimport/{1}-{2}", _webHelper.GetStoreLocation(false), store.Id, _froogleSettings.StaticFileName)
                    });
            }
            
            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }

        [HttpPost]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(FeedFroogleModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _froogleSettings.ProductPictureSize = model.ProductPictureSize;
            _froogleSettings.CurrencyId = model.CurrencyId;
            _froogleSettings.StoreId = model.StoreId;
            _froogleSettings.DefaultGoogleCategory = model.DefaultGoogleCategory;
            _settingService.SaveSetting(_froogleSettings);
            
            //redisplay the form
            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [ChildActionOnly]
        [FormValueRequired("generate")]
        public ActionResult GenerateFeed(FeedFroogleModel model)
        {
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Froogle");
                if (pluginDescriptor == null)
                    throw new Exception("Cannot load the plugin");

                //plugin
                var plugin = pluginDescriptor.Instance() as FroogleService;
                if (plugin == null)
                    throw new Exception("Cannot load the plugin");

                var stores = new List<Store>();
                var storeById = _storeService.GetStoreById(_froogleSettings.StoreId);
                if (storeById != null)
                    stores.Add(storeById);
                else
                    stores.AddRange(_storeService.GetAllStores());

                foreach (var store in stores)
                    plugin.GenerateStaticFile(store);

                model.GenerateFeedResult = _localizationService.GetResource("Plugins.Feed.Froogle.SuccessResult");
            }
            catch (Exception exc)
            {
                model.GenerateFeedResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }

            //stores
            model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
            //currencies
            foreach (var c in _currencyService.GetAllCurrencies())
                model.AvailableCurrencies.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            //Google categories
            model.AvailableGoogleCategories.Add(new SelectListItem() { Text = "Select a category", Value = "" });
            foreach (var gc in _googleService.GetTaxonomyList())
                model.AvailableGoogleCategories.Add(new SelectListItem() { Text = gc, Value = gc });

            //file paths
            foreach (var store in _storeService.GetAllStores())
            {
                var localFilePath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport", store.Id + "-" + _froogleSettings.StaticFileName);
                if (System.IO.File.Exists(localFilePath))
                    model.GeneratedFiles.Add(new FeedFroogleModel.GeneratedFileModel()
                    {
                        StoreName = store.Name,
                        FileUrl = string.Format("{0}content/files/exportimport/{1}-{2}", _webHelper.GetStoreLocation(false), store.Id, _froogleSettings.StaticFileName)
                    });
            }

            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GoogleProductList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var products = _productService.SearchProducts(pageIndex: command.Page - 1,
                pageSize: command.PageSize, showHidden: true);
            var productsModel = products
                .Select(x =>
                            {
                                var gModel = new FeedFroogleModel.GoogleProductModel()
                                {
                                    ProductId = x.Id,
                                    ProductName = x.Name

                                };
                                var googleProduct = _googleService.GetByProductId(x.Id);
                                if (googleProduct != null)
                                {
                                    gModel.GoogleCategory = googleProduct.Taxonomy;
                                    gModel.Gender = googleProduct.Gender;
                                    gModel.AgeGroup = googleProduct.AgeGroup;
                                    gModel.Color = googleProduct.Color;
                                    gModel.GoogleSize = googleProduct.Size;
                                }

                                return gModel;
                            })
                .ToList();

            var model = new GridModel<FeedFroogleModel.GoogleProductModel>
            {
                Data = productsModel,
                Total = products.TotalCount
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GoogleProductUpdate(GridCommand command, FeedFroogleModel.GoogleProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var googleProduct = _googleService.GetByProductId(model.ProductId);
            if (googleProduct != null)
            {

                googleProduct.Taxonomy = model.GoogleCategory;
                googleProduct.Gender = model.Gender;
                googleProduct.AgeGroup = model.AgeGroup;
                googleProduct.Color = model.Color;
                googleProduct.Size = model.GoogleSize;
                _googleService.UpdateGoogleProductRecord(googleProduct);
            }
            else
            {
                //insert
                googleProduct = new GoogleProductRecord()
                {
                    ProductId = model.ProductId,
                    Taxonomy = model.GoogleCategory,
                    Gender = model.Gender,
                    AgeGroup = model.AgeGroup,
                    Color = model.Color,
                    Size = model.GoogleSize
                };
                _googleService.InsertGoogleProductRecord(googleProduct);
            }
            
            return GoogleProductList(command);
        }
    }
}
