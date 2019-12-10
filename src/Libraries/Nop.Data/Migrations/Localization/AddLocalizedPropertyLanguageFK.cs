using FluentMigrator;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Migrations.Localization
{
    /// <summary>
    /// Represents a localized property mapping configuration
    /// </summary>
    [Migration(637097793590515436)]
    public class AddLocalizedPropertyLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(LocalizedProperty))
                .ForeignColumn(nameof(LocalizedProperty.LanguageId))
                .ToTable(nameof(Language))
                .PrimaryColumn(nameof(Language.Id));

            Create.Index().OnTable(nameof(LocalizedProperty)).OnColumn(nameof(LocalizedProperty.LanguageId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}