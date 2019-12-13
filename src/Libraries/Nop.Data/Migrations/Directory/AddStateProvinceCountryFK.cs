using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Directory
{
    [Migration(637097713433797964)]
    public class AddStateProvinceCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StateProvince)
                , nameof(StateProvince.CountryId)
                , nameof(Country)
                , nameof(Country.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}