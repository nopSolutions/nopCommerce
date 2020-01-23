using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class VendorNoteBuilder : BaseEntityBuilder<VendorNote>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorNote.Note)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(VendorNote.VendorId))
                    .AsInt32()
                    .ForeignKey<Vendor>();
        }

        #endregion
    }
}