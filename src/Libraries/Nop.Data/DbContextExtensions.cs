using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Nop.Core;

namespace Nop.Data 
{
    public static class DbContextExtensions
    {
        #region Utilities

        private static T InnerGetCopy<T>(IDbContext context, T currentCopy, Func<DbEntityEntry<T>, DbPropertyValues> func) where T : BaseEntity
        {
            //Get the database context
            DbContext dbContext = CastOrThrow(context);

            //Get the entity tracking object
            DbEntityEntry<T> entry = GetEntityOrReturnNull(currentCopy, dbContext);

            //The output 
            T output = null;

            //Try and get the values
            if (entry != null)
            {
                DbPropertyValues dbPropertyValues = func(entry);
                if (dbPropertyValues != null)
                {
                    output = dbPropertyValues.ToObject() as T;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the entity or return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentCopy">The current copy.</param>
        /// <param name="dbContext">The db context.</param>
        /// <returns></returns>
        private static DbEntityEntry<T> GetEntityOrReturnNull<T>(T currentCopy, DbContext dbContext) where T : BaseEntity
        {
            return dbContext.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity == currentCopy);
        }

        private static DbContext CastOrThrow(IDbContext context)
        {
            DbContext output = (context as DbContext);

            if (output == null)
            {
                throw new InvalidOperationException("Context does not support operation.");
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the original copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadOriginalCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity
        {
            return InnerGetCopy(context, currentCopy, e => e.OriginalValues);
        }

        /// <summary>
        /// Loads the database copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadDatabaseCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity
        {
            return InnerGetCopy(context, currentCopy, e => e.GetDatabaseValues());
        }

        /// <summary>
        /// Drop a plugin table
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="tableName">Table name</param>
        public static void DropPluginTable(this DbContext context, string tableName)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            //drop the table
            if (context.Database.SqlQuery<int>("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}", tableName).Any<int>())
            {
                var dbScript = "DROP TABLE [" + tableName + "]";
                context.Database.ExecuteSqlCommand(dbScript);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Get table name of entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="context">Context</param>
        /// <returns>Table name</returns>
        public static string GetTableName<T>(this IDbContext context) where T : BaseEntity
        {
            //var tableName = typeof(T).Name;
            //return tableName;

            //this code works only with Entity Framework.
            //If you want to support other database, then use the code above (commented)

            var adapter = ((IObjectContextAdapter)context).ObjectContext;
            var storageModel = (StoreItemCollection)adapter.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
            var containers = storageModel.GetItems<EntityContainer>();
            var entitySetBase = containers.SelectMany(c => c.BaseEntitySets.Where(bes => bes.Name == typeof(T).Name)).First();

            // Here are variables that will hold table and schema name
            string tableName = entitySetBase.MetadataProperties.First(p => p.Name == "Table").Value.ToString();
            //string schemaName = productEntitySetBase.MetadataProperties.First(p => p.Name == "Schema").Value.ToString();
            return tableName;
        }
        #endregion
    }
}