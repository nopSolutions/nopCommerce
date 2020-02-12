using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037689")]
    public class AddLanguageDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Language_DisplayOrder", nameof(Language), i => i.Ascending(),
                nameof(Language.DisplayOrder));
        }

        #endregion
    }
}