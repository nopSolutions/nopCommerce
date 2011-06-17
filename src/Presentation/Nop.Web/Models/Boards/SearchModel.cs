using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class SearchModel
    {
        public SearchModel()
        {
            LimitList = new List<SelectListItem>();
            ForumList = new List<SelectListItem>();
            WithinList = new List<SelectListItem>();
        }

        public bool ShowAdvancedSearch { get; set; }

        [AllowHtml]
        public string SearchTerms { get; set; }

        public int? ForumId { get; set; }

        public int? Within { get; set; }

        public int? LimitDays { get; set; }

        public PagedList<ForumTopic> PagedList { get; set; }

        public List<SelectListItem> LimitList { get; set; }

        public List<SelectListItem> ForumList { get; set; }

        public List<SelectListItem> WithinList { get; set; }

        public int ForumIdSelected { get; set; }

        public int WithinSelected { get; set; }

        public int LimitDaysSelected { get; set; }

        public bool SearchResultsVisible { get; set; }

        public bool NoResultsVisisble { get; set; }

        public string Error { get; set; }

        public int PostsPageSize { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }

        public ForumBreadcrumbModel ForumBreadcrumbModel { get; set; }
    }
}