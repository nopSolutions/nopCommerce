using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Builders
{
    public partial class CurrencyBuilder : BaseEntityBuilder<Currency>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Currency.Name)).AsString(50).NotNullable()
                .WithColumn(nameof(Currency.CurrencyCode)).AsString(5).NotNullable()
                .WithColumn(nameof(Currency.DisplayLocale)).AsString(50).Nullable()
                .WithColumn(nameof(Currency.CustomFormatting)).AsString(50).Nullable();
        }

        #endregion
    }
}