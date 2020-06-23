using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the migration manager
    /// </summary>
    public partial class MigrationManager : IMigrationManager
    {
        #region Fields

        private readonly Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>> _typeMapping;
        private readonly IFilteringMigrationSource _filteringMigrationSource;
        private readonly IMigrationRunner _migrationRunner;
        private readonly IMigrationRunnerConventions _migrationRunnerConventions;
        private readonly IMigrationContext _migrationContext;
        private readonly ITypeFinder _typeFinder;
        private readonly IVersionLoader _versionLoader;

        #endregion

        #region Ctor

        public MigrationManager(
            IFilteringMigrationSource filteringMigrationSource,
            IMigrationRunner migrationRunner,
            IMigrationRunnerConventions migrationRunnerConventions,
            IMigrationContext migrationContext,
            ITypeFinder typeFinder,
            IVersionLoader versionLoader)
        {
            _typeMapping = new Dictionary<Type, Action<ICreateTableColumnAsTypeSyntax>>()
            {
                [typeof(int)] = c => c.AsInt32(),
                [typeof(string)] = c => c.AsString(int.MaxValue).Nullable(),
                [typeof(bool)] = c => c.AsBoolean(),
                [typeof(decimal)] = c => c.AsDecimal(18, 4),
                [typeof(DateTime)] = c => c.AsDateTime2(),
                [typeof(byte[])] = c => c.AsBinary(int.MaxValue),
                [typeof(Guid)] = c => c.AsGuid()
            };

            _filteringMigrationSource = filteringMigrationSource;
            _migrationRunner = migrationRunner;
            _migrationRunnerConventions = migrationRunnerConventions;
            _migrationContext = migrationContext;
            _typeFinder = typeFinder;
            _versionLoader = versionLoader;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Defines the column specifications by default
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="create">An expression builder for a FluentMigrator.Expressions.CreateTableExpression</param>
        /// <param name="propertyInfo">Property info</param>
        /// <param name="canBeNullable">The value indicating whether this column is nullable</param>
        protected virtual void DefineByOwnType(Type type, CreateTableExpressionBuilder create, PropertyInfo propertyInfo, bool canBeNullable = false)
        {
            var propType = propertyInfo.PropertyType;

            if (Nullable.GetUnderlyingType(propType) is Type uType)
            {
                propType = uType;
                canBeNullable = true;
            }

            if (!_typeMapping.ContainsKey(propType))
                return;
            
            if (type == typeof(string) || propType.FindInterfaces((t, o) => t.FullName?.Equals(o.ToString(), StringComparison.InvariantCultureIgnoreCase) ?? false, "System.Collections.IEnumerable").Any())
                canBeNullable = true;

            var column = create.WithColumn(NameCompatibilityManager.GetColumnName(type, propertyInfo.Name));
            _typeMapping[propType](column);

            if (canBeNullable)
                create.Nullable();
        }

        /// <summary>
        /// Retrieves expressions for building an entity table
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="builder">An expression builder for a FluentMigrator.Expressions.CreateTableExpression</param>
        protected void RetrieveTableExpressions(Type type, CreateTableExpressionBuilder builder)
        {
            var tp = _typeFinder
                .FindClassesOfType(typeof(IEntityBuilder))
                .FirstOrDefault(t => t.BaseType?.GetGenericArguments().Contains(type) ?? false);

            if (tp != null)
                (EngineContext.Current.ResolveUnregistered(tp) as IEntityBuilder)?.MapEntity(builder);

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
                .GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.SetProperty)
                .Where(p =>
                    !expression.Columns.Any(x => x.Name.Equals(NameCompatibilityManager.GetColumnName(type, p.Name), StringComparison.OrdinalIgnoreCase)));

            foreach (var prop in propertiesToAutoMap)
            {
                DefineByOwnType(type, builder, prop);
            }
        }

        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        private IEnumerable<IMigrationInfo> GetMigrations(Assembly assembly)
        {
            var migrations = _filteringMigrationSource.GetMigrations(t => assembly == null || t.Assembly == assembly) ?? Enumerable.Empty<IMigration>();

            return migrations.Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m)).OrderBy(migration => migration.Version);
        }

        /// <summary>
        /// Provides migration context with a null implementation of a processor that does not do any work
        /// </summary>
        /// <returns>The context of a migration while collecting up/down expressions</returns>
        protected IMigrationContext CreateNullMigrationContext()
        {
            return new MigrationContext(new NullIfDatabaseProcessor(), _migrationContext.ServiceProvider, null, null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="isUpdateProcess">Indicates whether the upgrade or installation process is ongoing. True - if an upgrade process</param>
        public void ApplyUpMigrations(Assembly assembly = null, bool isUpdateProcess = false)
        {
            var migrations = GetMigrations(assembly);

            foreach (var migrationInfo in migrations)
            {
                if(isUpdateProcess && migrationInfo.Migration.GetType().GetCustomAttributes(typeof(SkipMigrationOnUpdateAttribute)).Any())
                    continue;

                _migrationRunner.MigrateUp(migrationInfo.Version);
            }
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        public void ApplyDownMigrations(Assembly assembly = null)
        {
            var migrations = GetMigrations(assembly).Reverse();

            foreach (var migrationInfo in migrations)
            {
                _migrationRunner.Down(migrationInfo.Migration);
                _versionLoader.DeleteVersion(migrationInfo.Version);
            }
        }

        /// <summary>
        /// Retrieves expressions into ICreateExpressionRoot
        /// </summary>
        /// <param name="expressionRoot">The root expression for a CREATE operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void BuildTable<TEntity>(ICreateExpressionRoot expressionRoot)
        {
            var type = typeof(TEntity);

            var builder = expressionRoot.Table(NameCompatibilityManager.GetTableName(type)) as CreateTableExpressionBuilder;

            RetrieveTableExpressions(type, builder);
        }

        /// <summary>
        /// Gets create table expression for entity type
        /// </summary>
        /// <param name="type">Entity type</param>
        /// <returns>Expression to create a table</returns>
        public virtual CreateTableExpression GetCreateTableExpression(Type type)
        {
            var expression = new CreateTableExpression { TableName = NameCompatibilityManager.GetTableName(type) };
            var builder = new CreateTableExpressionBuilder(expression, CreateNullMigrationContext());

            RetrieveTableExpressions(type, builder);

            return builder.Expression;
        }

        #endregion
    }
}