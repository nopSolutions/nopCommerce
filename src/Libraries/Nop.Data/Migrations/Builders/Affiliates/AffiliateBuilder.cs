using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public class AffiliateBuilder : BaseEntityBuilder<Affiliate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Affiliate.AddressId))
                    .AsInt32()
                    .ForeignKey<Address>()
                    .OnDelete(Rule.None);
        }

        #endregion
    }
}