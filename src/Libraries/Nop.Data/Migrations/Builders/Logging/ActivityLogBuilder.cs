using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ActivityLogBuilder : BaseEntityBuilder<ActivityLog>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ActivityLog.Comment)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ActivityLog.IpAddress)).AsString(200).Nullable()
                .WithColumn(nameof(ActivityLog.EntityName)).AsString(400).Nullable()
                .WithColumn(nameof(ActivityLog.ActivityLogTypeId))
                    .AsInt32()
                    .ForeignKey<ActivityLogType>()
                .WithColumn(nameof(ActivityLog.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>();

            #endregion
        }
    }
}