using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037677)]
    public class AddLocaleStringResourceIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_LocaleStringResource", nameof(LocaleStringResource), i => i.Ascending(),
                nameof(LocaleStringResource.ResourceName), nameof(LocaleStringResource.LanguageId));
        }

        #endregion
    }
}
