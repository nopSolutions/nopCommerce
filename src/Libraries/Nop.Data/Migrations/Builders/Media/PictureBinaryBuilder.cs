using System.Data;
using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PictureBinaryBuilder : BaseEntityBuilder<PictureBinary>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PictureBinary.PictureId))
                .AsInt32()
                .ForeignKey<Picture>()
                .OnDelete(Rule.Cascade);
        }

        #endregion
    }
}