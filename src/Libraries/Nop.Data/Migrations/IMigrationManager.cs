using System;
using System.Reflection;
using FluentMigrator.Builders.Create;
using FluentMigrator.Expressions;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents a migration manager
    /// </summary>
    public interface IMigrationManager
    {
        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="isUpdateProcess">Indicates whether the upgrade or installation process is ongoing. True - if an upgrade process</param>
        void ApplyUpMigrations(Assembly assembly = null, bool isUpdateProcess = false);

        /// <summary>
        /// Executes an Down migration
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        void ApplyDownMigrations(Assembly assembly = null);

        /// <summary>
        /// Retrieves expressions into ICreateExpressionRoot
        /// </summary>
        /// <param name="expressionRoot">The root expression for a CREATE operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        void BuildTable<TEntity>(ICreateExpressionRoot expressionRoot);

        /// <summary>
        /// Gets create table expression for entity type
        /// </summary>
        /// <param name="type">Entity type</param>
        /// <returns>Expression to create a table</returns>
        CreateTableExpression GetCreateTableExpression(Type type);
    }
}