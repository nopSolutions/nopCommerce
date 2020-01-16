using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Localization
{
    [Migration(637097792951964555)]
    public class AddLocaleStringResourceLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(LocaleStringResource),
                nameof(LocaleStringResource.LanguageId),
                nameof(Language),
                nameof(Language.Id),
                Rule.Cascade);
        }

        #endregion
    }
}