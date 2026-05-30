using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a product model to add for classification
/// </summary>
public record AddProductToClassificationModel : BaseNopModel
{
    #region Ctor

    public AddProductToClassificationModel()
    {
        SelectedProductIds = new List<int>();
    }

    #endregion

    #region Properties

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}