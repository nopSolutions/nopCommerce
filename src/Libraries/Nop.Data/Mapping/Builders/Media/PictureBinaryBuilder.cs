using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Media;

/// <summary>
/// Represents a picture binary entity builder
/// </summary>
public partial class PictureBinaryBuilder : NopEntityBuilder<PictureBinary>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(PictureBinary.PictureId)).AsInt32().ForeignKey<Picture>();
    }

    #endregion
}