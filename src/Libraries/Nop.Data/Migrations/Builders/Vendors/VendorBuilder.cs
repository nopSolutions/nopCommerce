using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class VendorBuilder : BaseEntityBuilder<Vendor>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Vendor.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Vendor.Email)).AsString(400).Nullable()
                .WithColumn(nameof(Vendor.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(Vendor.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(Vendor.PageSizeOptions)).AsString(400).Nullable();
        }

        #endregion
    }
}