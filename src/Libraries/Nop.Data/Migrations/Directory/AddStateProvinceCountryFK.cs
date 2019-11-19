using FluentMigrator;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Directory
{
    [Migration(637097713433797964)]
    public class AddStateProvinceCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(StateProvince))
                .ForeignColumn(nameof(StateProvince.CountryId))
                .ToTable(nameof(Country))
                .PrimaryColumn(nameof(Country.Id));
        }

        #endregion
    }
}