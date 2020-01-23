using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ExternalAuthenticationRecordBuilder : BaseEntityBuilder<ExternalAuthenticationRecord>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ExternalAuthenticationRecord.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>();
        }

        #endregion
    }
}