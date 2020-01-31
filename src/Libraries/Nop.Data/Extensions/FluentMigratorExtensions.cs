using System;
using System.Collections.Generic;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Runner;
using Nop.Core;

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

        /// <summary>
        /// Defines the column specifications by default
        /// </summary>
        /// <param name="create">An expression builder for a FluentMigrator.Expressions.CreateTableExpression</param>
        /// <param name="name">Specified column name</param>
        /// <param name="propType">Type of an entity property</param>
        /// <param name="canBeNullable">The value indicating whether this column is nullable</param>
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

        /// <summary>
        /// Specifies a foreign key
        /// </summary>
        /// <param name="column">The foreign key column</param>
        /// <param name="primaryTableName">The primary table name</param>
        /// <param name="primaryColumnName">The primary tables column name</param>
        /// <typeparam name="TPrimary"></typeparam>
        /// <returns>Set column options or create a new column or set a foreign key cascade rule</returns>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey<TPrimary>(this ICreateTableColumnOptionOrWithColumnSyntax column, string primaryTableName = null, string primaryColumnName = null) where TPrimary : BaseEntity
        {
            if (string.IsNullOrEmpty(primaryTableName))
                primaryTableName = typeof(TPrimary).Name;

            if (string.IsNullOrEmpty(primaryColumnName))
                primaryColumnName = nameof(BaseEntity.Id);

            return column.Indexed().ForeignKey(primaryTableName, primaryColumnName);
        }

        /// <summary>
        /// Configure the database servers support
        /// </summary>
        /// <param name="builder">Configuring migration runner services</param>
        /// <returns>The migration runner builder</returns>
        public static IMigrationRunnerBuilder SetServers(this IMigrationRunnerBuilder builder)
        {
            return builder
                .AddSqlServer()
                .AddMySql5();
        }
    }
}