using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class PartnerApplicationFormBuilder : NopEntityBuilder<PartnerApplicationForm>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PartnerApplicationForm.WUserId)).AsInt32().ForeignKey<WUser>()
                .WithColumn(nameof(PartnerApplicationForm.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(PartnerApplicationForm.TelephoneNumber)).AsAnsiString(20).NotNullable()
                .WithColumn(nameof(PartnerApplicationForm.WechatAccount)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerApplicationForm.AlipayAccount)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerApplicationForm.BankName)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerApplicationForm.BankAccount)).AsString(64).Nullable()
                .WithColumn(nameof(PartnerApplicationForm.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
