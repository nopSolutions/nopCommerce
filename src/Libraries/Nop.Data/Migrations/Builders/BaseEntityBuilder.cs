using System;
using System.Collections.Generic;
using FluentMigrator.Builders.Create.Table;
using Nop.Core;

namespace Nop.Data.Migrations.Builders
{
    public abstract class BaseEntityBuilder<TEntity> : IEntityBuilder where TEntity : BaseEntity
    {
        public string TableName => typeof(TEntity).Name;

        public abstract void MapEntity(CreateTableExpressionBuilder table);
    }
}