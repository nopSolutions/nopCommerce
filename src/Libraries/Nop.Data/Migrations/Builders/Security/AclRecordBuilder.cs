using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class AclRecordBuilder : BaseEntityBuilder<AclRecord>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AclRecord.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(AclRecord.CustomerRoleId))
                    .AsInt32()
                    .ForeignKey<CustomerRole>();
        }

        #endregion
    }
}