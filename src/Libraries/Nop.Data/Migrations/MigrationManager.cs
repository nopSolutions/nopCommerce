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

        private IEnumerable<IMigration> GetMigrations(Assembly assembly, params string[] tags)
        {
            return _filteringMigrationSource.GetMigrations(
                t => (assembly == null || t.Assembly == assembly) &&
                    (tags == null || _migrationRunnerConventions.HasRequestedTags(t, tags, true)));
        }

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
                .GetProperties()
                .Where(p =>
                    !expression.Columns.Any(x => x.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)) &&
                    !nameof(BaseEntity.Id).Equals(p.Name, StringComparison.OrdinalIgnoreCase));

            if (propertiesToAutoMap is null || propertiesToAutoMap.Count() == 0)
                return;

            foreach (var prop in propertiesToAutoMap)
            {
                builder.WithSelfType(prop.Name, prop.PropertyType);
            }
        }

        public virtual void BuildTable<TEntity>(ICreateExpressionRoot expressionRoot, string tableName = null)
        {
            var tblName = string.IsNullOrEmpty(tableName) ? typeof(TEntity).Name : tableName;

            var builder = expressionRoot.Table(tblName) as CreateTableExpressionBuilder;

            RetrieveTableExpressions(typeof(TEntity), builder);
        }

        public virtual CreateTableExpression GetCreateTableExpression(Type type)
        {
            var expression = new CreateTableExpression { TableName = type.Name };
            var builder = new CreateTableExpressionBuilder(expression, CreateNullMigrationContext());

            RetrieveTableExpressions(type, builder);

            return builder.Expression;
        }

        public IEnumerable<CreateTableExpression> LoadSchemeExpressions()
        {
            var localContext = CreateNullMigrationContext();
            var migrations = GetMigrations(null, NopMigrationTags.Schema);

            foreach (var migration in migrations)
            {
                migration.GetUpExpressions(localContext);
            }

            return localContext.Expressions.Where(x => x is CreateTableExpression).Cast<CreateTableExpression>();
        }

        #endregion
    }
}