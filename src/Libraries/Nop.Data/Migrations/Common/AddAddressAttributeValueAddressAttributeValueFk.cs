using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Common
{
    /// <summary>
    /// Represents an address attribute value mapping configuration
    /// </summary>
    [NopMigration("2019/11/19 02:09:12:6459118")]
    public class AddAddressAttributeValueAddressAttributeValueFk : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(AddressAttributeValue),
                nameof(AddressAttributeValue.AddressAttributeId),
                nameof(AddressAttribute),
                nameof(AddressAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}