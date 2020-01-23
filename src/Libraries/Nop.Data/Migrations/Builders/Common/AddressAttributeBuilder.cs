using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;

namespace Nop.Data.Migrations.Builders
{
    public partial class AddressAttributeBuilder : BaseEntityBuilder<AddressAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(AddressAttribute.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}