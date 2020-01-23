using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    /// <summary>
    /// Represents a state and province mapping configuration
    /// </summary>
    public partial class StateProvinceBuilder : BaseEntityBuilder<StateProvince>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StateProvince.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(StateProvince.Abbreviation)).AsString(100).Nullable()
                .WithColumn(nameof(StateProvince.CountryId))
                    .AsInt32()
                    .ForeignKey<Country>();
        }

        #endregion
    }
}