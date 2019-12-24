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
    public class NopIndexConvention : IIndexConvention
    {

        private readonly IMigrationContext _context;

        public NopIndexConvention(IMigrationContext context)
        {
            _context = context;
        }

        public IIndexExpression Apply(IIndexExpression expression) 
        {
            if (string.IsNullOrEmpty(expression.Index.Name))
            {
                expression.Index.Name = GetIndexName(expression.Index);
            }

            return expression;
        }

        private string GetIndexName(IndexDefinition index)
        {
            var sb = new StringBuilder();

            sb.Append("IX_");

            if (DatabaseTypeApplies("MySQL"))
            {
                sb.Append("_");
                sb.Append(Guid.NewGuid().ToString("D"));
                return sb.ToString();
            }

            sb.Append(index.TableName);

            foreach (var column in index.Columns)
            {
                sb.Append("_");
                sb.Append(column.Name);
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
