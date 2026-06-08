using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record SearchModel : BaseNopModel
{
    #region Properties

    public bool ShowAdvancedSearch { get; set; }
    public string SearchTerms { get; set; }
    public int? ForumId { get; set; }
    public int? Within { get; set; }
    public int? LimitDays { get; set; }
    public int TopicPageSize { get; set; }
    public int TopicTotalRecords { get; set; }
    public int TopicPageIndex { get; set; }
    public int ForumIdSelected { get; set; }
    public int WithinSelected { get; set; }
    public int LimitDaysSelected { get; set; }
    public bool SearchResultsVisible { get; set; }
    public bool NoResultsVisisble { get; set; }
    public string Error { get; set; }
    public int PostsPageSize { get; set; }
    public bool AllowPostVoting { get; set; }

    public List<SelectListItem> ForumList { get; set; } = new();
    public List<ForumTopicRowModel> ForumTopics { get; set; } = new();
    public List<SelectListItem> LimitList { get; set; } = new();
    public List<SelectListItem> WithinList { get; set; } = new();

    #endregion
}