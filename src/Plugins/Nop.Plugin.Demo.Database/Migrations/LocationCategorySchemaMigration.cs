using Nop.Data.Migrations;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/12 12:00:00:0123453", "dbo.LocationCategory base schema")]
    public class LocationCategorySchemaMigration : FluentMigrator.Migration
    {
        private readonly IMigrationManager _migrationManager;

        public LocationCategorySchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        // <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Alter.Table("LocationCategory").AddColumn("Description").AsString(400);
        }

        public override void Down()
        {
            Delete.Column("IsActive").FromTable("LocationCategory").InSchema("dbo");
        }
    }
}