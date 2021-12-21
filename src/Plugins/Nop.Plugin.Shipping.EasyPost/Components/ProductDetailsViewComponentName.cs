using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.EasyPost.Domain.Shipment;
using Nop.Plugin.Shipping.EasyPost.Models.Product;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Shipping.EasyPost.Components
{
    /// <summary>
    /// Represents view component to render an additional block on the product details page in the admin area
    /// </summary>
    [ViewComponent(Name = EasyPostDefaults.PRODUCT_DETAILS_VIEW_COMPONENT_NAME)]
    public class ProductDetailsViewComponentName : NopViewComponent
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IShippingPluginManager _shippingPluginManager;

        #endregion

        #region Ctor

        public ProductDetailsViewComponentName(ICountryService countryService,
            IGenericAttributeService genericAttributeService,
            IPermissionService permissionService,
            IProductService productService,
            IShippingPluginManager shippingPluginManager)
        {
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
            _permissionService = permissionService;
            _productService = productService;
            _shippingPluginManager = shippingPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return Content(string.Empty);

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return Content(string.Empty);

            if (!widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
                return Content(string.Empty);

            if (additionalData is not ProductModel productModel || !productModel.IsShipEnabled)
                return Content(string.Empty);

            var product = await _productService.GetProductByIdAsync(productModel.Id);
            if (product is null)
                return Content(string.Empty);

            //try to get previously saved values
            var predefinedPackageValue = await _genericAttributeService
                .GetAttributeAsync<string>(product, EasyPostDefaults.ProductPredefinedPackageAttribute) ?? string.Empty;
            var carrier = predefinedPackageValue.Split('.').FirstOrDefault();
            var predefinedPackage = predefinedPackageValue.Split('.').LastOrDefault();
            var htsNumber = await _genericAttributeService.GetAttributeAsync<string>(product, EasyPostDefaults.ProductHtsNumberAttribute);
            var originCountry = await _genericAttributeService.GetAttributeAsync<string>(product, EasyPostDefaults.ProductOriginCountryAttribute);

            var availablePredefinedPackages = PredefinedPackage.PredefinedPackages.SelectMany(carrierPackages => carrierPackages.Value
                .Select(package => new SelectListItem($"{carrierPackages.Key} - {package}", $"{carrierPackages.Key}.{package}")))
                .ToList();
            availablePredefinedPackages.Insert(0, new SelectListItem("---", string.Empty));

            var availableCountries = (await _countryService.GetAllCountriesAsync(showHidden: true))
                .Select(country => new SelectListItem(country.Name, country.TwoLetterIsoCode))
                .ToList();
            availableCountries.Insert(0, new SelectListItem("---", string.Empty));

            var model = new ProductDetailsModel
            {
                EasyPostPredefinedPackage = !string.IsNullOrEmpty(predefinedPackageValue) ? $"{carrier}.{predefinedPackage}" : string.Empty,
                AvailablePredefinedPackages = availablePredefinedPackages,
                EasyPostHtsNumber = htsNumber,
                EasyPostOriginCountry = originCountry,
                AvailableCountries = availableCountries
            };

            return View("~/Plugins/Shipping.EasyPost/Views/Product/_CreateOrUpdate.EasyPost.cshtml", model);
        }

        #endregion
    }
}