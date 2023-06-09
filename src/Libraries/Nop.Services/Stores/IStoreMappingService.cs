using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store mapping service interface
    /// </summary>
    public partial interface IStoreMappingService
    {
        /// <summary>
        /// Apply store mapping to the passed query
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the filtered query
        /// </returns>
        Task<IQueryable<TEntity>> ApplyStoreMapping<TEntity>(IQueryable<TEntity> query, int storeId) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Deletes a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteStoreMappingAsync(StoreMapping storeMapping);

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store mapping records
        /// </returns>
        Task<IList<StoreMapping>> GetStoreMappingsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertStoreMappingAsync<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store identifiers
        /// </returns>
        Task<int[]> GetStoresIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// The store identifiers
        /// </returns>
        int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// The rue - authorized; otherwise, false
        /// </returns>
        bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported;
    }
}