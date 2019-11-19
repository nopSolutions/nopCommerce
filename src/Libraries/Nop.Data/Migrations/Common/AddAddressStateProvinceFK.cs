using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Common
{
    [Migration(637097696240659481)]
    public class AddAddressStateProvinceFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Address))
                .ForeignColumn(nameof(Address.StateProvinceId))
                .ToTable(nameof(StateProvince))
                .PrimaryColumn(nameof(StateProvince.Id));
        }

        #endregion
    }
}