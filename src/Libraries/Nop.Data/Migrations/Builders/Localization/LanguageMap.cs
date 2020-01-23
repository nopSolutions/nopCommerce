using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Migrations.Builders
{
    public partial class LanguageBuilder : BaseEntityBuilder<Language>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Language.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(Language.LanguageCulture)).AsString(20).NotNullable()
                .WithColumn(nameof(Language.UniqueSeoCode)).AsFixedLengthString(2)
                .WithColumn(nameof(Language.FlagImageFileName)).AsString(50).Nullable();
        }

        #endregion
    }
}