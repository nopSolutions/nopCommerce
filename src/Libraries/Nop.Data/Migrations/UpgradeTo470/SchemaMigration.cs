using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopMigration("2023-03-07 00:00:00", "SchemaMigration for 4.70.0", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //#6167
            var messageTemplateTableName = NameCompatibilityManager.GetTableName(typeof(MessageTemplate));
            var allowDirectReplyColumnName = nameof(MessageTemplate.AllowDirectReply);

            //add column
            if (!Schema.Table(messageTemplateTableName).Column(allowDirectReplyColumnName).Exists())
            {
                Alter.Table(messageTemplateTableName)
                    .AddColumn(allowDirectReplyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
