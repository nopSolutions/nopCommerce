using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037708")]
    public class AddCurrencyDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Currency_DisplayOrder", nameof(Currency), i => i.Ascending(), nameof(Currency.DisplayOrder));
        }

        #endregion
    }
}