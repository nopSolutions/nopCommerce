using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PrivateMessageBuilder : BaseEntityBuilder<PrivateMessage>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PrivateMessage.Subject)).AsString(450).NotNullable()
                .WithColumn(nameof(PrivateMessage.Text)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(PrivateMessage.FromCustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                    .OnDelete(Rule.None)
                .WithColumn(nameof(PrivateMessage.ToCustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                    .OnDelete(Rule.None);
        }

        #endregion
    }
}