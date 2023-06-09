using Nop.Core.Domain.Stores;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store service interface
    /// </summary>
    public partial interface IStoreService
    {
        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteStoreAsync(Store store);

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stores
        /// </returns>
        Task<IList<Store>> GetAllStoresAsync();

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>
        /// The stores
        /// </returns>
        IList<Store> GetAllStores();

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store
        /// </returns>
        Task<Store> GetStoreByIdAsync(int storeId);

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertStoreAsync(Store store);

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateStoreAsync(Store store);

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        void UpdateStore(Store store);

        /// <summary>
        /// Indicates whether a store contains a specified host
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        bool ContainsHostValue(Store store, string host);

        /// <summary>
        /// Returns a list of names of not existing stores
        /// </summary>
        /// <param name="storeIdsNames">The names and/or IDs of the store to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing stores
        /// </returns>
        Task<string[]> GetNotExistingStoresAsync(string[] storeIdsNames);
    }
}