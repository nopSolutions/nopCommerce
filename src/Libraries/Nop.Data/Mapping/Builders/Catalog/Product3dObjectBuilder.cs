using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a 3D object mapping entity builder
/// </summary>
public partial class Product3dObjectBuilder : NopEntityBuilder<Product3dObject>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Product3dObject.ProductId)).AsInt32().ForeignKey<Product>()
            .WithColumn(nameof(Product3dObject.PreviewPictureId)).AsInt32().Nullable().ForeignKey<Picture>(onDelete: Rule.SetNull);
    }

    #endregion
}