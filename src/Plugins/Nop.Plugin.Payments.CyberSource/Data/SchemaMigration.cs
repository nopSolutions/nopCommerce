using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Payments.CyberSource.Domain;

namespace Nop.Plugin.Payments.CyberSource.Data
{
    [NopMigration("2022-04-13 00:00:00", "Payments.CyberSource base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<CyberSourceCustomerToken>();
        }

        #endregion
    }
}