using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037686")]
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