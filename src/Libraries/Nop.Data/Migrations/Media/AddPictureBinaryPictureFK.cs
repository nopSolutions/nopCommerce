using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Media;

namespace Nop.Data.Migrations.Media
{
    [Migration(637097796695631609)]
    public class AddPictureBinaryPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(PictureBinary))
                .ForeignColumn(nameof(PictureBinary.PictureId))
                .ToTable(nameof(Picture))
                .PrimaryColumn(nameof(Picture.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(nameof(PictureBinary)).OnColumn(nameof(PictureBinary.PictureId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}