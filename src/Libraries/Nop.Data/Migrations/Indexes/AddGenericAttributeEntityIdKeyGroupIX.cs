using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037686)]
    public class AddGenericAttributeEntityIdKeyGroupIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_GenericAttribute_EntityId_and_KeyGroup", nameof(GenericAttribute), i => i.Ascending(),
                nameof(GenericAttribute.EntityId), nameof(GenericAttribute.KeyGroup));
        }

        #endregion
    }
}