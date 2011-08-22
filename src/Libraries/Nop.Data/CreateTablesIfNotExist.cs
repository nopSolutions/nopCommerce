using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Nop.Data
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private string[] _tablesToValidate;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tablesToValidate">A list of existing table names to validate; null to don't validate table names</param>
        public CreateTablesIfNotExist(string[] tablesToValidate)
            : base()
        {
            this._tablesToValidate = tablesToValidate;
        }
        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (dbExists)
            {
                bool createTables = false;
                if (_tablesToValidate != null && _tablesToValidate.Length > 0)
                {
                    //we have some table names to validate
                    var existingTableNames = new List<string>(context.Database.SqlQuery<string>("SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table'"));
                    createTables = existingTableNames.Intersect(_tablesToValidate, StringComparer.InvariantCultureIgnoreCase).Count() == 0;
                }
                else
                {
                    //check whether tables are already created
                    int numberOfTables = 0;
                    foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table' "))
                        numberOfTables = t1;

                    createTables = numberOfTables == 0;
                }

                if (createTables)
                {
                    //create all tables
                    var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreationScript);

                    //Seed(context);
                    context.SaveChanges();

                    AddIndexes(context);
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }
        protected virtual void AddIndexes(TContext context)
        {
            //Add SQL Server indexes for performance optimization
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [dbo].[LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId] ON [dbo].[ProductVariant] ([ProductId])	INCLUDE ([Price],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc],[Published],[Deleted])");
        }
    }
}
