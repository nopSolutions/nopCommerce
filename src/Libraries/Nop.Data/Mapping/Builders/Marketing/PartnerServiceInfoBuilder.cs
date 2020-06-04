using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class PartnerServiceInfoBuilder : NopEntityBuilder<PartnerServiceInfo>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PartnerServiceInfo.WUserId)).AsInt32().ForeignKey<WUser>()
                .WithColumn(nameof(PartnerServiceInfo.Name)).AsString(32).NotNullable()
                .WithColumn(nameof(PartnerServiceInfo.SelfNumber)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.ServiceArea)).AsString(1024).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.ServiceContent)).AsString(1024).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.JobTitle)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.Description)).AsString(1024).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.HeadImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.WeChat)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.QrCodeUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.TelNumber)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.Address)).AsString(128).Nullable()
                .WithColumn(nameof(PartnerServiceInfo.Latitude)).AsDecimal(9,6)
                .WithColumn(nameof(PartnerServiceInfo.Longitude)).AsDecimal(9, 6)
                .WithColumn(nameof(PartnerServiceInfo.Precision)).AsDecimal(9, 6)

                ;
        }

        #endregion
    }
}
