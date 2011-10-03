using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Core;
using Nop.Data;

namespace Nop.Plugin.Feed.Froogle.Data
{
    /// <summary>
    /// Object context
    /// </summary>
    public class GoogleProductObjectContext : DbContext, IDbContext
    {
        public GoogleProductObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GoogleProductRecordMap());

            //disable EdmMetadata generation
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
         base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Install
        /// </summary>
        public void Install()
        {
            //create all tables
            //TODO don't create the table if it already exists
            var dbScript = CreateDatabaseScript();
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        public void Uninstall()
        {
            //TODO don't drop the table if it already exists

            try
            {
                //we place it in try-catch here because previous versions of Froogle didn't have any tables
                var dbScript = "DROP TABLE GoogleProduct";
                Database.ExecuteSqlCommand(dbScript);
                SaveChanges();
            }
            catch
            {
            }
        }


        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }
    }
}