using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.EasyPost.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityDeletedEvent<Shipment>>,
        IConsumer<ModelReceivedEvent<BaseNopModel>>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<ShipmentCreatedEvent>
    {
        #region Fields

        private readonly EasyPostService _easyPostService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IShippingPluginManager _shippingPluginManager;

        #endregion

        #region Ctor

        public EventConsumer(EasyPostService easyPostService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            IPermissionService permissionService,
            IProductService productService,
            IShippingPluginManager shippingPluginManager)
        {
            _easyPostService = easyPostService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
            _productService = productService;
            _shippingPluginManager = shippingPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Shipment> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _easyPostService.DeleteShipmentAsync(eventMessage.Entity);
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage.Model is not ProductModel model)
                return;

            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return;

            var product = await _productService.GetProductByIdAsync(model.Id);
            if (product is null)
                return;

            //try to get additional form values for the product
            var form = _httpContextAccessor.HttpContext.Request.Form;
            if (form.TryGetValue(EasyPostDefaults.ProductPredefinedPackageFormKey, out var predefinedPackageValue))
            {
                var predefinedPackage = !StringValues.IsNullOrEmpty(predefinedPackageValue) ? predefinedPackageValue.ToString() : null;
                await _genericAttributeService.SaveAttributeAsync(product, EasyPostDefaults.ProductPredefinedPackageAttribute, predefinedPackage);
            }
            if (form.TryGetValue(EasyPostDefaults.ProductHtsNumberFormKey, out var htsNumberValue))
            {
                var htsNumber = !StringValues.IsNullOrEmpty(htsNumberValue) ? htsNumberValue.ToString() : null;
                await _genericAttributeService.SaveAttributeAsync(product, EasyPostDefaults.ProductHtsNumberAttribute, htsNumber);
            }
            if (form.TryGetValue(EasyPostDefaults.ProductOriginCountryFormKey, out var originCountryValue))
            {
                var originCountry = !StringValues.IsNullOrEmpty(originCountryValue) ? originCountryValue.ToString() : null;
                await _genericAttributeService.SaveAttributeAsync(product, EasyPostDefaults.ProductOriginCountryAttribute, originCountry);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order is null)
                return;

            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return;

            await _easyPostService.SaveShipmentAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle shipment created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ShipmentCreatedEvent eventMessage)
        {
            if (eventMessage.Shipment is null)
                return;

            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return;

            await _easyPostService.SaveShipmentAsync(eventMessage.Shipment);
        }

        #endregion
    }
}