<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            LimitList = new List<SelectListItem>();
            ForumList = new List<SelectListItem>();
            WithinList = new List<SelectListItem>();
            ForumTopics = new List<ForumTopicRowModel>();
        }

        public bool ShowAdvancedSearch { get; set; }

        public string SearchTerms { get; set; }

        public int? ForumId { get; set; }

        public int? Within { get; set; }

        public int? LimitDays { get; set; }

        public IList<ForumTopicRowModel> ForumTopics { get; set; }
        public int TopicPageSize { get; set; }
        public int TopicTotalRecords { get; set; }
        public int TopicPageIndex { get; set; }
        
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

        public bool AllowPostVoting { get; set; }
    }
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            LimitList = new List<SelectListItem>();
            ForumList = new List<SelectListItem>();
            WithinList = new List<SelectListItem>();
            ForumTopics = new List<ForumTopicRowModel>();
        }

        public bool ShowAdvancedSearch { get; set; }

        public string SearchTerms { get; set; }

        public int? ForumId { get; set; }

        public int? Within { get; set; }

        public int? LimitDays { get; set; }

        public IList<ForumTopicRowModel> ForumTopics { get; set; }
        public int TopicPageSize { get; set; }
        public int TopicTotalRecords { get; set; }
        public int TopicPageIndex { get; set; }
        
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

        public bool AllowPostVoting { get; set; }
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            LimitList = new List<SelectListItem>();
            ForumList = new List<SelectListItem>();
            WithinList = new List<SelectListItem>();
            ForumTopics = new List<ForumTopicRowModel>();
        }

        public bool ShowAdvancedSearch { get; set; }

        public string SearchTerms { get; set; }

        public int? ForumId { get; set; }

        public int? Within { get; set; }

        public int? LimitDays { get; set; }

        public IList<ForumTopicRowModel> ForumTopics { get; set; }
        public int TopicPageSize { get; set; }
        public int TopicTotalRecords { get; set; }
        public int TopicPageIndex { get; set; }
        
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

        public bool AllowPostVoting { get; set; }
    }
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}