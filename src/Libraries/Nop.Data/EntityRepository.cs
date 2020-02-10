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

            using (var _dataConnection = new NopDataConnection())
            {
                entity.Id = _dataConnection.InsertWithInt32Identity(entity);
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
            using (var _dataConnection = new NopDataConnection())
            {
                _dataConnection.BeginTransaction();

                try
                {
                    foreach (var entity in entities)
                    {
                        Insert(entity);
                    }

                    _dataConnection.CommitTransaction();
                }
                catch
                {
                    _dataConnection.RollbackTransaction();
                    throw;
                }
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

            using (var _dataConnection = new NopDataConnection())
            {
                _dataConnection.Update(entity);
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

            using (var _dataConnection = new NopDataConnection())
            {
                _dataConnection.Delete(entity);
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
            using (var _dataConnection = new NopDataConnection())
            {
                return _dataConnection.ExecuteStoredProcedure<TEntity>(storeProcedureName, dataParameters?.ToArray());
            }
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        public virtual void Truncate(bool resetIdentity = false)
        {
            using (var _dataConnection = new NopDataConnection())
            {
                _dataConnection.GetTable<TEntity>().Truncate(resetIdentity);
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
        protected virtual ITable<TEntity> Entities => _entities ?? (_entities = new DataContext
        {
            MappingSchema = NopDataConnection.AdditionalSchema
        }.GetTable<TEntity>());

        #endregion
    }
}