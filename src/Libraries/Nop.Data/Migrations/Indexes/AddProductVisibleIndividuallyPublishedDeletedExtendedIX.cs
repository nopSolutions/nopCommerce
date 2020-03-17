using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 12:02:35:9280394")]
    public class AddProductVisibleIndividuallyPublishedDeletedExtendedIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            Create.Index("IX_Product_VisibleIndividually_Published_Deleted_Extended").OnTable(nameof(Product))
                .OnColumn(nameof(Product.VisibleIndividually)).Ascending()
                .OnColumn(nameof(Product.Published)).Ascending()
                .OnColumn(nameof(Product.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(Product.Id))
                .Include(nameof(Product.AvailableStartDateTimeUtc))
                .Include(nameof(Product.AvailableEndDateTimeUtc));
        }

        #endregion
    }
}