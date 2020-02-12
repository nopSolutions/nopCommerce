using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 12:02:35:9280394")]
    public class AddProductVisibleIndividuallyPublishedDeletedExtendedIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_Product_VisibleIndividually_Published_Deleted_Extended", nameof(Product),
                    i => i.Ascending(), nameof(Product.VisibleIndividually), nameof(Product.Published),
                    nameof(Product.Deleted)).Include(nameof(Product.Id))
                .Include(nameof(Product.AvailableStartDateTimeUtc))
                .Include(nameof(Product.AvailableEndDateTimeUtc));
        }

        #endregion
    }
}