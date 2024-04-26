using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Builders.Media;

/// <summary>
/// Represents a picture entity builder
/// </summary>
public partial class PictureBuilder : NopEntityBuilder<Picture>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Picture.MimeType)).AsString(40).NotNullable()
            .WithColumn(nameof(Picture.SeoFilename)).AsString(300).Nullable();
    }

    #endregion
}