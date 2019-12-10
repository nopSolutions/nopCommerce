using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;

namespace Nop.Data.Migrations.Affiliates
{
    [Migration(637097594562551771)]
    public class AddAffiliateAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Affiliate))
                .ForeignColumn(nameof(Affiliate.AddressId))
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id))
                .OnDelete(Rule.None);

            Create.Index().OnTable(nameof(Affiliate)).OnColumn(nameof(Affiliate.AddressId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}