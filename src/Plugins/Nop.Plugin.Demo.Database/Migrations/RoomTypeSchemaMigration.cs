using Nop.Data.Migrations;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/12 12:00:00:3456783", "dbo.RoomType base schema")]
    public class RoomTypeSchemaMigration : FluentMigrator.Migration
    {
        private readonly IMigrationManager _migrationManager;

        public RoomTypeSchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        // <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<RoomType>(Create);
        }

        public override void Down()
        {
            Delete.Table("RoomType");
        }
    }
}