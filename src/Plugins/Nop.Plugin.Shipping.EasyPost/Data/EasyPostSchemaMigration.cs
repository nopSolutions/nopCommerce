using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.EasyPost.Domain.Batch;

namespace Nop.Plugin.Shipping.EasyPost.Data
{
    [NopMigration("2021/12/15 12:00:00", "Shipping.EasyPost base schema", MigrationProcessType.Installation)]
    public class EasyPostSchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<EasyPostBatch>();
        }

        #endregion
    }
}