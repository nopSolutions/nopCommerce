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
        /// <param name="assembly">Assembly to find the migration</param>
        /// <param name="isUpdateProcess">Indicates whether the upgrade or installation process is ongoing. True - if an upgrade process</param>
        void ApplyUpMigrations(Assembly assembly, bool isUpdateProcess = false);

        /// <summary>
        /// Executes an Down migration
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        void ApplyDownMigrations(Assembly assembly);
    }
}