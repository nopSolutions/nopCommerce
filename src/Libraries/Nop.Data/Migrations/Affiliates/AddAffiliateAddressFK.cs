using FluentMigrator;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Affiliates
{
    [NopMigration("2019/11/19 11:24:16:2551771")]
    public class AddAffiliateAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Affiliate), 
                nameof(Affiliate.AddressId), 
                nameof(Address), 
                nameof(Address.Id));
        }

        #endregion
    }
}