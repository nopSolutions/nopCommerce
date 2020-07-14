using FluentMigrator;

namespace Nop.Data.Migrations.UpgradeTo440
{
    [NopMigration(MigrationType.Data)]
    [SkipMigrationOnInstall]
    public class DataMigration : MigrationBase
    {      
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            #if DEBUG
            throw new PreventFixMigrationException();
            #endif
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
