using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Core;
using Nop.Data;

namespace Nop.Plugin.Tax.CountryStateZip.Data
{
    /// <summary>
    /// Object context
    /// </summary>
    public class TaxRateObjectContext : DbContext, IDbContext
    {
        public TaxRateObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TaxRateMap());

            //disable EdmMetadata generation
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
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
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<TaxRateObjectContext>(null);

            //create the table
            var dbScript = CreateDatabaseScript();
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        public void Uninstall()
        {
            //drop the table
            var dbScript = "DROP TABLE TaxRate";
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }
    }
}