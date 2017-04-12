using System.Collections.Generic;
#if NET451
using System.Web.Mvc;
#endif

namespace Nop.Web.Models.Boards
{
    public partial class SearchModel
    {
        public SearchModel()
        {
#if NET451
            LimitList = new List<SelectListItem>();
            ForumList = new List<SelectListItem>();
            WithinList = new List<SelectListItem>();
#endif
            this.ForumTopics = new List<ForumTopicRowModel>();
        }

        public bool ShowAdvancedSearch { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        public string SearchTerms { get; set; }

        public int? ForumId { get; set; }

        public int? Within { get; set; }

        public int? LimitDays { get; set; }

        public IList<ForumTopicRowModel> ForumTopics { get; set; }
        public int TopicPageSize { get; set; }
        public int TopicTotalRecords { get; set; }
        public int TopicPageIndex { get; set; }

#if NET451
        public List<SelectListItem> LimitList { get; set; }

        public List<SelectListItem> ForumList { get; set; }

        public List<SelectListItem> WithinList { get; set; }
#endif

        public int ForumIdSelected { get; set; }

        public int WithinSelected { get; set; }

        public int LimitDaysSelected { get; set; }

        public bool SearchResultsVisible { get; set; }

        public bool NoResultsVisisble { get; set; }

        public string Error { get; set; }

        public int PostsPageSize { get; set; }

        public bool AllowPostVoting { get; set; }
    }
}