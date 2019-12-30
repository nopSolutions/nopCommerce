using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097789101910240)]
    public class AddForumTopicForumFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsTopicTable,
                nameof(ForumTopic.ForumId),
                NopMappingDefaults.ForumTable,
                nameof(Forum.Id),
                Rule.Cascade);
        }

        #endregion
    }
}