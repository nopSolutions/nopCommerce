using System.Collections.Concurrent;
using System.Data;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Nop.Data.Mapping;

/// <summary>
/// Provides an access to entity mapping information
/// </summary>
public static class NopMappingSchema
{
    #region Fields

    private static ConcurrentDictionary<Type, NopEntityDescriptor> EntityDescriptors { get; } = new();

    #endregion

    /// <summary>
    /// Returns mapped entity descriptor
    /// </summary>
    /// <param name="entityType">Type of entity</param>
    /// <returns>Mapped entity descriptor</returns>
    public static NopEntityDescriptor GetEntityDescriptor(Type entityType)
    {
        if (!typeof(BaseEntity).IsAssignableFrom(entityType))
            return null;

        return EntityDescriptors.GetOrAdd(entityType, t =>
        {
            var tableName = NameCompatibilityManager.GetTableName(t);
            var expression = new CreateTableExpression { TableName = tableName };
            var builder = new CreateTableExpressionBuilder(expression, new NullMigrationContext());
            builder.RetrieveTableExpressions(t);

            return new NopEntityDescriptor
            {
                EntityName = tableName,
                SchemaName = builder.Expression.SchemaName,
                Fields = builder.Expression.Columns.Select(column => new NopEntityFieldDescriptor
                {
                    Name = column.Name,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsNullable = column.IsNullable,
                    Size = column.Size,
                    Precision = column.Precision,
                    IsIdentity = column.IsIdentity,
                    Type = getColumnType(column)
                }).ToList()
            };

            static DbType getColumnType(ColumnDefinition column)
            {
                if (column.Type != null)
                    return column.Type.Value;

                if (!string.IsNullOrEmpty(column.CustomType))
                {
                    if(column.CustomType.StartsWith("datetime(", StringComparison.OrdinalIgnoreCase))
                        return DbType.DateTime;

                    if (column.CustomType.StartsWith("datetime2(", StringComparison.OrdinalIgnoreCase))
                        return DbType.DateTime2;
                }

                return DbType.String;
            }   
        });
    }

    /// <summary>
    /// Get or create mapping schema with specified configuration name
    /// </summary>
    public static MappingSchema GetMappingSchema(string configurationName, IDataProvider mappings)
    {
        if (Singleton<MappingSchema>.Instance is null)
        {
            Singleton<MappingSchema>.Instance = new MappingSchema(configurationName, mappings.MappingSchema);
            Singleton<MappingSchema>.Instance.AddMetadataReader(new FluentMigratorMetadataReader());
        }

        return Singleton<MappingSchema>.Instance;
    }
}
