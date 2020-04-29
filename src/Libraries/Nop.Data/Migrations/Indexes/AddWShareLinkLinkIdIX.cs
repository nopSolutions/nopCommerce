using FluentMigrator;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/04/13 09:36:08:9037684")]
    public class AddWShareLinkLinkIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_WShareLink_LinkId").OnTable(nameof(WShareLink))
                .OnColumn(nameof(WShareLink.LinkId)).Ascending()
                .OnColumn(nameof(WShareLink.Deleted)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}