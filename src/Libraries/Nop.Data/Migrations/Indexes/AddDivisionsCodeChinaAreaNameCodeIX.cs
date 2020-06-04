using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Marketing;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/04/13 09:36:08:9057686")]
    public class AddDivisionsCodeChinaAreaNameCodeIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_DivisionsCodeChina_AreaNameCode").OnTable(nameof(DivisionsCodeChina))
                .OnColumn(nameof(DivisionsCodeChina.AreaName)).Ascending()
                .OnColumn(nameof(DivisionsCodeChina.AreaCode)).Ascending()
                .WithOptions().NonClustered()
                ;
        }

        #endregion
    }
}