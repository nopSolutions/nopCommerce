using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
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

        private readonly FacebookPixelService _facebookPixelService;

        #endregion

        #region Ctor

        public EventConsumer(FacebookPixelService facebookPixelService)
        {
            _facebookPixelService = facebookPixelService;
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
                await _facebookPixelService.PrepareAddToCartScriptAsync(eventMessage.Entity);
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order != null)
                await _facebookPixelService.PreparePurchaseScriptAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle product details model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage?.Model is ProductDetailsModel productDetailsModel)
                await _facebookPixelService.PrepareViewContentScriptAsync(productDetailsModel);
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
                await _facebookPixelService.PrepareInitiateCheckoutScriptAsync();
        }

        /// <summary>
        /// Handle product search event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ProductSearchEvent eventMessage)
        {
            if (!string.IsNullOrEmpty(eventMessage?.SearchTerm))
                await _facebookPixelService.PrepareSearchScriptAsync(eventMessage.SearchTerm);
        }

        /// <summary>
        /// Handle message token added event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(MessageTokensAddedEvent<Token> eventMessage)
        {
            if (eventMessage?.Message?.Name == MessageTemplateSystemNames.ContactUsMessage)
                await _facebookPixelService.PrepareContactScriptAsync();
        }

        /// <summary>
        /// Handle customer registered event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
        {
            if (eventMessage?.Customer != null)
                await _facebookPixelService.PrepareCompleteRegistrationScriptAsync();
        }

        #endregion
    }
}