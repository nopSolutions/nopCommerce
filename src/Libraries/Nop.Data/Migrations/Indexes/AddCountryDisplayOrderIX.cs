using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037679")]
    public class AddCountryDisplayOrderIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Country_DisplayOrder", nameof(Country), i => i.Ascending(), nameof(Country.DisplayOrder));
        }

        #endregion
    }
}