using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

/// <summary>
/// Represents the homepage news model
/// </summary>
public record HomepageNewsItemsModel : BaseNopModel
{
    #region Properties

    public List<NewsItemModel> NewsItems { get; set; } = [];

    #endregion
}