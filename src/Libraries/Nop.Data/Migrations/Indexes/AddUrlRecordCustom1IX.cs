using FluentMigrator;
using Nop.Core.Domain.Seo;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647928")]
    public class AddUrlRecordCustom1IX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_UrlRecord_Custom_1").OnTable(nameof(UrlRecord))
                .OnColumn(nameof(UrlRecord.EntityId)).Ascending()
                .OnColumn(nameof(UrlRecord.EntityName)).Ascending()
                .OnColumn(nameof(UrlRecord.LanguageId)).Ascending()
                .OnColumn(nameof(UrlRecord.IsActive)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}