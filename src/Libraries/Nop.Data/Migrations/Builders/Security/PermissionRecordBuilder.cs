using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Security;

namespace Nop.Data.Migrations.Builders
{
    public partial class PermissionRecordBuilder : BaseEntityBuilder<PermissionRecord>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PermissionRecord.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(PermissionRecord.SystemName)).AsString(255).NotNullable()
                .WithColumn(nameof(PermissionRecord.Category)).AsString(255).NotNullable();
        }

        #endregion
    }
}