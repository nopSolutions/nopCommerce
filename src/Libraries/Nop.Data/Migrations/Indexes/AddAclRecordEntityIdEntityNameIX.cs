using FluentMigrator;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647929)]
    public class AddAclRecordEntityIdEntityNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_AclRecord_EntityId_EntityName", nameof(AclRecord), i => i.Ascending(),
                nameof(AclRecord.EntityId), nameof(AclRecord.EntityName));
        }

        #endregion
    }
}