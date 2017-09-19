using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Core.Plugins;
using Nop.Plugin.Feed.GoogleShopping.Domain;
using Nop.Plugin.Feed.GoogleShopping.Models;
using Nop.Plugin.Feed.GoogleShopping.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Feed.GoogleShopping.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class FeedGoogleShoppingController : BasePluginController
    {
        private readonly IGoogleService _googleService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IStoreService _storeService;
        private readonly GoogleShoppingSettings _googleShoppingSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FeedGoogleShoppingController(IGoogleService googleService,
            IProductService productService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IPluginFinder pluginFinder,
            ILogger logger,
            IWebHelper webHelper,
            IStoreService storeService,
            GoogleShoppingSettings googleShoppingSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            IHostingEnvironment hostingEnvironment)
        {
            this._googleService = googleService;
            this._productService = productService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._webHelper = webHelper;
            this._storeService = storeService;
            this._googleShoppingSettings = googleShoppingSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new FeedGoogleShoppingModel();
            model.ProductPictureSize = _googleShoppingSettings.ProductPictureSize;
            model.PassShippingInfoWeight = _googleShoppingSettings.PassShippingInfoWeight;
            model.PassShippingInfoDimensions = _googleShoppingSettings.PassShippingInfoDimensions;
            model.PricesConsiderPromotions = _googleShoppingSettings.PricesConsiderPromotions;
            //stores
            model.StoreId = _googleShoppingSettings.StoreId;
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            //currencies
            model.CurrencyId = _googleShoppingSettings.CurrencyId;
            foreach (var c in _currencyService.GetAllCurrencies())
                model.AvailableCurrencies.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //Google categories
            model.DefaultGoogleCategory = _googleShoppingSettings.DefaultGoogleCategory;
            model.AvailableGoogleCategories.Add(new SelectListItem {Text = "Select a category", Value = ""});
            foreach (var gc in _googleService.GetTaxonomyList())
                model.AvailableGoogleCategories.Add(new SelectListItem { Text = gc, Value = gc });

            //file paths
            foreach (var store in _storeService.GetAllStores())
            {
                var localFilePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "files\\exportimport", store.Id + "-" + _googleShoppingSettings.StaticFileName);
                if (System.IO.File.Exists(localFilePath))
                    model.GeneratedFiles.Add(new FeedGoogleShoppingModel.GeneratedFileModel
                    {
                        StoreName = store.Name,
                        FileUrl = $"{_webHelper.GetStoreLocation(false)}files/exportimport/{store.Id}-{_googleShoppingSettings.StaticFileName}"
                    });
            }

            return View("~/Plugins/Feed.GoogleShopping/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public IActionResult Configure(FeedGoogleShoppingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _googleShoppingSettings.ProductPictureSize = model.ProductPictureSize;
            _googleShoppingSettings.PassShippingInfoWeight = model.PassShippingInfoWeight;
            _googleShoppingSettings.PassShippingInfoDimensions = model.PassShippingInfoDimensions;
            _googleShoppingSettings.PricesConsiderPromotions = model.PricesConsiderPromotions;
            _googleShoppingSettings.CurrencyId = model.CurrencyId;
            _googleShoppingSettings.StoreId = model.StoreId;
            _googleShoppingSettings.DefaultGoogleCategory = model.DefaultGoogleCategory;
            _settingService.SaveSetting(_googleShoppingSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            //redisplay the form
            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("generate")]
        public IActionResult GenerateFeed(FeedGoogleShoppingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Froogle");
                if (pluginDescriptor == null)
                    throw new Exception("Cannot load the plugin");

                //plugin
                var plugin = pluginDescriptor.Instance() as GoogleShoppingService;
                if (plugin == null)
                    throw new Exception("Cannot load the plugin");

                var stores = new List<Store>();
                var storeById = _storeService.GetStoreById(_googleShoppingSettings.StoreId);
                if (storeById != null)
                    stores.Add(storeById);
                else
                    stores.AddRange(_storeService.GetAllStores());

                foreach (var store in stores)
                    plugin.GenerateStaticFile(store);

                SuccessNotification(_localizationService.GetResource("Plugins.Feed.GoogleShopping.SuccessResult"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                _logger.Error(exc.Message, exc);
            }

            return Configure();
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult GoogleProductList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var products = _productService.SearchProducts(pageIndex: command.Page - 1,
                pageSize: command.PageSize, showHidden: true);
            var productsModel = products
                .Select(x =>
                            {
                                var gModel = new FeedGoogleShoppingModel.GoogleProductModel
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
                                    gModel.CustomGoods = googleProduct.CustomGoods;
                                }

                                return gModel;
                            })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productsModel,
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult GoogleProductUpdate(FeedGoogleShoppingModel.GoogleProductModel model)
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
                googleProduct.CustomGoods = model.CustomGoods;
                _googleService.UpdateGoogleProductRecord(googleProduct);
            }
            else
            {
                //insert
                googleProduct = new GoogleProductRecord
                {
                    ProductId = model.ProductId,
                    Taxonomy = model.GoogleCategory,
                    Gender = model.Gender,
                    AgeGroup = model.AgeGroup,
                    Color = model.Color,
                    Size = model.GoogleSize,
                    CustomGoods = model.CustomGoods
                };
                _googleService.InsertGoogleProductRecord(googleProduct);
            }

            return new NullJsonResult();
        }
    }
}