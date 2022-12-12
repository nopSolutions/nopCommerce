using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Widgets.CustomProductReviews.Migrations
{
    [NopMigration("", "Nop.Plugin.Widgets.CustomProductReviews schema", MigrationProcessType.Installation)]
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
