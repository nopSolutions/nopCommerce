using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the migration context with a null implementation of a processor that does not do any work
    /// </summary>
    public class NullMigrationContext : IMigrationContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ICollection<IMigrationExpression> Expressions { get; set; } = new List<IMigrationExpression>();

        public IQuerySchema QuerySchema { get; set; }
#pragma warning disable 612
        public IAssemblyCollection MigrationAssemblies { get; set; }
        public object ApplicationContext { get; set; }
#pragma warning restore 612
        public string Connection { get; set; }
    }
}