using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.DropShipping.AliExpress.Models;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Plugin.DropShipping.AliExpress.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Text.Json;
using Nop.Services.Plugins;

namespace Nop.Plugin.DropShipping.AliExpress.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class AliExpressController : BasePluginController
{
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IAliExpressService _aliExpressService;
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly IProductService _productService;
    private readonly IPluginService _pluginService;

    public AliExpressController(
        ISettingService settingService,
        IStoreContext storeContext,
        INotificationService notificationService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IAliExpressService aliExpressService,
        IAliExpressProductMappingService mappingService,
        IProductService productService,
        IPluginService pluginService)
    {
        _settingService = settingService;
        _storeContext = storeContext;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _aliExpressService = aliExpressService;
        _mappingService = mappingService;
        _productService = productService;
        _pluginService = pluginService;
    }

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
            return AccessDeniedView();

        var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
        var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(storeId);
        Dictionary<string, string> customProperties = new Dictionary<string, string>();

        var model = new ConfigurationModel
        {
            AppKey = settings.AppKey,
            AppSecret = settings.AppSecret,
            AccessToken = settings.AccessToken,
            RefreshToken = settings.RefreshToken,
            AccessTokenExpiresOnUtc = settings.AccessTokenExpiresOnUtc,
            RefreshTokenExpiresOnUtc = settings.RefreshTokenExpiresOnUtc,
            DefaultMarginPercentage = settings.DefaultMarginPercentage,
            VatPercentage = settings.VatPercentage,
            CustomsDutyPercentage = settings.CustomsDutyPercentage,
            DefaultShippingCountry = settings.DefaultShippingCountry,
            DefaultCurrency = settings.DefaultCurrency,
            EnableDailySync = settings.EnableDailySync,
            DailySyncHour = settings.DailySyncHour,
            AutoCreateOrders = settings.AutoCreateOrders,
            AutoCreateLocalShipments = settings.AutoCreateLocalShipments,
            TokenRefreshDaysBeforeExpiry = settings.TokenRefreshDaysBeforeExpiry,
            UseSandbox = settings.UseSandbox,
            AuthorizationUrl = settings.AuthorizationUrl,
            RedirectUri = settings.RedirectUri,
            RedirectUriHost = settings.RedirectUri,
            IsTokenValid = await _aliExpressService.IsTokenValid(),
            
        };

        if (!string.IsNullOrEmpty(settings.AppKey))
        {
            model.AuthorizationLaunchUrl = await _aliExpressService.GetAuthorizationUrlAsync();
        }

        return View("~/Plugins/DropShipping.AliExpress/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
        var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(storeId);

        settings.AppKey = model.AppKey;
        settings.AppSecret = model.AppSecret;
        settings.DefaultMarginPercentage = model.DefaultMarginPercentage;
        settings.VatPercentage = model.VatPercentage;
        settings.CustomsDutyPercentage = model.CustomsDutyPercentage;
        settings.DefaultShippingCountry = model.DefaultShippingCountry;
        settings.DefaultCurrency = model.DefaultCurrency;
        settings.EnableDailySync = model.EnableDailySync;
        settings.DailySyncHour = model.DailySyncHour;
        settings.AutoCreateOrders = model.AutoCreateOrders;
        settings.AutoCreateLocalShipments = model.AutoCreateLocalShipments;
        settings.TokenRefreshDaysBeforeExpiry = model.TokenRefreshDaysBeforeExpiry;
        settings.UseSandbox = model.UseSandbox;
        settings.AuthorizationUrl = model.AuthorizationUrl;
        settings.RedirectUri = model.RedirectUri;
        settings.RedirectUriHost = settings.RedirectUriHost;
        settings.AuthorizationLaunchUrl = await _aliExpressService.GetAuthorizationUrlAsync();

        await _settingService.SaveSettingAsync(settings, storeId);

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    public async Task<IActionResult> ExchangeAuthCode(string authCode)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
            return AccessDeniedView();

        if (string.IsNullOrWhiteSpace(authCode))
        {
            _notificationService.ErrorNotification("Authorization code is required");
            return await Configure();
        }

        var success = await _aliExpressService.ExchangeAuthorizationCodeAsync(authCode);

        if (success)
        {
            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Plugins.DropShipping.AliExpress.AuthSuccess"));
        }
        else
        {
            _notificationService.ErrorNotification(
                await _localizationService.GetResourceAsync("Plugins.DropShipping.AliExpress.AuthFailed"));
        }

        return await Configure();
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
            return AccessDeniedView();

        var success = await _aliExpressService.RefreshAccessTokenAsync();

        if (success)
        {
            _notificationService.SuccessNotification("Token refreshed successfully");
        }
        else
        {
            _notificationService.ErrorNotification("Failed to refresh token");
        }

        return await Configure();
    }

    [HttpGet]
    public async Task<IActionResult> SearchProducts(string keyword, int pageNo = 1)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE))
            return Json(new { success = false, message = "Access denied" });

        if (string.IsNullOrWhiteSpace(keyword))
            return Json(new { success = false, message = "Keyword is required" });

        var products = await _aliExpressService.SearchProductsAsync(keyword, pageNo, 20);

        return Json(new { success = true, data = products });
    }

    [HttpPost]
    public async Task<IActionResult> SaveProductMapping(int productId, long aliExpressProductId, string productData)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE))
            return Json(new { success = false, message = "Access denied" });

        try
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return Json(new { success = false, message = "Product not found" });

            var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(storeId);

            // Parse product data
            AliExpressProductSearchResultModel? productInfo = null;
            if (!string.IsNullOrEmpty(productData))
            {
                try
                {
                    productInfo = JsonSerializer.Deserialize<AliExpressProductSearchResultModel>(productData);
                }
                catch { }
            }

            var existingMapping = await _mappingService.GetMappingByProductIdAsync(productId);

            if (existingMapping == null)
            {
                // Create new mapping
                var mapping = new AliExpressProductMapping
                {
                    ProductId = productId,
                    AliExpressProductId = aliExpressProductId,
                    AliExpressProductUrl = productInfo?.ProductUrl,
                    AliExpressPrice = productInfo?.SalePrice ?? productInfo?.OriginalPrice ?? 0,
                    ShippingCost = 0, // Will be calculated separately
                    VatAmount = 0,
                    CustomsDuty = 0,
                    MarginPercentage = settings.DefaultMarginPercentage,
                    CalculatedPrice = 0,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastSyncOnUtc = DateTime.UtcNow,
                    IsAvailable = true,
                    ProductDetailsJson = productData
                };

                // Calculate price
                var baseAmount = mapping.AliExpressPrice + mapping.ShippingCost;
                mapping.CustomsDuty = baseAmount * (settings.CustomsDutyPercentage / 100);
                var subtotal = baseAmount + mapping.CustomsDuty;
                mapping.VatAmount = subtotal * (settings.VatPercentage / 100);
                var totalBeforeMargin = subtotal + mapping.VatAmount;
                mapping.CalculatedPrice = totalBeforeMargin * (1 + mapping.MarginPercentage / 100);

                await _mappingService.InsertMappingAsync(mapping);

                // Update product price
                product.Price = mapping.CalculatedPrice;
                await _productService.UpdateProductAsync(product);
            }
            else
            {
                // Update existing mapping
                existingMapping.AliExpressProductId = aliExpressProductId;
                existingMapping.AliExpressProductUrl = productInfo?.ProductUrl;
                existingMapping.AliExpressPrice = productInfo?.SalePrice ??
                                                  productInfo?.OriginalPrice ?? existingMapping.AliExpressPrice;
                existingMapping.LastSyncOnUtc = DateTime.UtcNow;
                existingMapping.ProductDetailsJson = productData;

                // Recalculate price
                var baseAmount = existingMapping.AliExpressPrice + existingMapping.ShippingCost;
                existingMapping.CustomsDuty = baseAmount * (settings.CustomsDutyPercentage / 100);
                var subtotal = baseAmount + existingMapping.CustomsDuty;
                existingMapping.VatAmount = subtotal * (settings.VatPercentage / 100);
                var totalBeforeMargin = subtotal + existingMapping.VatAmount;
                existingMapping.CalculatedPrice = totalBeforeMargin * (1 + existingMapping.MarginPercentage / 100);

                await _mappingService.UpdateMappingAsync(existingMapping);

                // Update product price
                product.Price = existingMapping.CalculatedPrice;
                await _productService.UpdateProductAsync(product);
            }

            return Json(new { success = true, message = "Product mapping saved successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProductMapping(int productId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE))
            return Json(new { success = false, message = "Access denied" });

        var mapping = await _mappingService.GetMappingByProductIdAsync(productId);

        if (mapping == null)
            return Json(new { success = false, message = "No mapping found" });

        return Json(new
        {
            success = true,
            data = new
            {
                aliExpressProductId = mapping.AliExpressProductId,
                productUrl = mapping.AliExpressProductUrl,
                price = mapping.AliExpressPrice,
                calculatedPrice = mapping.CalculatedPrice,
                lastSync = mapping.LastSyncOnUtc
            }
        });
    }
}