using System;
using System.Data;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Model;
using Nop.Core;
using Nop.Data.Extensions;
using Nop.Data.Migrations.Builders;

namespace Nop.Data.Migrations
{
    [Tags(NopMigrationTags.Schema)]
    public abstract class BaseInitMigration : AutoReversingMigration
    {
        protected virtual void BuildEntity<TEntity>(string tableName = null,
                IEntityBuilder builder = null) where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);
            var tblName = string.IsNullOrEmpty(tableName) ? typeof(TEntity).Name : tableName;

            var create = Create.Table(tblName) as CreateTableExpressionBuilder;
            var expression = create.Expression;

            if (builder != null)
                builder.MapEntity(create);

            if (!expression.Columns.Any(c => c.IsPrimaryKey))
            {
                var pk = new ColumnDefinition
                {
                    Name = nameof(BaseEntity.Id),
                    Type = DbType.Int32,
                    IsIdentity = true,
                    TableName = tblName,
                    ModificationType = ColumnModificationType.Create,
                    IsPrimaryKey = true
                };

                expression.Columns.Insert(0, pk);
            }

            var propertiesToAutoMap = typeof(TEntity)
                .GetProperties()
                .Where(p =>
                    !expression.Columns.Any(x => x.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)) &&
                    !nameof(BaseEntity.Id).Equals(p.Name, StringComparison.OrdinalIgnoreCase));

            if (propertiesToAutoMap is null || propertiesToAutoMap.Count() == 0)
                return;

            foreach (var prop in propertiesToAutoMap)
            {
                create.WithSelfType(prop.Name, prop.PropertyType);
            }
        }

    }
}
