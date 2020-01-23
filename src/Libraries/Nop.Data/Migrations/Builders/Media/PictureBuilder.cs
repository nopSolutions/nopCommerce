using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PictureBuilder : BaseEntityBuilder<Picture>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Picture.MimeType)).AsString(40).NotNullable()
                .WithColumn(nameof(Picture.SeoFilename)).AsString(300).NotNullable();
        }

        #endregion
    }
}