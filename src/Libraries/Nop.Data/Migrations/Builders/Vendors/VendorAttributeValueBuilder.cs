using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class VendorAttributeValueBuilder : BaseEntityBuilder<VendorAttributeValue>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(VendorAttributeValue.VendorAttributeId))
                    .AsInt32()
                    .ForeignKey<VendorAttribute>();
        }

        #endregion
    }
}