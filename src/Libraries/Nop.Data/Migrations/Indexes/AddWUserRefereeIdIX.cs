using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/04/13 09:36:08:9037686")]
    public class AddWUserRefereeIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_WUser_RefereeId").OnTable(nameof(WUser))
                .OnColumn(nameof(WUser.RefereeId)).Ascending()
                .OnColumn(nameof(WUser.CreatTime)).Descending()
                .WithOptions().NonClustered()
                ;
        }

        #endregion
    }
}