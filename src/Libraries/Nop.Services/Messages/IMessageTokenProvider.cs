using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Messages;

/// <summary>
/// Message token provider
/// </summary>
public partial interface IMessageTokenProvider
{
    /// <summary>
    /// Add store tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="store">Store</param>
    /// <param name="emailAccount">Email account</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddStoreTokensAsync(IList<Token> tokens, Store store, EmailAccount emailAccount, int languageId);

    /// <summary>
    /// Add order tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="order"></param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="vendorId">Vendor identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId, int vendorId = 0);

    /// <summary>
    /// Add refunded order tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="order">Order</param>
    /// <param name="refundedAmount">Refunded amount of order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddOrderRefundedTokensAsync(IList<Token> tokens, Order order, decimal refundedAmount);

    /// <summary>
    /// Add shipment tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="shipment">Shipment item</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddShipmentTokensAsync(IList<Token> tokens, Shipment shipment, int languageId);

    /// <summary>
    /// Add order note tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="orderNote">Order note</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddOrderNoteTokensAsync(IList<Token> tokens, OrderNote orderNote);

    /// <summary>
    /// Add recurring payment tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddRecurringPaymentTokensAsync(IList<Token> tokens, RecurringPayment recurringPayment);

    /// <summary>
    /// Add return request tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="returnRequest">Return request</param>
    /// <param name="orderItem">Order item</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddReturnRequestTokensAsync(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem, int languageId);

    /// <summary>
    /// Add gift card tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="giftCard">Gift card</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddGiftCardTokensAsync(IList<Token> tokens, GiftCard giftCard, int languageId);

    /// <summary>
    /// Add customer tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddCustomerTokensAsync(IList<Token> tokens, int customerId);

    /// <summary>
    /// Add customer tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddCustomerTokensAsync(IList<Token> tokens, Customer customer);

    /// <summary>
    /// Add vendor tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddVendorTokensAsync(IList<Token> tokens, Vendor vendor);

    /// <summary>
    /// Add newsletter subscription tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="subscription">Newsletter subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription);

    /// <summary>
    /// Add product review tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="productReview">Product review</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddProductReviewTokensAsync(IList<Token> tokens, ProductReview productReview);

    /// <summary>
    /// Add blog comment tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="blogComment">Blog post comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment);

    /// <summary>
    /// Add news comment tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="newsComment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment);

    /// <summary>
    /// Add product tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="product">Product</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddProductTokensAsync(IList<Token> tokens, Product product, int languageId);

    /// <summary>
    /// Add product attribute combination tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="combination">Product attribute combination</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddAttributeCombinationTokensAsync(IList<Token> tokens, ProductAttributeCombination combination, int languageId);

    /// <summary>
    /// Add forum tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddForumTokensAsync(IList<Token> tokens, Forum forum);

    /// <summary>
    /// Add forum topic tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
    /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
        int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

    /// <summary>
    /// Add forum post tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="forumPost">Forum post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost);

    /// <summary>
    /// Add private message tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="privateMessage">Private message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage);

    /// <summary>
    /// Add tokens of BackInStock subscription
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="subscription">BackInStock subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddBackInStockTokensAsync(IList<Token> tokens, BackInStockSubscription subscription);

    /// <summary>
    /// Get collection of allowed (supported) message tokens for campaigns
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of allowed (supported) message tokens for campaigns
    /// </returns>
    Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync();

    /// <summary>
    /// Get collection of allowed (supported) message tokens
    /// </summary>
    /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of allowed message tokens
    /// </returns>
    Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null);

    /// <summary>
    /// Get token groups of message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>Collection of token group names</returns>
    IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);
}