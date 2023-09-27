using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<CustomerActivatedEvent>,
        IConsumer<CustomerPermanentlyDeleted>,
        IConsumer<EntityDeletedEvent<Order>>,
        IConsumer<ModelPreparedEvent<BaseNopModel>>,
        IConsumer<ModelReceivedEvent<BaseNopModel>>,
        IConsumer<OrderStatusChangedEvent>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<OrderRefundedEvent>,
        IConsumer<OrderVoidedEvent>
    {
        #region Fields

        protected readonly AvalaraTaxManager _avalaraTaxManager;
        protected readonly AvalaraTaxSettings _avalaraTaxSettings;
        protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
        protected readonly ICustomerService _customerService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IProductService _productService;
        protected readonly ITaxPluginManager _taxPluginManager;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EventConsumer(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IProductService productService,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _checkoutAttributeService = checkoutAttributeService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _productService = productService;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle customer activated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerActivatedEvent eventMessage)
        {
            if (eventMessage.Customer is null)
                return;

            //ensure that Avalara tax provider is active for the passed customer, since it's the event from the public area
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, eventMessage.Customer))
                return;

            if (!_avalaraTaxSettings.EnableCertificates)
                return;

            //create customer
            await _avalaraTaxManager.CreateOrUpdateCustomerAsync(eventMessage.Customer);
        }

        /// <summary>
        /// Handle customer permanently deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerPermanentlyDeleted eventMessage)
        {
            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            if (!_avalaraTaxSettings.EnableCertificates)
                return;

            //delete customer
            await _avalaraTaxManager.DeleteCustomerAsync(eventMessage.CustomerId);
        }

        /// <summary>
        /// Handle model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            var customerModel = eventMessage.Model as CustomerInfoModel;
            var navigationModel = eventMessage.Model as CustomerNavigationModel;
            if (customerModel is null && navigationModel is null)
                return;

            //ensure that Avalara tax provider is active for the passed customer, since it's the event from the public area
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
                return;

            if (!_avalaraTaxSettings.EnableCertificates)
                return;

            if (navigationModel is not null)
            {
                //ACL
                if (_avalaraTaxSettings.CustomerRoleIds.Any())
                {
                    var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
                    if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                        return;
                }

                var infoItem = navigationModel.CustomerNavigationItems.FirstOrDefault(item => item.Tab == (int)CustomerNavigationEnum.Info);
                var position = navigationModel.CustomerNavigationItems.IndexOf(infoItem) + 1;
                navigationModel.CustomerNavigationItems.Insert(position, new CustomerNavigationItemModel
                {
                    RouteName = AvalaraTaxDefaults.ExemptionCertificatesRouteName,
                    ItemClass = AvalaraTaxDefaults.ExemptionCertificatesMenuClassName,
                    Tab = AvalaraTaxDefaults.ExemptionCertificatesMenuTab,
                    Title = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.ExemptionCertificates")
                });
            }

            if (customerModel is not null && !_avalaraTaxSettings.AllowEditCustomer)
                await _avalaraTaxManager.CreateOrUpdateCustomerAsync(customer);
        }

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
                CheckoutAttributeModel checkoutAttributeModel => await _checkoutAttributeService.GetAttributeByIdAsync(checkoutAttributeModel.Id),
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

            //ensure that Avalara tax provider is active for the passed customer, since it's the event from the public area
            var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
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
        /// Handle order voided event
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

            //void tax transaction
            await _avalaraTaxManager.VoidTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order cancelled event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            if (eventMessage.Order.OrderStatus != OrderStatus.Cancelled)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //void tax transaction
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