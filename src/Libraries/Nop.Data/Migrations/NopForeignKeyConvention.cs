using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations
{
    public class NopForeignKeyConvention : IForeignKeyConvention
    {
        private readonly IDataProvider _dataProvider;
        private readonly IMigrationContext _context;

        public NopForeignKeyConvention(IDataProvider dataProvider, IMigrationContext context)
        {
            _dataProvider = dataProvider;
            _context = context;
        }

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
            var foreignColumns = string.Join('_', foreignKey.ForeignColumns);
            var primaryColumns = string.Join('_', foreignKey.PrimaryColumns);

            var keyName = _dataProvider.GetForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns);
            
            if (_context.QuerySchema.ConstraintExists(foreignKey.ForeignTableSchema, foreignKey.ForeignTable, keyName))
                keyName = _dataProvider.GetForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns, false);

            return keyName;
        }
    }
}