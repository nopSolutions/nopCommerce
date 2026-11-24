using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Affiliates;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Affiliates;

/// <summary>
/// Represents an affiliate commission entity builder
/// </summary>
public partial class AffiliateCommissionBuilder : NopEntityBuilder<AffiliateCommission>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AffiliateCommission.AffiliateId)).AsInt32().ForeignKey<Affiliate>().OnDelete(Rule.None);
    }

    #endregion
}