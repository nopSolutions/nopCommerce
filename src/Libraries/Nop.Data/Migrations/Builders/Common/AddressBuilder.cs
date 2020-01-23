using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class AddressBuilder : BaseEntityBuilder<Address>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Address.CountryId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<Country>()
                .WithColumn(nameof(Address.StateProvinceId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<StateProvince>();
        }

        #endregion
    }
}