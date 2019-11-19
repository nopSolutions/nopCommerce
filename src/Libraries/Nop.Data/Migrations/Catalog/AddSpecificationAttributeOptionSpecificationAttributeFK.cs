using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097653366619708)]
    public class AddSpecificationAttributeOptionSpecificationAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(SpecificationAttributeOption))
                .ForeignColumn(nameof(SpecificationAttributeOption.SpecificationAttributeId))
                .ToTable(nameof(SpecificationAttribute))
                .PrimaryColumn(nameof(SpecificationAttribute.Id));
        }

        #endregion
    }
}