using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Tax.Avalara.Models.EntityUseCode;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to render an additional field on customer details, customer role details, product details, checkout attribute details views
    /// </summary>
    [ViewComponent(Name = AvalaraTaxDefaults.ENTITY_USE_CODE_VIEW_COMPONENT_NAME)]
    public class EntityUseCodeViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public EntityUseCodeViewComponent(AvalaraTaxManager avalaraTaxManager,
            ICheckoutAttributeService checkoutAttributeService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _checkoutAttributeService = checkoutAttributeService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _taxPluginManager = taxPluginManager;
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
            //ensure that model is passed
            if (additionalData is not BaseNopEntityModel entityModel)
                return Content(string.Empty);

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return Content(string.Empty);

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content(string.Empty);

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock) &&
                !widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop) &&
                !widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock) &&
                !widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
            {
                return Content(string.Empty);
            }

            //get Avalara pre-defined entity use codes
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.EntityUseCodesCacheKey);
            var cachedEntityUseCodes = await _staticCacheManager.GetAsync(cacheKey, async () => await _avalaraTaxManager.GetEntityUseCodesAsync());

            var entityUseCodes = cachedEntityUseCodes?.Select(useCode => new SelectListItem
            {
                Value = useCode.code,
                Text = $"{useCode.name} ({string.Join(", ", useCode.validCountries)})"
            }).ToList() ?? new List<SelectListItem>();

            //add the special item for 'undefined' with empty guid value
            var defaultValue = Guid.Empty.ToString();
            entityUseCodes.Insert(0, new SelectListItem
            {
                Value = defaultValue,
                Text = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.EntityUseCode.None")
            });

            //prepare model
            var model = new EntityUseCodeModel
            {
                Id = entityModel.Id,
                EntityUseCodes = entityUseCodes
            };

            //get entity by the model identifier
            BaseEntity entity = null;
            if (widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock))
            {
                model.PrecedingElementId = nameof(CustomerModel.IsTaxExempt);
                entity = await _customerService.GetCustomerByIdAsync(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop))
            {
                model.PrecedingElementId = nameof(CustomerRoleModel.TaxExempt);
                entity = await _customerService.GetCustomerRoleByIdAsync(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
            {
                model.PrecedingElementId = nameof(ProductModel.IsTaxExempt);
                entity = await _productService.GetProductByIdAsync(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
            {
                model.PrecedingElementId = nameof(CheckoutAttributeModel.IsTaxExempt);
                entity = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(entityModel.Id);
            }

            //try to get previously saved entity use code
            model.AvalaraEntityUseCode = entity == null ? defaultValue :
                await _genericAttributeService.GetAttributeAsync<string>(entity, AvalaraTaxDefaults.EntityUseCodeAttribute);

            return View("~/Plugins/Tax.Avalara/Views/EntityUseCode/EntityUseCode.cshtml", model);
        }

        #endregion
    }
}