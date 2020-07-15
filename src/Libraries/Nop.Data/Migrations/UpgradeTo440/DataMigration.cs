using FluentMigrator;

namespace Nop.Data.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Data)]
    [SkipMigrationOnInstall]
    public class DataMigration : MigrationBase
    {      
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
