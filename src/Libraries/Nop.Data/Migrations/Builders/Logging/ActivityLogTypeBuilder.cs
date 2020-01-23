using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.Builders
{
    public partial class ActivityLogTypeBuilder : BaseEntityBuilder<ActivityLogType>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ActivityLogType.SystemKeyword)).AsString(100).NotNullable()
                .WithColumn(nameof(ActivityLogType.Name)).AsString(200).NotNullable();
        }

        #endregion
    }
}