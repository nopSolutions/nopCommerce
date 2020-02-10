using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037699")]
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