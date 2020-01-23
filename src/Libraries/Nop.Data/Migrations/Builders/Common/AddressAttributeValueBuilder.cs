using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    /// <summary>
    /// Represents an address attribute value mapping configuration
    /// </summary>
    public partial class AddressAttributeValueBuilder : BaseEntityBuilder<AddressAttributeValue>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AddressAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(AddressAttributeValue.AddressAttributeId))
                    .AsInt32()
                    .ForeignKey<AddressAttribute>();
        }

        #endregion
    }
}