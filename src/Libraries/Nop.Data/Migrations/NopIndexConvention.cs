using System.Linq;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations
{
    public class NopIndexConvention : IIndexConvention
    {

        private readonly INopDataProvider _dataProvider;

        public NopIndexConvention(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
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
            return _dataProvider.GetIndexName(index.TableName, string.Join('_', index.Columns.Select(c => c.Name)));
        }
    }
}
