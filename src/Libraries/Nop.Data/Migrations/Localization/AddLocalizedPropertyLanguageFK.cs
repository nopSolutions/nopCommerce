using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Localization
{
    /// <summary>
    /// Represents a localized property mapping configuration
    /// </summary>
    [NopMigration("2019/11/19 04:55:59:0515436")]
    public class AddLocalizedPropertyLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(LocalizedProperty),
                nameof(LocalizedProperty.LanguageId),
                nameof(Language),
                nameof(Language.Id),
                Rule.Cascade);
        }

        #endregion
    }
}