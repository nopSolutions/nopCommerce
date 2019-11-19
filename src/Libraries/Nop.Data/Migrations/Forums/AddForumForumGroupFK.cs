using FluentMigrator;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097783627313370)]
    public class AddForumForumGroupFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ForumTable)
                .ForeignColumn(nameof(Forum.ForumGroupId))
                .ToTable(nameof(ForumGroup))
                .PrimaryColumn(nameof(ForumGroup.Id));
        }

        #endregion
    }
}