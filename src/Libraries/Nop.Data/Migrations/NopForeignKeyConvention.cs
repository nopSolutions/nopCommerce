using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations
{
    public class NopForeignKeyConvention : IForeignKeyConvention
    {
        private readonly IMigrationContext _context;

        public NopForeignKeyConvention(IMigrationContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public IForeignKeyExpression Apply(IForeignKeyExpression expression)
        {
            if (string.IsNullOrEmpty(expression.ForeignKey.Name))
            {
                expression.ForeignKey.Name = GetForeignKeyName(expression.ForeignKey);
            }
            
            return expression;
        }

        private string GetForeignKeyName(ForeignKeyDefinition foreignKey)
        {
            var sb = new StringBuilder();

            sb.Append("FK_");
            
            if (DatabaseTypeApplies("MySQL"))
            {
                sb.Append("_");
                sb.Append(Guid.NewGuid().ToString("D"));
                return sb.ToString();
            }

            sb.Append(foreignKey.ForeignTable);

            foreach (var foreignColumn in foreignKey.ForeignColumns)
            {
                sb.Append("_");
                sb.Append(foreignColumn);
            }

            sb.Append("_");
            sb.Append(foreignKey.PrimaryTable);

            foreach (var primaryColumn in foreignKey.PrimaryColumns)
            {
                sb.Append("_");
                sb.Append(primaryColumn);
            }

            return sb.ToString();
        }

        private bool DatabaseTypeApplies(params string[] databaseTypes)
        {
            if (_context.QuerySchema is IMigrationProcessor mp)
            {
                var processorDbTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { mp.DatabaseType };
                foreach (var databaseType in mp.DatabaseTypeAliases)
                    processorDbTypes.Add(databaseType);

                return databaseTypes
                    .Any(db => processorDbTypes.Contains(db));
            }

            return false;
        }
    }
}