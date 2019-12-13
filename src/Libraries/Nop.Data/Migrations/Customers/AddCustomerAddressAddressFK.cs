using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097698595245359)]
    public class AddCustomerAddressAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.CustomerAddressesTable
                , "Address_Id"
                , nameof(Address)
                , nameof(Address.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}