using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Common
{
    [NopMigration("2019/11/19 02:13:44:0659480")]
    public class AddAddressCountryFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        { 
            this.AddForeignKey(nameof(Address),
                nameof(Address.CountryId),
                nameof(Country),
                nameof(Country.Id));
        }

        #endregion
    }
}