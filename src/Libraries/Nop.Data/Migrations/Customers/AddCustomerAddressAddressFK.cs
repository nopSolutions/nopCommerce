using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Customers
{
    [NopMigration("2019/11/19 02:17:39:5245359")]
    public class AddCustomerAddressAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.CustomerAddressesTable,
                "Address_Id",
                nameof(Address),
                nameof(Address.Id),
                Rule.Cascade);
        }

        #endregion
    }
}