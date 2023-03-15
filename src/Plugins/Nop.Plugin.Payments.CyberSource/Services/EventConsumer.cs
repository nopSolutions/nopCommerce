using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.UI;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Payments.CyberSource.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<CustomerPermanentlyDeleted>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<ModelPreparedEvent<BaseNopModel>>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<PageRenderingEvent>
    {
        #region Fields

        protected readonly CustomerTokenService _customerTokenService;
        protected readonly CyberSourceSettings _cyberSourceSettings;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IOrderProcessingService _orderProcessingService;
        protected readonly IOrderService _orderService;
        protected readonly IPaymentPluginManager _paymentPluginManager;
        protected readonly IPaymentService _paymentService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EventConsumer(CustomerTokenService customerTokenService,
            CyberSourceSettings cyberSourceSettings,
            IHttpContextAccessor httpContextAccessor,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _customerTokenService = customerTokenService;
            _cyberSourceSettings = cyberSourceSettings;
            _httpContextAccessor = httpContextAccessor;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle customer permanently deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerPermanentlyDeleted eventMessage)
        {
            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (!_cyberSourceSettings.TokenizationEnabled)
                return;

            //delete customer tokens
            var tokens = await _customerTokenService.GetAllTokensAsync(eventMessage.CustomerId);
            foreach (var token in tokens)
            {
                await _customerTokenService.DeleteAsync(token);
            }
        }

        /// <summary>
        /// Handle model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage.Model is not CustomerNavigationModel navigationModel)
                return;

            //ensure that CyberSource payment method is active for the current customer, since it's the event from the public area
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName, customer, store.Id))
                return;

            if (!_cyberSourceSettings.TokenizationEnabled)
                return;

            //add a new menu item
            var orderItem = navigationModel.CustomerNavigationItems.FirstOrDefault(item => item.Tab == (int)CustomerNavigationEnum.Orders);
            var position = navigationModel.CustomerNavigationItems.IndexOf(orderItem) + 1;
            navigationModel.CustomerNavigationItems.Insert(position, new CustomerNavigationItemModel
            {
                RouteName = CyberSourceDefaults.CustomerTokensRouteName,
                ItemClass = CyberSourceDefaults.CustomerTokensMenuClassName,
                Tab = CyberSourceDefaults.CustomerTokensMenuTab,
                Title = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PaymentTokens")
            });
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Order> eventMessage)
        {
            if (eventMessage.Entity is not Order order)
                return;

            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (order.PaymentMethodSystemName != CyberSourceDefaults.SystemName)
                return;

            //save authorize and capture date
            var customValueChanged = false;
            var customValues = _paymentService.DeserializeCustomValues(order);
            if (!string.IsNullOrEmpty(order.AuthorizationTransactionId) &&
                !customValues.ContainsKey(CyberSourceDefaults.AuthorizationDateCustomValue))
            {
                customValues.Add(CyberSourceDefaults.AuthorizationDateCustomValue, DateTime.UtcNow);
                customValueChanged = true;
            }

            if (!string.IsNullOrEmpty(order.CaptureTransactionId) &&
                !customValues.ContainsKey(CyberSourceDefaults.CaptureDateCustomValue))
            {
                customValues.Add(CyberSourceDefaults.CaptureDateCustomValue, DateTime.UtcNow);
                customValueChanged = true;
            }

            if (customValueChanged)
            {
                order.CustomValuesXml = _paymentService.SerializeCustomValues(new ProcessPaymentRequest
                {
                    CustomValues = customValues
                });

                await _orderService.UpdateOrderAsync(order);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order is not Order order)
                return;

            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (order.PaymentMethodSystemName != CyberSourceDefaults.SystemName)
                return;

            var key = string.Format(CyberSourceDefaults.OrderStatusesSessionKey, order.OrderGuid);
            var (orderStatus, paymentStatus) = await _httpContextAccessor.HttpContext.Session.GetAsync<(OrderStatus?, PaymentStatus?)>(key);
            if (!orderStatus.HasValue || !paymentStatus.HasValue)
                return;

            //remove value from session
            await _httpContextAccessor.HttpContext.Session.RemoveAsync(key);

            var note = $"Order status has been changed to {orderStatus.Value} by CyberSource AVS/CVN/decision profile results";
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = note,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            //update order status for AVS or CVN decline
            if (orderStatus.Value == OrderStatus.Cancelled)
                await _orderProcessingService.CancelOrderAsync(order, false);

            if (orderStatus.Value == OrderStatus.Pending)
            {
                //set payment status for future use
                await _genericAttributeService.SaveAttributeAsync(order, CyberSourceDefaults.PaymentStatusAttributeName, paymentStatus);

                await _orderProcessingService.CheckOrderStatusAsync(order);
            }
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(PageRenderingEvent eventMessage)
        {
            var routeName = eventMessage.GetRouteName();
            if (routeName is null || !routeName.Equals(CyberSourceDefaults.OnePageCheckoutRouteName))
                return;

            //ensure that CyberSource payment method is active for the current customer, since it's the event from the public area
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName, customer, store.Id))
                return;

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return;

            if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                return;

            //add sсript to the one page checkout
            eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, CyberSourceDefaults.FlexMicroformScriptUrl, excludeFromBundle: true);
        }

        #endregion
    }
}
