using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

public partial record NewsItemListModel : BaseNopModel
{
    public int WorkingLanguageId { get; set; }
    public NewsPagingFilteringModel PagingFilteringContext { get; set; } = new();
    public List<NewsItemModel> NewsItems { get; set; } = [];
}