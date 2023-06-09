using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Back in stock subscription service interface
    /// </summary>
    public partial interface IBackInStockSubscriptionService
    {
        /// <summary>
        /// Delete a back in stock subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSubscriptionAsync(BackInStockSubscription subscription);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subscriptions
        /// </returns>
        Task<IPagedList<BackInStockSubscription>> GetAllSubscriptionsByCustomerIdAsync(int customerId,
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subscriptions
        /// </returns>
        Task<BackInStockSubscription> FindSubscriptionAsync(int customerId, int productId, int storeId);

        /// <summary>
        /// Gets a subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subscription
        /// </returns>
        Task<BackInStockSubscription> GetSubscriptionByIdAsync(int subscriptionId);

        /// <summary>
        /// Inserts subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertSubscriptionAsync(BackInStockSubscription subscription);

        /// <summary>
        /// Send notification to subscribers
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of sent email
        /// </returns>
        Task<int> SendNotificationsToSubscribersAsync(Product product);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subscriptions
        /// </returns>
        Task<IPagedList<BackInStockSubscription>> GetAllSubscriptionsByProductIdAsync(int productId,
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
