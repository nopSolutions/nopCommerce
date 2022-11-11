using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Directory
{
    /// <summary>
    /// Represents a state and province entity builder
    /// </summary>
    public partial class StateProvinceBuilder : NopEntityBuilder<StateProvince>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StateProvince.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(StateProvince.Abbreviation)).AsString(100).Nullable()
                .WithColumn(nameof(StateProvince.CountryId)).AsInt32().ForeignKey<Country>();
        }

        #endregion
    }
}