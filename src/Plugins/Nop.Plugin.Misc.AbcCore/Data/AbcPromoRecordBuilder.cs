using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    public class AbcPromoRecordBuilder : NopEntityBuilder<AbcPromo>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
            .WithColumn(nameof(AbcPromo.Name)).AsString()
            .WithColumn(nameof(AbcPromo.Description)).AsString()
            .WithColumn(nameof(AbcPromo.StartDate)).AsDate()
            .WithColumn(nameof(AbcPromo.EndDate)).AsDate()
            .WithColumn(nameof(AbcPromo.ManufacturerId)).AsInt32()
                                                        .Nullable()
                                                        .ForeignKey<Manufacturer>()
                                                        .OnDelete(Rule.SetNull);
        }
    }
}
