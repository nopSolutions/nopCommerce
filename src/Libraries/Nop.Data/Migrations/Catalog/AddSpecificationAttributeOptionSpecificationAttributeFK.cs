using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 01:02:16:6619708")]
    public class AddSpecificationAttributeOptionSpecificationAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(SpecificationAttributeOption), 
                nameof(SpecificationAttributeOption.SpecificationAttributeId), 
                nameof(SpecificationAttribute), 
                nameof(SpecificationAttribute.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}