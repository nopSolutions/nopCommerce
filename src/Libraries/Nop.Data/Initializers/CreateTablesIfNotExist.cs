using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Nop.Data.Initializers
{
    public class CreateTablesIfNotExist<TContext> where TContext : DbContext
    {
        private readonly string[] _tablesToValidate;
        private readonly string[] _customCommands;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tablesToValidate">A list of existing table names to validate; null to don't validate table names</param>
        /// <param name="customCommands">A list of custom commands to execute</param>
        public CreateTablesIfNotExist(string[] tablesToValidate, string [] customCommands)
        {
            this._tablesToValidate = tablesToValidate;
            this._customCommands = customCommands;
        }

        public void InitializeDatabase(TContext context)
        {
            RelationalDatabaseCreator creator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            creator.EnsureCreated();
            creator.CreateTables();
            if (_customCommands != null && _customCommands.Length > 0)
            {
                foreach (var command in _customCommands)
                    context.Database.ExecuteSqlCommand(command);
            }
        }
    }
}
