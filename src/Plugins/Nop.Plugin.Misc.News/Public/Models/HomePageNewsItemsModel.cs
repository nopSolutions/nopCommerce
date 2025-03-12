using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

public partial record HomepageNewsItemsModel : BaseNopModel
{
    public int WorkingLanguageId { get; set; }
    public List<NewsItemModel> NewsItems { get; set; } = [];
}