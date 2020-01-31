using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Data.Migrations.Builders;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// FluentMigrator extensions
    /// </summary>
    public static class FluentMigratorExtensions
    {
        private readonly static Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>> _typeMapping = new Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>>()
        {
            [typeof(int)] = (c) => c.AsInt32(),
            [typeof(string)] = (c) => c.AsString(int.MaxValue).Nullable(),
            [typeof(bool)] = (c) => c.AsBoolean(),
            [typeof(decimal)] = (c) => c.AsDecimal(18, 4),
            [typeof(DateTime)] = (c) => c.AsDateTime(),
            [typeof(byte[])] = (c) => c.AsBinary(int.MaxValue),
            [typeof(Guid)] = (c) => c.AsGuid()
        };

        public static void WithSelfType(this CreateTableExpressionBuilder create, string name, Type propType, bool canBeNullable = false)
        {
            if (Nullable.GetUnderlyingType(propType) is Type uType)
            {
                propType = uType;
                canBeNullable = true;
            }

            if (!_typeMapping.ContainsKey(propType))
                return;

            var column = create.WithColumn(name);
            _typeMapping[propType](column);

            if (canBeNullable)
                create.Nullable();
        }

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey<TPrimary>(this ICreateTableColumnOptionOrWithColumnSyntax column, string primaryTableName = null, string primaryColumnName = null) where TPrimary : BaseEntity
        {
            if (string.IsNullOrEmpty(primaryTableName))
                primaryTableName = typeof(TPrimary).Name;

            if (string.IsNullOrEmpty(primaryColumnName))
                primaryColumnName = nameof(BaseEntity.Id);

            return column.Indexed().ForeignKey(primaryTableName, primaryColumnName);
        }

        /// <summary>
        /// Configure the database server
        /// </summary>
        /// <param name="builder">Configuring migration runner services</param>
        /// <returns></returns>
        public static IMigrationRunnerBuilder SetServers(this IMigrationRunnerBuilder builder)
        {
            return builder
                .AddSqlServer()
                .AddMySql5();
        }
    }
}