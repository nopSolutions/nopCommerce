using FluentMigrator;
using Nop.Core.Domain.Media;

namespace Nop.Data.Migrations.Media
{
    [Migration(637152095658050000)]
    public class FixPictureBinaryColumnTypes : AutoReversingMigration
    {
        public override void Up()
        {
            IfDatabase("MySQL")
                .Alter.Table(nameof(PictureBinary))
                    .AlterColumn(nameof(PictureBinary.BinaryData))
                    .AsCustom("LONGBLOB")
                    .Nullable();
        }
    }
}