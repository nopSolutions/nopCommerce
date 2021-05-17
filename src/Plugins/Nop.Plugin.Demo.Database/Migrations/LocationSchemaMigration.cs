using Nop.Data.Migrations;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/12 12:00:00:1234563", "dbo.Location base schema")]
    public class LocationSchemaMigration : FluentMigrator.Migration
    {
        private readonly IMigrationManager _migrationManager;

        public LocationSchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        // <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<Location>(Create);
        }

        public override void Down()
        {
            Delete.Table("Location");
        }
    }
}