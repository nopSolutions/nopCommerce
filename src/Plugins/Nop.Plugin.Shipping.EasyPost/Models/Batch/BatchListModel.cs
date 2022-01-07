using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.EasyPost.Models.Batch
{
    /// <summary>
    /// Represents a batch list model
    /// </summary>
    public record BatchListModel : BasePagedListModel<BatchModel>
    {
    }
}