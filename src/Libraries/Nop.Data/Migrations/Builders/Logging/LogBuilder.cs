using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class LogBuilder : BaseEntityBuilder<Log>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Log.ShortMessage)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Log.IpAddress)).AsString(200).Nullable()
                .WithColumn(nameof(Log.CustomerId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<Customer>();
        }

        #endregion
    }
}