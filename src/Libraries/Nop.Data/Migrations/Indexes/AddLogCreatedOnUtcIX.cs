using FluentMigrator;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037680")]
    public class AddLogCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Log_CreatedOnUtc", nameof(Log), i => i.Descending(), nameof(Log.CreatedOnUtc));
        }

        #endregion
    }
}