using FluentMigrator;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647926)]
    public class AddActivityLogCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_ActivityLog_CreatedOnUtc", nameof(ActivityLog), i => i.Descending(),
                nameof(ActivityLog.CreatedOnUtc));
        }

        #endregion
    }
}