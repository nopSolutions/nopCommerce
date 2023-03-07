using Nop.Plugin.Pickup.PickupInStore.Models;

namespace Nop.Plugin.Pickup.PickupInStore.Factories
{
    /// <summary>
    /// Represents the store pickup point models factory
    /// </summary>
    public interface IStorePickupPointModelFactory
    {
        /// <summary>
        /// Prepare store pickup point list model
        /// </summary>
        /// <param name="searchModel">Store pickup point search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store pickup point list model
        /// </returns>
        Task<StorePickupPointListModel> PrepareStorePickupPointListModelAsync(StorePickupPointSearchModel searchModel);

        /// <summary>
        /// Prepare store pickup point search model
        /// </summary>
        /// <param name="searchModel">Store pickup point search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store pickup point search model
        /// </returns>
        Task<StorePickupPointSearchModel> PrepareStorePickupPointSearchModelAsync(StorePickupPointSearchModel searchModel);
    }
}