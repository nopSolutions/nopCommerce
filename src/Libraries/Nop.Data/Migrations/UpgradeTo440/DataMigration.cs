using FluentMigrator;
using Nop.Core;

namespace Nop.Data.Migrations.UpgradeTo440
{
    [NopMigration(NopVersion.FULL_VERSION, MigrationType.Data, "version 4.40. Update data on the database")]
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
