using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityDeletedEvent<Order>>,
        IConsumer<ModelReceivedEvent<BaseNopModel>>,
        IConsumer<OrderCancelledEvent>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<OrderRefundedEvent>,
        IConsumer<OrderVoidedEvent>
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public EventConsumer(AvalaraTaxManager avalaraTaxManager,
            ICheckoutAttributeService checkoutAttributeService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            IPermissionService permissionService,
            IProductService productService,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _checkoutAttributeService = checkoutAttributeService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _permissionService = permissionService;
            _productService = productService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
        {
            //get entity by received model
            var entity = eventMessage.Model switch
            {
                CustomerModel customerModel => (BaseEntity)await _customerService.GetCustomerByIdAsync(customerModel.Id),
                CustomerRoleModel customerRoleModel => await _customerService.GetCustomerRoleByIdAsync(customerRoleModel.Id),
                ProductModel productModel => await _productService.GetProductByIdAsync(productModel.Id),
                CheckoutAttributeModel checkoutAttributeModel => await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeModel.Id),
                _ => null
            };
            if (entity == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return;

            //whether there is a form value for the entity use code
            if (_httpContextAccessor.HttpContext.Request.Form.TryGetValue(AvalaraTaxDefaults.EntityUseCodeAttribute, out var entityUseCodeValue)
                && !StringValues.IsNullOrEmpty(entityUseCodeValue))
            {
                //save attribute
                var entityUseCode = !entityUseCodeValue.ToString().Equals(Guid.Empty.ToString()) ? entityUseCodeValue.ToString() : null;
                await _genericAttributeService.SaveAttributeAsync(entity, AvalaraTaxDefaults.EntityUseCodeAttribute, entityUseCode);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active
            var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer, eventMessage.Order.StoreId))
                return;

            //create tax transaction
            await _avalaraTaxManager.CreateOrderTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order refunded event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //refund tax transaction
            await _avalaraTaxManager.RefundTaxTransactionAsync(eventMessage.Order, eventMessage.Amount);
        }

        /// <summary>
        /// Handle order async Tasked event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderVoidedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //async Task tax transaction
            await _avalaraTaxManager.VoidTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order cancelled event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderCancelledEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //async Task tax transaction
            await _avalaraTaxManager.VoidTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Order> eventMessage)
        {
            if (eventMessage.Entity == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //delete tax transaction
            await _avalaraTaxManager.DeleteTaxTransactionAsync(eventMessage.Entity);
        }

        #endregion
    }
}