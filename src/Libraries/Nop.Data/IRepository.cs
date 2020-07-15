using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Methods

        /// <summary>
        /// Get the entity entry
        /// </summary>
        /// <param name="id">Entity entry identifier</param>
        /// <param name="cacheTime">Cache time in minutes; pass null to use default value; pass 0 to do not cache</param>
        /// <returns>Entity entry</returns>
        TEntity GetById(int? id, int? cacheTime = null);

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <param name="cacheKey">Cache key; pass null to don't cache</param>
        /// <returns>Entity entries</returns>
        IList<TEntity> GetByIds(IList<int> ids, CacheKey cacheKey = null);

        /// <summary>
        /// Insert the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        void Insert(TEntity entity, bool publishEvent = true);

        /// <summary>
        /// Insert entity entries
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entities">Entity entries</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        void Insert(IList<TEntity> entities, bool publishEvent = true);

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        TEntity LoadOriginalCopy(TEntity entity);

        /// <summary>
        /// Update the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        void Update(TEntity entity, bool publishEvent = true);

        /// <summary>
        /// Update entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        void Update(IList<TEntity> entities, bool publishEvent = true);

        /// <summary>
        /// Delete the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        void Delete(TEntity entity, bool publishEvent = true);

        /// <summary>
        /// Delete entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        void Delete(IList<TEntity> entities, bool publishEvent = true);

        /// <summary>
        /// Delete entity entries (soft deletion and event notification are not supported)
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters);

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        void Truncate(bool resetIdentity = false);

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="cacheKey">Cache key; pass null to don't cache</param>
        /// <returns>Entity entries</returns>
        IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, CacheKey cacheKey = null);

        /// <summary>
        /// Get paged list of all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>Paged list of entity entries</returns>
        IPagedList<TEntity> GetAllPaged(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        #endregion
    }
}