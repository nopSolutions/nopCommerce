using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IStaticCacheManager _cacheManager;
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
            IStaticCacheManager cacheManager,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _checkoutAttributeService = checkoutAttributeService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _productService = productService;
            _cacheManager = cacheManager;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            //ensure that model is passed
            if (!(additionalData is BaseNopEntityModel entityModel))
                return Content(string.Empty);

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content(string.Empty);

            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
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
            var cachedEntityUseCodes = _cacheManager.Get(AvalaraTaxDefaults.EntityUseCodesCacheKey, () => _avalaraTaxManager.GetEntityUseCodes());
            var entityUseCodes = cachedEntityUseCodes?.Select(useCode => new SelectListItem
            {
                Value = useCode.code,
                Text = $"{useCode.name} ({useCode.validCountries.Aggregate(string.Empty, (list, country) => $"{list}{country},").TrimEnd(',')})"
            }).ToList() ?? new List<SelectListItem>();

            //add the special item for 'undefined' with empty guid value
            var defaultValue = Guid.Empty.ToString();
            entityUseCodes.Insert(0, new SelectListItem
            {
                Value = defaultValue,
                Text = _localizationService.GetResource("Plugins.Tax.Avalara.Fields.EntityUseCode.None")
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
                entity = _customerService.GetCustomerById(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop))
            {
                model.PrecedingElementId = nameof(CustomerRoleModel.TaxExempt);
                entity = _customerService.GetCustomerRoleById(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
            {
                model.PrecedingElementId = nameof(ProductModel.IsTaxExempt);
                entity = _productService.GetProductById(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
            {
                model.PrecedingElementId = nameof(CheckoutAttributeModel.IsTaxExempt);
                entity = _checkoutAttributeService.GetCheckoutAttributeById(entityModel.Id);
            }

            //try to get previously saved entity use code
            model.AvalaraEntityUseCode = entity == null ? defaultValue :
                _genericAttributeService.GetAttribute<string>(entity, AvalaraTaxDefaults.EntityUseCodeAttribute);

            return View("~/Plugins/Tax.Avalara/Views/EntityUseCode/EntityUseCode.cshtml", model);
        }

        #endregion
    }
}