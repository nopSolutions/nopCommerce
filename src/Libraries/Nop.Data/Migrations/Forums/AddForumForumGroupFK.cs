using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097783627313370)]
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