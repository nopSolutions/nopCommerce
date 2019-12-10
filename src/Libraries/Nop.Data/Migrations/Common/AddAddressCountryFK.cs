using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Common
{
    [Migration(637097696240659480)]
    public class AddAddressCountryFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        { 
            Create.ForeignKey().FromTable(nameof(Address))
                .ForeignColumn(nameof(Address.CountryId))
                .ToTable(nameof(Country))
                .PrimaryColumn(nameof(Country.Id));

            Create.Index().OnTable(nameof(Address)).OnColumn(nameof(Address.CountryId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}