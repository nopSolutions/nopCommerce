using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Represents the Entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private INopDataProvider _dataProvider;

        public EntityRepository(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        #region Fields

        private ITable<TEntity> _entities;

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(object id)
        {
            return Entities.FirstOrDefault(e => e.Id == Convert.ToInt32(id));
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                entity.Id = dataConnection.InsertWithInt32Identity(entity);
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                dataConnection.BeginTransaction();

                try
                {
                    dataConnection.BulkInsertEntities(entities);
                    dataConnection.CommitTransaction();
                }
                catch
                {
                    dataConnection.RollbackTransaction();
                    throw;
                }
            }
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy(TEntity entity)
        {
            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                var entities = dataConnection.GetTable<TEntity>();
                return entities.FirstOrDefault(e => e.Id == Convert.ToInt32(entity.Id));
            }
        }
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                dataConnection.Update(entity);
            }
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                dataConnection.Delete(entity);
            }
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        public virtual IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters)
        {
            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                return dataConnection.QueryProc<TEntity>(storeProcedureName, dataParameters?.ToArray());
            }
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        public virtual void Truncate(bool resetIdentity = false)
        {
            using (var dataConnection = _dataProvider.CreateDataContext())
            {
                dataConnection.GetTable<TEntity>().Truncate(resetIdentity);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual ITable<TEntity> Entities => _entities ?? (_entities = _dataProvider.GetTable<TEntity>());

        #endregion
    }
}