using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.FacebookPixel.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<CustomerRegisteredEvent>,
        IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
        IConsumer<MessageTokensAddedEvent<Token>>,
        IConsumer<ModelPreparedEvent<BaseNopModel>>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<PageRenderingEvent>,
        IConsumer<ProductSearchEvent>
    {
        #region Fields

        protected readonly FacebookPixelService _facebookPixelService;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public EventConsumer(FacebookPixelService facebookPixelService,
            IHttpContextAccessor httpContextAccessor)
        {
            _facebookPixelService = facebookPixelService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shopping cart item inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            if (eventMessage?.Entity != null)
                await _facebookPixelService.SendAddToCartEventAsync(eventMessage.Entity);
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order != null)
                await _facebookPixelService.SendPurchaseEventAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle product details model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage?.Model is ProductDetailsModel productDetailsModel)
                await _facebookPixelService.SendViewContentEventAsync(productDetailsModel);
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(PageRenderingEvent eventMessage)
        {
            var routeName = eventMessage.GetRouteName() ?? string.Empty;
            if (routeName == FacebookPixelDefaults.CheckoutRouteName || routeName == FacebookPixelDefaults.CheckoutOnePageRouteName)
                await _facebookPixelService.SendInitiateCheckoutEventAsync();

            if (_httpContextAccessor.HttpContext.GetRouteValue("area") is not string area || area != AreaNames.Admin)
                await _facebookPixelService.SendPageViewEventAsync();
        }

        /// <summary>
        /// Handle product search event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ProductSearchEvent eventMessage)
        {
            if (eventMessage?.SearchTerm != null)
                await _facebookPixelService.SendSearchEventAsync(eventMessage.SearchTerm);
        }

        /// <summary>
        /// Handle message token added event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(MessageTokensAddedEvent<Token> eventMessage)
        {
            if (eventMessage?.Message?.Name == MessageTemplateSystemNames.ContactUsMessage)
                await _facebookPixelService.SendContactEventAsync();
        }

        /// <summary>
        /// Handle customer registered event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
        {
            if (eventMessage?.Customer != null)
                await _facebookPixelService.SendCompleteRegistrationEventAsync();
        }

        #endregion
    }
}