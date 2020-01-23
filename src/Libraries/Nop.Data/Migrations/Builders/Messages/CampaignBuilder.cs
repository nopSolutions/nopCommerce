using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.Builders
{
    public partial class CampaignBuilder : BaseEntityBuilder<Campaign>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Campaign.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Campaign.Subject)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Campaign.Body)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}