using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Seo;

namespace Nop.Data.Migrations.Builders
{
    public partial class UrlRecordBuilder : BaseEntityBuilder<UrlRecord>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UrlRecord.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(UrlRecord.Slug)).AsString(400).NotNullable();
        }

        #endregion
    }
}