using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Localization
{
    /// <summary>
    /// Represents a localized property entity builder
    /// </summary>
    public partial class LocalizedPropertyBuilder : NopEntityBuilder<LocalizedProperty>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(LocalizedProperty.LocaleKeyGroupId)).AsInt32()
                .WithColumn(nameof(LocalizedProperty.LocaleKeyId)).AsInt32()
                .WithColumn(nameof(LocalizedProperty.LocaleValue)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(LocalizedProperty.LanguageId)).AsInt32().ForeignKey<Language>();
        }

        #endregion
    }
}