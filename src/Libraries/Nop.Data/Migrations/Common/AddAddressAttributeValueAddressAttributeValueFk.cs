using FluentMigrator;
using Nop.Core.Domain.Common;

namespace Nop.Data.Migrations.Common
{
    /// <summary>
    /// Represents an address attribute value mapping configuration
    /// </summary>
    [Migration(637097693526459118)] public class AddAddressAttributeValueAddressAttributeValueFk : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(AddressAttributeValue))
                .ForeignColumn(nameof(AddressAttributeValue.AddressAttributeId))
                .ToTable(nameof(AddressAttribute))
                .PrimaryColumn(nameof(AddressAttribute.Id));

            Create.Index().OnTable(nameof(AddressAttributeValue)).OnColumn(nameof(AddressAttributeValue.AddressAttributeId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}