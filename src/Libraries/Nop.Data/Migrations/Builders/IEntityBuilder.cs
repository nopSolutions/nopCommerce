using FluentMigrator.Builders.Create.Table;
using Nop.Core;

namespace Nop.Data.Migrations.Builders
{
    public interface IEntityBuilder<TEntity> where TEntity : BaseEntity
    {
        void MapEntity(CreateTableExpressionBuilder table);

        string TableName { get; }
    }
}
