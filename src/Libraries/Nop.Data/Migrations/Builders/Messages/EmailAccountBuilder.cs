using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class EmailAccountBuilder : BaseEntityBuilder<EmailAccount>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(EmailAccount.DisplayName)).AsString(255).NotNullable()
                .WithColumn(nameof(EmailAccount.Email)).AsString(255).NotNullable()
                .WithColumn(nameof(EmailAccount.Host)).AsString(255).NotNullable()
                .WithColumn(nameof(EmailAccount.Username)).AsString(255).NotNullable()
                .WithColumn(nameof(EmailAccount.Password)).AsString(255).NotNullable();
        }

        #endregion
    }
}