using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037699)]
    public class AddForumsForumDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Forum_DisplayOrder", NopMappingDefaults.ForumTable, i => i.Ascending(),
                nameof(Forum.DisplayOrder));
        }

        #endregion
    }
}