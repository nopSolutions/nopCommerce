using FluentMigrator.Builders.Create.Table;

namespace Nop.Data.Migrations.Builders
{
    public interface IEntityBuilder
    {
        void MapEntity(CreateTableExpressionBuilder table);

        string TableName { get; }
    }
}
