using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

public record HomepageNewsItemsModel : BaseNopModel
{
    public List<NewsItemModel> NewsItems { get; set; } = [];
}