using FluentMigrator;
using Nop.Core.Domain.Seo;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647927)]
    public class AddUrlRecordSlugIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_UrlRecord_Slug", nameof(UrlRecord), i => i.Ascending(), nameof(UrlRecord.Slug));
        }

        #endregion
    }
}