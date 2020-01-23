using System.Reflection;

namespace Nop.Data.Migrations
{
    public interface IMigrationManager
    {
        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="tags">Migration tags for filtering</param>
        void ApplyUpMigrations(Assembly assembly, params string[] tags);
        
        /// <summary>
        /// Executes an Down migration
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="tags">Migration tags for filtering</param>
        void ApplyDownMigrations(Assembly assembly, params string[] tags);
    }
}