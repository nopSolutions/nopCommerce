using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Media
{
    [Migration(637097796695631609)]
    public class AddPictureBinaryPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PictureBinary)
                , nameof(PictureBinary.PictureId)
                , nameof(Picture)
                , nameof(Picture.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}