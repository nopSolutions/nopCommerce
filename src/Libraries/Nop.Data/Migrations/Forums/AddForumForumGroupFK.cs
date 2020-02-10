using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:39:22:7313370")]
    public class AddForumForumGroupFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumTable,
                nameof(Forum.ForumGroupId),
                NopMappingDefaults.ForumsGroupTable,
                nameof(ForumGroup.Id),
                Rule.Cascade);
        }

        #endregion
    }
}