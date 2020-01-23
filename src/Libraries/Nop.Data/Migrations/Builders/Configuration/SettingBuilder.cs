using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Configuration;

namespace Nop.Data.Migrations.Builders
{
    public partial class SettingBuilder : BaseEntityBuilder<Setting>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(Setting.Name)).AsString(200).NotNullable()
               .WithColumn(nameof(Setting.Value)).AsString(6000).NotNullable();
        }

        #endregion
    }
}