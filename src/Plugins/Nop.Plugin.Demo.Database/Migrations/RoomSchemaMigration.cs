using Nop.Data.Migrations;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/12 12:00:00:2345673", "dbo.Room base schema")]
    public class RoomSchemaMigration : FluentMigrator.Migration
    {
        private readonly IMigrationManager _migrationManager;

        public RoomSchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        // <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<Room>(Create);
        }

        public override void Down()
        {
            Delete.Table("Room");
        }
    }
}