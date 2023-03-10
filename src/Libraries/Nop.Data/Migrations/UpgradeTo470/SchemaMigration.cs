using FluentMigrator;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopSchemaMigration("2023-03-07 00:00:00", "SchemaMigration for 4.70.0")]
    public class SchemaMigration : ForwardOnlyMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //#6167
            var messageTemplateTableName = nameof(MessageTemplate);
            var allowDirectReplyColumnName = nameof(MessageTemplate.AllowDirectReply);

            //add column
            if (!Schema.Table(messageTemplateTableName).Column(allowDirectReplyColumnName).Exists())
                Alter.Table(messageTemplateTableName)
                    .AddColumn(allowDirectReplyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
        }
    }
}
