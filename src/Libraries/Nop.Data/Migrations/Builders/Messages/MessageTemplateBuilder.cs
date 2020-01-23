using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class MessageTemplateBuilder : BaseEntityBuilder<MessageTemplate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MessageTemplate.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(MessageTemplate.BccEmailAddresses)).AsString(200).Nullable()
                .WithColumn(nameof(MessageTemplate.Subject)).AsString(1000).Nullable()
                .WithColumn(nameof(MessageTemplate.EmailAccountId))
                    .AsInt32()
                    .ForeignKey<EmailAccount>();

            //builder.Ignore(template => template.DelayPeriod);
        }

        #endregion
    }
}