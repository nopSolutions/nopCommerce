using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MySql.Data.MySqlClient;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Plugin.Data.MySQL.Data
{
    /// <summary>
    /// Represents MySQL data provider
    /// </summary>
    public class MySQLDataProvider : IDataProvider
    {
        #region Methods

        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitializeDatabase()
        {
            var context = EngineContext.Current.Resolve<IDbContext>();

            //check some of table names to ensure that we have nopCommerce 2.00+ installed
            var tableNamesToValidate = new List<string> { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };
            var existingTableNames = context
                .QueryFromSql<StringQueryType>("SELECT table_name AS Value FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'")
                .Select(stringValue => stringValue.Value).ToList();
            var createTables = !existingTableNames.Intersect(tableNamesToValidate, StringComparer.InvariantCultureIgnoreCase).Any();
            if (!createTables)
                return;

            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            //create tables
            //EngineContext.Current.Resolve<IRelationalDatabaseCreator>().CreateTables();
            //(context as DbContext).Database.EnsureCreated();
            context.ExecuteSqlScript(context.GenerateCreateScript());

            //create indexes
            context.ExecuteSqlScriptFromFile(fileProvider.MapPath(NopMySQLDataDefaults.MySQLIndexesFilePath));
        }

        /// <summary>
        /// Get a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new MySqlParameter();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public virtual bool BackupSupported => true;

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        public virtual int SupportedLengthOfBinaryHash => 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.

        #endregion
    }
}
