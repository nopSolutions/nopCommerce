using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Builders
{
    public partial class VendorAttributeBuilder : BaseEntityBuilder<VendorAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorAttribute.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}