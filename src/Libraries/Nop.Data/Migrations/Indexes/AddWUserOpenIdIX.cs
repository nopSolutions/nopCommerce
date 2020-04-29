using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/04/13 09:36:08:9037685")]
    public class AddWUserOpenIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_WUser_OpenId").OnTable(nameof(WUser))
                .OnColumn(nameof(WUser.OpenId)).Ascending()
                .WithOptions().NonClustered()
                
                ;
        }

        #endregion
    }
}