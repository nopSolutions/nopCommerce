using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a item classification list model
/// </summary>
public record ItemClassificationListModel : BasePagedListModel<ItemClassificationModel>
{
}