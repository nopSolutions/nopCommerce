using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Delete.Column;
using FluentMigrator.Builders.Schema;
using FluentMigrator.Builders.Schema.Table;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using LinqToDB.Mapping;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.Extensions;

/// <summary>
/// FluentMigrator extensions
/// </summary>
public static class FluentMigratorExtensions
{
    #region  Utils

    private const int DATE_TIME_PRECISION = 6;

    private static Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>> TypeMapping { get; } = new Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>>
    {
        [typeof(int)] = c => c.AsInt32(),
        [typeof(long)] = c => c.AsInt64(),
        [typeof(string)] = c => c.AsString(int.MaxValue).Nullable(),
        [typeof(bool)] = c => c.AsBoolean(),
        [typeof(decimal)] = c => c.AsDecimal(18, 4),
        [typeof(DateTime)] = c => c.AsNopDateTime2(),
        [typeof(byte[])] = c => c.AsBinary(int.MaxValue),
        [typeof(Guid)] = c => c.AsGuid()
    };

    private static void DefineByOwnType(string columnName, Type propType, CreateTableExpressionBuilder create, bool canBeNullable = false)
    {
        if (string.IsNullOrEmpty(columnName))
            throw new ArgumentException("The column name cannot be empty");

        if (propType == typeof(string) || propType.FindInterfaces((t, o) => t.FullName?.Equals(o.ToString(), StringComparison.InvariantCultureIgnoreCase) ?? false, "System.Collections.IEnumerable").Length > 0)
            canBeNullable = true;

        var column = create.WithColumn(columnName);

        TypeMapping[propType](column);

        if (propType == typeof(DateTime))
            create.CurrentColumn.Precision = DATE_TIME_PRECISION;

        if (canBeNullable)
            create.Nullable();
    }

    #endregion

    /// <summary>
    /// Adds database support for migrations
    /// </summary>
    /// <param name="builder">The builder to add the database engine(s) to</param>
    /// <returns>The migration runner builder</returns>
    public static IMigrationRunnerBuilder AddNopDbEngines(this IMigrationRunnerBuilder builder)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return builder.AddSqlServer().AddMySql5().AddPostgres92();

        var dataSettings = DataSettingsManager.LoadSettings();

        return dataSettings.DataProvider switch
        {
            DataProviderType.MySql => builder.AddMySql5(),
            DataProviderType.SqlServer => builder.AddSqlServer(),
            DataProviderType.PostgreSQL => builder.AddPostgres92(),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Defines the column type as date that is combined with a time of day and a specified precision
    /// </summary>
    public static ICreateTableColumnOptionOrWithColumnSyntax AsNopDateTime2(this ICreateTableColumnAsTypeSyntax syntax)
    {
        var dataSettings = DataSettingsManager.LoadSettings();

        return dataSettings.DataProvider switch
        {
            DataProviderType.MySql => syntax.AsCustom($"datetime({DATE_TIME_PRECISION})"),
            DataProviderType.SqlServer => syntax.AsCustom($"datetime2({DATE_TIME_PRECISION})"),
            _ => syntax.AsDateTime2()
        };
    }

    /// <summary>
    /// Specifies a foreign key
    /// </summary>
    /// <param name="column">The foreign key column</param>
    /// <param name="primaryTableName">The primary table name</param>
    /// <param name="primaryColumnName">The primary tables column name</param>
    /// <param name="onDelete">Behavior for DELETEs</param>
    /// <typeparam name="TPrimary"></typeparam>
    /// <returns>Set column options or create a new column or set a foreign key cascade rule</returns>
    public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey<TPrimary>(this ICreateTableColumnOptionOrWithColumnSyntax column, string primaryTableName = null, string primaryColumnName = null, Rule onDelete = Rule.Cascade) where TPrimary : BaseEntity
    {
        if (string.IsNullOrEmpty(primaryTableName))
            primaryTableName = NameCompatibilityManager.GetTableName(typeof(TPrimary));

        if (string.IsNullOrEmpty(primaryColumnName))
            primaryColumnName = nameof(BaseEntity.Id);

        return column.Indexed().ForeignKey(primaryTableName, primaryColumnName).OnDelete(onDelete);
    }

    /// <summary>
    /// Specifies a foreign key
    /// </summary>
    /// <param name="column">The foreign key column</param>
    /// <param name="primaryTableName">The primary table name</param>
    /// <param name="primaryColumnName">The primary tables column name</param>
    /// <param name="onDelete">Behavior for DELETEs</param>
    /// <typeparam name="TPrimary"></typeparam>
    /// <returns>Alter/add a column with an optional foreign key</returns>
    public static IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ForeignKey<TPrimary>(this IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax column, string primaryTableName = null, string primaryColumnName = null, Rule onDelete = Rule.Cascade) where TPrimary : BaseEntity
    {
        if (string.IsNullOrEmpty(primaryTableName))
            primaryTableName = NameCompatibilityManager.GetTableName(typeof(TPrimary));

        if (string.IsNullOrEmpty(primaryColumnName))
            primaryColumnName = nameof(BaseEntity.Id);

        return column.Indexed().ForeignKey(primaryTableName, primaryColumnName).OnDelete(onDelete);
    }

    /// <summary>
    /// Retrieves expressions into ICreateExpressionRoot
    /// </summary>
    /// <param name="expressionRoot">The root expression for a CREATE operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public static void TableFor<TEntity>(this ICreateExpressionRoot expressionRoot) where TEntity : BaseEntity
    {
        var type = typeof(TEntity);
        var builder = expressionRoot.Table(NameCompatibilityManager.GetTableName(type)) as CreateTableExpressionBuilder;
        builder.RetrieveTableExpressions(type);
    }

    /// <summary>
    /// Targets the entity's mapped table for a DELETE operation.
    /// </summary>
    /// <param name="expressionRoot">The root expression for a DELETE operation</param>
    /// <typeparam name="TEntity">The entity type mapped to the database table</typeparam>
    public static void TableFor<TEntity>(this IDeleteExpressionRoot expressionRoot) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        expressionRoot.Table(tableName);
    }

    /// <summary>
    /// Targets the entity's mapped table for an ALTER TABLE operation.
    /// </summary>
    /// <param name="expressionRoot">The root expression for an ALTER operation</param>
    /// <typeparam name="TEntity">The entity type mapped to the database table</typeparam>
    /// <returns>
    /// A fluent syntax interface allowing further ALTER TABLE operations 
    /// such as adding or modifying columns.
    /// </returns>
    public static IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax TableFor<TEntity>(this IAlterExpressionRoot expressionRoot) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        return expressionRoot.Table(tableName);
    }

    /// <summary>
    /// Targets the entity's mapped table for schema-related operations.
    /// </summary>
    /// <param name="expressionRoot">The root expression for schema inspection</param>
    /// <typeparam name="TEntity">The entity type mapped to the database table</typeparam>
    /// <returns>
    /// A fluent syntax interface for performing schema operations 
    /// such as checking table or column existence.
    /// </returns>
    public static ISchemaTableSyntax TableFor<TEntity>(this ISchemaExpressionRoot expressionRoot) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        return expressionRoot.Table(tableName);
    }

    /// <summary>
    /// Determines whether the database table mapped to the specified entity exists,
    /// resolving the table name using <see cref="NameCompatibilityManager"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type mapped to the database table.</typeparam>
    /// <param name="expressionRoot">The root schema expression.</param>
    /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
    public static bool TableExist<TEntity>(this ISchemaExpressionRoot expressionRoot) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        return expressionRoot.Table(tableName).Exists();
    }

    /// <summary>
    /// Checks whether a mapped column exists in the database table for the specified entity.
    /// Resolves both the table name and column name using <see cref="NameCompatibilityManager"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type mapped to the database table.</typeparam>
    /// <param name="expressionRoot">The root schema expression.</param>
    /// <param name="selector">An expression selecting the entity property to check.</param>
    /// <returns><c>true</c> if the column exists; otherwise, <c>false</c>.</returns>
    public static bool ColumnExist<TEntity>(
    this ISchemaExpressionRoot expressionRoot, Expression<Func<TEntity, object>> selector) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        var property = ((MemberExpression)selector.Body)?.Member?.Name;
        var columnName = NameCompatibilityManager.GetColumnName(typeof(TEntity), property!);
        return expressionRoot.Table(tableName).Column(columnName).Exists();
    }

    /// <summary>
    /// Checks whether a mapped column exists in the database table for the specified entity.
    /// Resolves both the table name and column name using <see cref="NameCompatibilityManager"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type mapped to the database table.</typeparam>
    /// <param name="expressionRoot">The root schema expression.</param>
    /// <param name="columnName">The column name</param>
    /// <returns><c>true</c> if the column exists; otherwise, <c>false</c>.</returns>
    public static bool ColumnExist<TEntity>(
    this ISchemaExpressionRoot expressionRoot, string columnName) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        return expressionRoot.Table(tableName).Column(columnName).Exists();
    }

    /// <summary>
    /// Adds a new column to the entity's mapped table for ALTER TABLE operations,
    /// resolving the column name via <see cref="NameCompatibilityManager"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type mapped to the database table</typeparam>
    /// <param name="tableSchema">The alter table expression</param>
    /// <param name="selector">An expression selecting the entity property</param>
    /// <returns>
    /// A fluent syntax interface allowing further ALTER TABLE operations 
    /// on the specified column.
    /// </returns>
    public static IAlterTableColumnAsTypeSyntax AddColumnFor<TEntity>(
    this IAlterExpressionRoot expressionRoot, Expression<Func<TEntity, object>> selector) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        var property = ((MemberExpression)selector.Body)?.Member?.Name;
        var columnName = NameCompatibilityManager.GetColumnName(typeof(TEntity), property!);
        return expressionRoot.Table(tableName).AddColumn(columnName);
    }

    /// <summary>
    /// Targets the mapped table of the specified entity for a DELETE COLUMN operation,
    /// resolving the table name via <see cref="NameCompatibilityManager"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type mapped to the database table</typeparam>
    /// <param name="expressionRoot">The delete column expression from table syntax</param>
    /// <returns>
    /// A fluent syntax interface allowing the deletion of columns from the specified table.
    /// </returns>
    public static IInSchemaSyntax FromTable<TEntity>(
    this IDeleteColumnFromTableSyntax expressionRoot) where TEntity : BaseEntity
    {
        var tableName = NameCompatibilityManager.GetTableName(typeof(TEntity));
        return expressionRoot.FromTable(tableName);
    }

    /// <summary>
    /// Retrieves expressions for building an entity table
    /// </summary>
    /// <param name="builder">An expression builder for a FluentMigrator.Expressions.CreateTableExpression</param>
    /// <param name="type">Type of entity</param>
    public static void RetrieveTableExpressions(this CreateTableExpressionBuilder builder, Type type)
    {
        var typeFinder = Singleton<ITypeFinder>.Instance
            .FindClassesOfType(typeof(IEntityBuilder))
            .FirstOrDefault(t => t.BaseType?.GetGenericArguments().Contains(type) ?? false);

        if (typeFinder != null)
            (EngineContext.Current.ResolveUnregistered(typeFinder) as IEntityBuilder)?.MapEntity(builder);

        var expression = builder.Expression;
        if (!expression.Columns.Any(c => c.IsPrimaryKey))
        {
            var pk = new ColumnDefinition
            {
                Name = nameof(BaseEntity.Id),
                Type = DbType.Int32,
                IsIdentity = true,
                TableName = NameCompatibilityManager.GetTableName(type),
                ModificationType = ColumnModificationType.Create,
                IsPrimaryKey = true
            };
            expression.Columns.Insert(0, pk);
            builder.CurrentColumn = pk;
        }

        var propertiesToAutoMap = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
            .Where(pi => pi.DeclaringType != typeof(BaseEntity) &&
                         pi.CanWrite &&
                         !pi.HasAttribute<NotMappedAttribute>() && !pi.HasAttribute<NotColumnAttribute>() &&
                         !expression.Columns.Any(x => x.Name.Equals(NameCompatibilityManager.GetColumnName(type, pi.Name), StringComparison.OrdinalIgnoreCase)) &&
                         TypeMapping.ContainsKey(GetTypeToMap(pi.PropertyType).propType));

        foreach (var prop in propertiesToAutoMap)
        {
            var columnName = NameCompatibilityManager.GetColumnName(type, prop.Name);
            var (propType, canBeNullable) = GetTypeToMap(prop.PropertyType);
            DefineByOwnType(columnName, propType, builder, canBeNullable);
        }
    }

    public static (Type propType, bool canBeNullable) GetTypeToMap(this Type type)
    {
        if (Nullable.GetUnderlyingType(type) is Type uType)
            return (uType, true);

        return (type, false);
    }
}