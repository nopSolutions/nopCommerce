using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:48:30:1910240")]
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