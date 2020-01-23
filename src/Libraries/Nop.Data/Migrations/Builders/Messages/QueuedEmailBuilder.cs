using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class QueuedEmailBuilder : BaseEntityBuilder<QueuedEmail>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(QueuedEmail.From)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.FromName)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.To)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.ToName)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.ReplyTo)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.ReplyToName)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.CC)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.Bcc)).AsString(500).NotNullable()
                .WithColumn(nameof(QueuedEmail.Subject)).AsString(1000).NotNullable()
                .WithColumn(nameof(QueuedEmail.EmailAccountId))
                    .AsInt32()
                    .ForeignKey<EmailAccount>();
        }

        #endregion
    }
}