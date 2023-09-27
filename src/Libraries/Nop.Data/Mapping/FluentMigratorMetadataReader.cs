using System.Collections.Concurrent;
using System.Reflection;
using FluentMigrator.Expressions;
using LinqToDB.Mapping;
using LinqToDB.Metadata;
using Nop.Core;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// LINQ To DB metadata reader for schema created by FluentMigrator
    /// </summary>
    public partial class FluentMigratorMetadataReader : IMetadataReader
    {
        #region Fields

        protected readonly IMappingEntityAccessor _mappingEntityAccessor;

        #endregion

        #region Ctor

        public FluentMigratorMetadataReader(IMappingEntityAccessor mappingEntityAccessor)
        {
            _mappingEntityAccessor = mappingEntityAccessor;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets attributes of specified type, associated with specified type member
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="type">Attributes owner type</param>
        /// <param name="memberInfo">Attributes owner member</param>
        /// <returns>Attribute of specified type</returns>
        protected T GetAttribute<T>(Type type, MemberInfo memberInfo) where T : Attribute
        {
            var attribute = Types.GetOrAdd((type, memberInfo), _ =>
            {
                var entityDescriptor = _mappingEntityAccessor.GetEntityDescriptor(type);

                if (typeof(T) == typeof(TableAttribute))
                    return new TableAttribute(entityDescriptor.EntityName) { Schema = entityDescriptor.SchemaName };

                if (typeof(T) != typeof(ColumnAttribute))
                    return null;

                var entityField = entityDescriptor.Fields.SingleOrDefault(cd => cd.Name.Equals(NameCompatibilityManager.GetColumnName(type, memberInfo.Name), StringComparison.OrdinalIgnoreCase));

                if (entityField is null)
                    return null;

                if (!(memberInfo as PropertyInfo)?.CanWrite ?? false)
                    return null;

                var columnSystemType = (memberInfo as PropertyInfo)?.PropertyType ?? typeof(string);

                var mappingSchema = _mappingEntityAccessor.GetMappingSchema();

                return new ColumnAttribute
                {
                    Name = entityField.Name,
                    IsPrimaryKey = entityField.IsPrimaryKey,
                    IsColumn = true,
                    CanBeNull = entityField.IsNullable ?? false,
                    Length = entityField.Size ?? 0,
                    Precision = entityField.Precision ?? 0,
                    IsIdentity = entityField.IsIdentity,
                    DataType = mappingSchema.GetDataType(columnSystemType).Type.DataType
                };
            });

            return (T)attribute;
        }

        /// <summary>
        /// Gets attributes of specified type, associated with specified type
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="type">Attributes owner type</param>
        /// <param name="attributeType">Attribute type</param>
        /// <param name="memberInfo">Attributes owner member</param>
        /// <returns>Attributes of specified type</returns>
        protected T[] GetAttributes<T>(Type type, Type attributeType, MemberInfo memberInfo = null)
            where T : Attribute
        {
            if (type.IsSubclassOf(typeof(BaseEntity)) && typeof(T) == attributeType && GetAttribute<T>(type, memberInfo) is T attr)
            {
                return new[] { attr };
            }

            return Array.Empty<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets attributes of specified type, associated with specified type.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="type">Attributes owner type.</param>
        /// <param name="inherit">If <c>true</c> - include inherited attributes.</param>
        /// <returns>Attributes of specified type.</returns>
        public virtual T[] GetAttributes<T>(Type type, bool inherit = true) where T : Attribute
        {
            return GetAttributes<T>(type, typeof(TableAttribute));
        }

        /// <summary>
        /// Gets attributes of specified type, associated with specified type member.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="type">Member's owner type.</param>
        /// <param name="memberInfo">Attributes owner member.</param>
        /// <param name="inherit">If <c>true</c> - include inherited attributes.</param>
        /// <returns>Attributes of specified type.</returns>
        public virtual T[] GetAttributes<T>(Type type, MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            return GetAttributes<T>(type, typeof(ColumnAttribute), memberInfo);
        }

        /// <summary>
        /// Gets the dynamic columns defined on given type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>All dynamic columns defined on given type</returns>
        public MemberInfo[] GetDynamicColumns(Type type)
        {
            return Array.Empty<MemberInfo>();
        }

        #endregion

        #region Properties

        protected static ConcurrentDictionary<(Type, MemberInfo), Attribute> Types { get; } = new ConcurrentDictionary<(Type, MemberInfo), Attribute>();
        protected static ConcurrentDictionary<Type, CreateTableExpression> Expressions { get; } = new ConcurrentDictionary<Type, CreateTableExpression>();

        #endregion
    }
}
