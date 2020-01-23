using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public class ManufacturerBuilder : BaseEntityBuilder<Manufacturer>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Manufacturer.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Manufacturer.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Manufacturer.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(Manufacturer.PriceRanges)).AsString(400).Nullable()
                .WithColumn(nameof(Manufacturer.PageSizeOptions)).AsString(400).Nullable();
        }

        #endregion
    }
}