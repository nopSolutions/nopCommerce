using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Tasks;

namespace Nop.Data.Migrations.Builders
{
    public partial class ScheduleTaskBuilder : BaseEntityBuilder<ScheduleTask>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ScheduleTask.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ScheduleTask.Type)).AsString(int.MaxValue).NotNullable();
        }

        #endregion

    }
}