using FluentMigrator;
using Nop.Data.Migrations;

namespace geniussoftware.plugin.widgeds.productreview.Migrations
{
    [NopMigration("", "geniussoftware.plugin.widgeds.productreview schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
        }
    }
}
