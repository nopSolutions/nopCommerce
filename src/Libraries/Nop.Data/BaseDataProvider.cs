using System.Collections.Generic;
using System.Data;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data
{
    public abstract class BaseDataProvider
    {
        public abstract IDbConnection CreateDbConnection();
        public abstract NopDataConnection CreateDataContext();

        public NopDataConnection CreateDataContext(IDataProvider dataProvider)
        {
            var dataContext = new NopDataConnection(dataProvider, CreateDbConnection());
            dataContext.AddMappingSchema(AdditionalSchema);

            return dataContext;
        }

        public EntityDescriptor GetEntityDescriptor<TEntity>() where TEntity : BaseEntity
        {
            return AdditionalSchema?.GetEntityDescriptor(typeof(TEntity));
        }

        /// <summary>
        /// Returns queryable source for specified mapping class for current connection,
        /// mapped to database table or view.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual ITable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
        {
            return new DataContext { MappingSchema = AdditionalSchema }.GetTable<TEntity>();
        }

        public static MappingSchema AdditionalSchema { get; } = new MappingSchema();
    }
}