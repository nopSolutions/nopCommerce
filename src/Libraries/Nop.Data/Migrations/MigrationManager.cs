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
using Nop.Data.Extensions;
using Nop.Data.Migrations.Builders;

/// <summary>
/// Represents the migration manager
/// </summary>
namespace Nop.Data.Migrations
{
    public class MigrationManager : IMigrationManager
    {
        #region Fields

        protected IFilteringMigrationSource _filteringMigrationSource;
        protected IMigrationRunner _migrationRunner;
        protected IMigrationRunnerConventions _migrationRunnerConventions;
        private IMigrationContext _migrationContext;

        #endregion

        #region Ctor

        public MigrationManager(
            IFilteringMigrationSource filteringMigrationSource,
            IMigrationRunner migrationRunner,
            IMigrationRunnerConventions migrationRunnerConventions,
            IMigrationContext migrationContext)
        {
            _filteringMigrationSource = filteringMigrationSource;
            _migrationRunner = migrationRunner;
            _migrationRunnerConventions = migrationRunnerConventions;
            _migrationContext = migrationContext;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Retrieves expressions for building an entity table
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="builder">An expression builder for a FluentMigrator.Expressions.CreateTableExpression</param>
        protected void RetrieveTableExpressions(Type type, CreateTableExpressionBuilder builder)
        {
            var tp = new AppDomainTypeFinder()
                .FindClassesOfType(typeof(IEntityBuilder))
                .FirstOrDefault(t => t.BaseType.GetGenericArguments().Contains(type));

            if (tp != null)
            {
                ((IEntityBuilder)Activator.CreateInstance(tp)).MapEntity(builder);
            }

            var expression = builder.Expression;

            if (!expression.Columns.Any(c => c.IsPrimaryKey))
            {
                var pk = new ColumnDefinition
                {
                    Name = nameof(BaseEntity.Id),
                    Type = DbType.Int32,
                    IsIdentity = true,
                    TableName = type.Name,
                    ModificationType = ColumnModificationType.Create,
                    IsPrimaryKey = true
                };

                expression.Columns.Insert(0, pk);
                builder.CurrentColumn = pk;
            }

            var propertiesToAutoMap = type
                .GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(p =>
                    !expression.Columns.Any(x => x.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));

            if (propertiesToAutoMap is null || propertiesToAutoMap.Count() == 0)
                return;

            foreach (var prop in propertiesToAutoMap)
            {
                builder.WithSelfType(prop.Name, prop.PropertyType);
            }
        }

        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="tags">The list of tags to check against</param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        private IEnumerable<IMigration> GetMigrations(Assembly assembly, params string[] tags)
        {
            return _filteringMigrationSource.GetMigrations(
                t => (assembly == null || t.Assembly == assembly) &&
                    (tags == null || _migrationRunnerConventions.HasRequestedTags(t, tags, true)));
        }

        /// <summary>
        /// Provides migration context with a null implementation of a processor that does not do any work
        /// </summary>
        /// <returns>The context of a migration while collecting up/down expressions</returns>
        protected IMigrationContext CreateNullMigrationContext()
        {
            return new MigrationContext(
                new NullIfDatabaseProcessor(),
                _migrationContext.ServiceProvider, null, null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="tags">Migration tags for filtering</param>
        public void ApplyUpMigrations(Assembly assembly = null, params string[] tags)
        {
            var migrations = GetMigrations(assembly, tags);

            foreach (var migration in migrations)
            {
                _migrationRunner.Up(migration);
            }
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="tags">Migration tags for filtering</param>
        public void ApplyDownMigrations(Assembly assembly = null, params string[] tags)
        {
            var migrations = GetMigrations(assembly, tags);

            foreach (var migration in migrations)
            {
                _migrationRunner.Down(migration);
            }
        }

        /// <summary>
        /// Retrieves expressions into ICreateExpressionRoot
        /// </summary>
        /// <param name="expressionRoot">The root expression for a CREATE operation</param>
        /// <param name="tableName">Specified table name; pass null to use the name of a type</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void BuildTable<TEntity>(ICreateExpressionRoot expressionRoot, string tableName = null)
        {
            var tblName = string.IsNullOrEmpty(tableName) ? typeof(TEntity).Name : tableName;

            var builder = expressionRoot.Table(tblName) as CreateTableExpressionBuilder;

            RetrieveTableExpressions(typeof(TEntity), builder);
        }

        /// <summary>
        /// Gets create table expression for entity type
        /// </summary>
        /// <param name="type">Entity type</param>
        /// <returns>Expression to create a table</returns>
        public virtual CreateTableExpression GetCreateTableExpression(Type type)
        {
            var expression = new CreateTableExpression { TableName = type.Name };
            var builder = new CreateTableExpressionBuilder(expression, CreateNullMigrationContext());

            RetrieveTableExpressions(type, builder);

            return builder.Expression;
        }

        #endregion
    }
}