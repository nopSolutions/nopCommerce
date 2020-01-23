using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Builders
{
    /// <summary>
    /// Represents a country mapping configuration
    /// </summary>
    public partial class CountryBuilder : BaseEntityBuilder<Country>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Country.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(Country.TwoLetterIsoCode)).AsFixedLengthString(2)
                .WithColumn(nameof(Country.ThreeLetterIsoCode)).AsFixedLengthString(3);
        }

        #endregion
    }
}