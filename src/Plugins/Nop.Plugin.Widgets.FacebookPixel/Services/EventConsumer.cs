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
        public void HandleEvent(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            if (eventMessage?.Entity != null)
                _facebookPixelService.PrepareAddToCartScript(eventMessage.Entity);
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order != null)
                _facebookPixelService.PreparePurchaseScript(eventMessage.Order);
        }

        /// <summary>
        /// Handle product details model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage?.Model is ProductDetailsModel productDetailsModel)
                _facebookPixelService.PrepareViewContentScript(productDetailsModel);
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PageRenderingEvent eventMessage)
        {
            var routeName = eventMessage.GetRouteName() ?? string.Empty;
            if (routeName == FacebookPixelDefaults.CheckoutRouteName || routeName == FacebookPixelDefaults.CheckoutOnePageRouteName)
                _facebookPixelService.PrepareInitiateCheckoutScript();
        }

        /// <summary>
        /// Handle product search event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(ProductSearchEvent eventMessage)
        {
            if (!string.IsNullOrEmpty(eventMessage?.SearchTerm))
                _facebookPixelService.PrepareSearchScript(eventMessage.SearchTerm);
        }

        /// <summary>
        /// Handle message token added event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(MessageTokensAddedEvent<Token> eventMessage)
        {
            if (eventMessage?.Message?.Name == MessageTemplateSystemNames.ContactUsMessage)
                _facebookPixelService.PrepareContactScript();
        }

        /// <summary>
        /// Handle customer registered event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(CustomerRegisteredEvent eventMessage)
        {
            if (eventMessage?.Customer != null)
                _facebookPixelService.PrepareCompleteRegistrationScript();
        }

        #endregion
    }
}