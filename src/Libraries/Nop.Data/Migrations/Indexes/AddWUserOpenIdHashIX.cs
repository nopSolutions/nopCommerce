using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/04/13 09:36:08:9038685")]
    public class AddWUserOpenIdHashIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_WUser_OpenIdHash").OnTable(nameof(WUser))
                .OnColumn(nameof(WUser.OpenIdHash)).Ascending()
                .WithOptions().NonClustered()
                ;
        }

        #endregion
    }
}