using FluentMigrator;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Migrations.Localization
{
    [Migration(637097792951964555)]
    public class AddLocaleStringResourceLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(LocaleStringResource))
                .ForeignColumn(nameof(LocaleStringResource.LanguageId))
                .ToTable(nameof(Language))
                .PrimaryColumn(nameof(Language.Id));

            Create.Index().OnTable(nameof(LocaleStringResource)).OnColumn(nameof(LocaleStringResource.LanguageId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}