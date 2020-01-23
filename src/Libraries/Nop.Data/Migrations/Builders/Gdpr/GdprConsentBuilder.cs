using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Gdpr;

namespace Nop.Data.Migrations.Builders
{
    public partial class GdprConsentBuilder : BaseEntityBuilder<GdprConsent>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GdprConsent.Message)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}