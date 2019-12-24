using FluentMigrator;
using Nop.Core.Domain.Seo;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647928)]
    public class AddUrlRecordCustom1IX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_UrlRecord_Custom_1", nameof(UrlRecord), i => i.Ascending(), nameof(UrlRecord.EntityId),
                nameof(UrlRecord.EntityName), nameof(UrlRecord.LanguageId), nameof(UrlRecord.IsActive));
        }

        #endregion
    }
}