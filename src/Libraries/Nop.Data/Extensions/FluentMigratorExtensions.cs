using System;
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
        public static void WithSelfType(this CreateTableExpressionBuilder create, string name, Type propType, bool canBeNullable = false)
        {
            if (Nullable.GetUnderlyingType(propType) is Type uType)
            {
                propType = uType;
                canBeNullable = true;
            }

            switch (propType)
            {
                case Type intType when intType == typeof(int):
                    create
                        .WithColumn(name)
                        .AsInt32();
                    break;
                case Type strType when strType == typeof(string):
                    create
                        .WithColumn(name)
                        .AsString(int.MaxValue);
                    break;
                case Type boolType when boolType == typeof(bool):
                    create
                        .WithColumn(name)
                        .AsBoolean();
                    break;
                case Type decimalType when decimalType == typeof(decimal):
                    create
                        .WithColumn(name)
                        .AsDecimal(18, 4);
                    break;
                case Type dateType when dateType == typeof(DateTime):
                    create
                        .WithColumn(name)
                        .AsDateTime();
                    break;
                case Type byteArrType when byteArrType == typeof(byte[]):
                    create
                        .WithColumn(name)
                        .AsBinary(int.MaxValue);
                    break;
                case Type byteArrType when byteArrType == typeof(Guid):
                    create
                        .WithColumn(name)
                        .AsGuid();
                    break;
                default:
                    return;
            }

            if(canBeNullable)
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