using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class NopEntityTypeConfiguration<TEntity> : IMappingConfiguration where TEntity : BaseEntity
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        public abstract void Configure(EntityMappingBuilder<TEntity> builder);

        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        public void ApplyConfiguration(FluentMappingBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TEntity>();
            builder.HasPrimaryKey(entity => entity.Id).HasIdentity(entity => entity.Id);
            builder.Ignore(entity => entity.EntityCacheKey);

            Configure(builder);
        }

        /// <summary>
        /// Creates new table in database for mapping class
        /// </summary>
        public virtual void CreateTableIfNotExists(DataConnection dataConnection)
        {
            if (dataConnection is null)
                throw new ArgumentNullException(nameof(dataConnection));

            var sp = dataConnection.DataProvider.GetSchemaProvider();
            var dbSchema = sp.GetSchema(dataConnection);

            if (dbSchema.Tables.Any(t => t.TypeName == typeof(TEntity).Name))
                return;

            //no required table-create it
            try
            {
                dataConnection.CreateTable<TEntity>();
            }
            catch (System.Data.Common.DbException)
            {
            }
        }

        /// <summary>
        /// Drops table identified by mapping class
        /// </summary>
        public virtual void DeleteTableIfExists()
        {
            var dataConnection = new NopDataConnection();

            var sp = dataConnection.DataProvider.GetSchemaProvider();
            var dbSchema = sp.GetSchema(dataConnection);

            if (dbSchema.Tables.All(t => t.TypeName != typeof(TEntity).Name))
                return;

            //table exists delete it
            try
            {
                dataConnection.DropTable<TEntity>();
            }
            catch (System.Data.SqlClient.SqlException)
            {
            }
        }

        #endregion
    }
}