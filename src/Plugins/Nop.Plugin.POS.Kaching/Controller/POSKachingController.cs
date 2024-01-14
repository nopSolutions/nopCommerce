using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching.Controller
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public class POSKachingController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly POSKachingSettings _kachingSettings;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IPOSKachingService _poskachingService;
        private readonly IManufacturerService _manufacturerService;

        public POSKachingController(ILogger logger, POSKachingSettings kachingSettings, IManufacturerService manufacturerService, ISettingService settingService, IPOSKachingService poskachingService, IProductService productService)
        {
            _logger = logger;
            _kachingSettings = kachingSettings;
            _settingService = settingService;
            _productService = productService;          
            _poskachingService = poskachingService;
            _manufacturerService = manufacturerService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> ConfigureAsync()
        {
            KachingConfigurationModel model = null;
            try
            {
                model = GetBaseModel();
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                await _logger.ErrorAsync("Configure POS Kaching: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return View("~/Plugins/POS.Kaching/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> ConfigureAsync(KachingConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await ConfigureAsync();

            try
            {
                _kachingSettings.POSKaChingActive = model.POSKaChingActive;
                _kachingSettings.POSKaChingHost = model.POSKaChingHost;
                _kachingSettings.POSKaChingId = model.POSKaChingId;
                _kachingSettings.POSKaChingAccountToken = model.POSKaChingAccountToken;
                _kachingSettings.POSKaChingAPIToken = model.POSKaChingAPIToken;
                _kachingSettings.POSKaChingImportQueueName = model.POSKaChingImportQueueName;
                _kachingSettings.POSKaChingReconciliationMailAddresses = model.POSKaChingReconciliationMailAddresses;
                _kachingSettings.POSKaChingReconciliationMailName = model.POSKaChingReconciliationMailName;
                _kachingSettings.ReconciliationInvoiceProductId = model.ReconciliationInvoiceProductId;

                await _settingService.SaveSettingAsync(_kachingSettings);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                await _logger.ErrorAsync("Configure POS Kaching: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return await ConfigureAsync();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> SendAllProducts()
        {
            var model = GetBaseModel();

            if (this._kachingSettings.POSKaChingActive)
            {
                IPagedList<Core.Domain.Catalog.Product> products = await _productService.SearchProductsAsync();               
                int count = 0;
                foreach (Core.Domain.Catalog.Product product in products)
                {
                    try
                    {
                        var json = await _poskachingService.BuildJSONStringAsync(product);
                        await _poskachingService.SaveProductAsync(json);                        
                        count++;
                    }
                    catch (Exception ex)
                    {
                        Exception inner = ex;
                        while (inner.InnerException != null)
                            inner = inner.InnerException;

                        string message = $"Error sending all products to Kaching, productid: {product.Id}{Environment.NewLine}Message: {inner.Message}";
                        await _logger.ErrorAsync(message, ex);
                        model.ErrorMessage += "<br />" + message;
                    }
                }

                if (count > 0)
                {
                    model.ProductsTransferred = "Products transferred: " + count;
                }
            }
            else
            {
                model.ErrorMessage = "Kaching is not active";
            }

            return View("~/Plugins/POS.Kaching/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> SendAllProductCategories()
        {
            var model = GetBaseModel();

            if (this._kachingSettings.POSKaChingActive)
            {
                IPagedList<Core.Domain.Catalog.Manufacturer> manufacturers = await _manufacturerService.GetAllManufacturersAsync();
                int count = 0;
                foreach (Core.Domain.Catalog.Manufacturer manufacturer in manufacturers)
                {
                    try
                    {
                        var json = _poskachingService.BuildJSONStringForCategory(manufacturer.Name);
                        await _poskachingService.SaveProductCategoryAsync(json);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        Exception inner = ex;
                        while (inner.InnerException != null)
                            inner = inner.InnerException;
                        await _logger.ErrorAsync("SendAllProductCategories to Kaching: " + inner.Message, ex);
                        model.ErrorMessage += "<br />" + inner.Message;
                    }
                }

                if (count > 0)
                {
                    model.ProductsTransferred = "ProductCategories transferred: " + count;
                }
            }
            else
            {
                model.ErrorMessage = "Kaching is not active";
            }

            return View("~/Plugins/POS.Kaching/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> SendProduct(int productId)
        {
            var model = GetBaseModel();

            if (this._kachingSettings.POSKaChingActive)
            {
                try
                {
                    Core.Domain.Catalog.Product product = await _productService.GetProductByIdAsync(productId);
                    if (product == null)
                    {
                        throw new ArgumentException($"Product not found with productid: {productId}");
                    }
                   
                    var json = await _poskachingService.BuildJSONStringAsync(product);
                    await _poskachingService.SaveProductAsync(json);
                }
                catch (Exception ex)
                {
                    Exception inner = ex;
                    while (inner.InnerException != null)
                        inner = inner.InnerException;

                    string message = $"Error sending 1 product to Kaching, productid: {productId}{Environment.NewLine}Message: {inner.Message}";
                    await _logger.ErrorAsync(message, ex);
                    model.ErrorMessage += "<br />" + message;
                }
            }
            else
            {
                model.ErrorMessage = "Kaching is not active";
            }

            return View("~/Plugins/POS.Kaching/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var model = GetBaseModel();

            if (_kachingSettings.POSKaChingActive)
            {
                try
                {
                    string[] ids = new string[1];
                    ids[0] = productId.ToString();
                    await _poskachingService.DeleteProductAsync(ids);
                }
                catch (Exception ex)
                {
                    Exception inner = ex;
                    while (inner.InnerException != null)
                        inner = inner.InnerException;
                    await _logger.ErrorAsync($"SendProduct to Kaching: {inner.Message}", ex);
                    model.ErrorMessage += "<br />" + inner.Message;
                }
            }
            else
            {
                model.ErrorMessage = "Kaching is not active";
            }

            return View("~/Plugins/POS.Kaching/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> TestConnection()
        {
            KachingConfigurationModel model = GetBaseModel();

            try
            {                
                if (await _poskachingService.TestConnection())
                {
                    model.KachingAliveValue = "Kaching is alive";
                }
                else
                {
                    model.KachingIsDead = "Kaching is dead";
                }
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                await _logger.ErrorAsync("Configure POS Kaching: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return View("~/Plugins/POS.Kaching/Views/Configure.cshtml", model);
        }

        private KachingConfigurationModel GetBaseModel()
        {
            return new KachingConfigurationModel
            {
                POSKaChingActive = _kachingSettings.POSKaChingActive,
                POSKaChingHost = _kachingSettings.POSKaChingHost,
                POSKaChingId = _kachingSettings.POSKaChingId,
                POSKaChingAccountToken = _kachingSettings.POSKaChingAccountToken,
                POSKaChingAPIToken = _kachingSettings.POSKaChingAPIToken,
                POSKaChingImportQueueName = _kachingSettings.POSKaChingImportQueueName,
                POSKaChingReconciliationMailAddresses = _kachingSettings.POSKaChingReconciliationMailAddresses,
                POSKaChingReconciliationMailName = _kachingSettings.POSKaChingReconciliationMailName,
                ReconciliationInvoiceProductId = _kachingSettings.ReconciliationInvoiceProductId
            };
        }
    }
}