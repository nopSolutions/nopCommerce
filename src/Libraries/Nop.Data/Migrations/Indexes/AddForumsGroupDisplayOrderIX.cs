using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037698")]
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