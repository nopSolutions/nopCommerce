using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    public class AbcPromoProductMappingRecordBuilder : NopEntityBuilder<AbcPromoProductMapping>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(AbcPromoProductMapping.AbcPromoId)).AsInt32()
                                                        .ForeignKey<AbcPromo>()
                                                        .OnDelete(Rule.None)
            .WithColumn(nameof(AbcPromoProductMapping.ProductId)).AsInt32()
                                                        .ForeignKey<Product>()
                                                        .OnDelete(Rule.None);
        }
    }
}
