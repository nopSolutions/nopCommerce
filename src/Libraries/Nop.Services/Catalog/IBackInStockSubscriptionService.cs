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
        void DeleteSubscription(BackInStockSubscription subscription);
        
        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Subscriptions</returns>
        IPagedList<BackInStockSubscription> GetAllSubscriptionsByCustomerId(int customerId,
            int storeId, int pageIndex, int pageSize, bool showHidden = false);
        
        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Subscriptions</returns>
        IPagedList<BackInStockSubscription> GetAllSubscriptionsByProductVariantId(int productVariantId,
            int storeId, int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Subscriptions</returns>
        BackInStockSubscription FindSubscription(int customerId, int productVariantId, int storeId);

        /// <summary>
        /// Gets a subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>Subscription</returns>
        BackInStockSubscription GetSubscriptionById(int subscriptionId);

        /// <summary>
        /// Inserts subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void InsertSubscription(BackInStockSubscription subscription);

        /// <summary>
        /// Updates subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void UpdateSubscription(BackInStockSubscription subscription);

        /// <summary>
        /// Send notification to subscribers
        /// </summary>
        /// <param name="productVariant"></param>
        /// <returns>Number of sent email</returns>
        int SendNotificationsToSubscribers(ProductVariant productVariant);
    }
}
