using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Migrations.Builders
{
    public partial class StoreBuilder : BaseEntityBuilder<Store>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Store.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Store.Url)).AsString(400).NotNullable()
                .WithColumn(nameof(Store.Hosts)).AsString(1000).Nullable()
                .WithColumn(nameof(Store.CompanyName)).AsString(1000).Nullable()
                .WithColumn(nameof(Store.CompanyAddress)).AsString(1000).Nullable()
                .WithColumn(nameof(Store.CompanyPhoneNumber)).AsString(1000).Nullable()
                .WithColumn(nameof(Store.CompanyVat)).AsString(1000).Nullable();
        }

        #endregion
    }
}