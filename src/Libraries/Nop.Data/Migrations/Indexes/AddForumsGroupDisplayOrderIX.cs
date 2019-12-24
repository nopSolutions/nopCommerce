using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037698)]
    public class AddForumsGroupDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Group_DisplayOrder", NopMappingDefaults.ForumsGroupTable, i => i.Ascending(),
                nameof(ForumGroup.DisplayOrder));
        }

        #endregion
    }
}