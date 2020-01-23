using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class LocalizedPropertyBuilder : BaseEntityBuilder<LocalizedProperty>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(LocalizedProperty.LocaleKeyGroup)).AsString(400).NotNullable()
                .WithColumn(nameof(LocalizedProperty.LocaleKey)).AsString(400).NotNullable()
                .WithColumn(nameof(LocalizedProperty.LocaleValue)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(LocalizedProperty.LanguageId))
                    .AsInt32()
                    .ForeignKey<Language>();
        }

        #endregion
    }
}