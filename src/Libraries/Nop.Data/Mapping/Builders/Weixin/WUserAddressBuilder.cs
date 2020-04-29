using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WUserAddressBuilder : NopEntityBuilder<WUserAddress>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WUserAddress.OpenId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WUserAddress.UserName)).AsString(50).NotNullable()
                .WithColumn(nameof(WUserAddress.PostalCode)).AsAnsiString(15).Nullable()
                .WithColumn(nameof(WUserAddress.TelNumber)).AsAnsiString(50).Nullable()
                .WithColumn(nameof(WUserAddress.ProvinceName)).AsString(15).Nullable()
                .WithColumn(nameof(WUserAddress.CityName)).AsString(15).Nullable()
                .WithColumn(nameof(WUserAddress.CountryName)).AsString(15).Nullable()
                .WithColumn(nameof(WUserAddress.DetailInfo)).AsString(255).Nullable()
                .WithColumn(nameof(WUserAddress.NationalCode)).AsAnsiString(15).Nullable()
                ;
        }

        #endregion
    }
}
