using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class OfficialCustomerBuilder : NopEntityBuilder<OfficialCustomer>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OfficialCustomer.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(OfficialCustomer.NickName)).AsString(64).Nullable()
                .WithColumn(nameof(OfficialCustomer.HeadImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(OfficialCustomer.QrCodeUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(OfficialCustomer.CategoryIds)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(OfficialCustomer.ContactNumber)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(OfficialCustomer.WorkTime)).AsString(64).Nullable()
                ;
        }

        #endregion
    }
}
