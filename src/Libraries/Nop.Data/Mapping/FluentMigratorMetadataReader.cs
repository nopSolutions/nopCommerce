using System;
using System.Collections.Concurrent;
using LinqToDB.Mapping;
using System.Linq;
using System.Reflection;
using FluentMigrator.Expressions;
using LinqToDB.Metadata;
using Nop.Data.Migrations;
using Nop.Core.Infrastructure;
using LinqToDB.SqlQuery;
using Nop.Core;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// LINQ To DB metadata reader for schema created by FluentMigrator
    /// </summary>
    public class FluentMigratorMetadataReader : IMetadataReader
    {
        private static readonly ConcurrentDictionary<(Type, MemberInfo), Attribute> _types = new ConcurrentDictionary<(Type, MemberInfo), Attribute>();
        private static readonly ConcurrentDictionary<Type, CreateTableExpression> _expressions = new ConcurrentDictionary<Type, CreateTableExpression>();

        private readonly IMigrationManager _migrationManager;

        public FluentMigratorMetadataReader()
        {
            _migrationManager = EngineContext.Current.Resolve<IMigrationManager>();
        }

        private T GetAttribute<T>(Type type, MemberInfo memberInfo) where T : Attribute
        {
            var attribute = _types.GetOrAdd((type, memberInfo), t =>
            {
                var tableExpr = _expressions.GetOrAdd(type, entityType => _migrationManager.GetCreateTableExpression(entityType));

                if (typeof(T) == typeof(TableAttribute))
                {
                    return new TableAttribute(tableExpr.TableName) { Schema = tableExpr.SchemaName };
                }

                if (typeof(T) == typeof(ColumnAttribute))
                {
                    var column = tableExpr.Columns.SingleOrDefault(cd => cd.Name.Equals(memberInfo.Name, StringComparison.OrdinalIgnoreCase));

                    if (column is null)
                        return null;

                    return new ColumnAttribute
                    {
                        Name = column.Name,
                        IsPrimaryKey = column.IsPrimaryKey,
                        IsColumn = true,
                        CanBeNull = column.IsNullable ?? false,
                        Length = column.Size ?? 0,
                        Precision = column.Precision ?? 0,
                        IsIdentity = column.IsIdentity,
                        DataType = new SqlDataType((memberInfo as PropertyInfo).PropertyType).DataType
                    };
                }

                return null;
            });

            return (T)attribute;
        }

        public T[] GetAttributes<T>(Type type, bool inherit = true) where T : Attribute
        {
            if (type.IsSubclassOf(typeof(BaseEntity)) && typeof(T) == typeof(TableAttribute) && GetAttribute<T>(type, null) is T attr)
            {
                return new T[] { attr };
            }

            return Array.Empty<T>();
        }

        public T[] GetAttributes<T>(Type type, MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {

            if (type.IsSubclassOf(typeof(BaseEntity)) && typeof(T) == typeof(ColumnAttribute) && GetAttribute<T>(type, memberInfo) is T attr)
            {
                return new T[] { attr };
            }

            return Array.Empty<T>();
        }

        public MemberInfo[] GetDynamicColumns(Type type)
        {
            return Array.Empty<MemberInfo>();
        }

    }
}