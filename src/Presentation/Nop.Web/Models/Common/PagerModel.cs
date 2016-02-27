using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Models.Common
{
    public partial class PagerModel
    {
        #region Constructors

        public PagerModel()
            : this(EngineContext.Current.Resolve<ILocalizationService>())
        {

        }

        public PagerModel(ILocalizationService localizationService)
        {
            this._localizationService = localizationService;
        }

        #endregion Constructors

        #region Fields

        private readonly ILocalizationService _localizationService;
        private int individualPagesDisplayedCount;
        private int pageIndex = -2;
        private int pageSize;

        private bool? showFirst;
        private bool? showIndividualPages;
        private bool? showLast;
        private bool? showNext;
        private bool? showPagerItems;
        private bool? showPrevious;
        private bool? showTotalSummary;

        private string firstButtonText;
        private string lastButtonText;
        private string nextButtonText;
        private string previousButtonText;
        private string currentPageText;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the current page index
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return (this.PageIndex + 1);
            }
        }

        /// <summary>
        /// Gets or sets a count of individual pages to be displayed
        /// </summary>
        public int IndividualPagesDisplayedCount
        {
            get
            {
                if (individualPagesDisplayedCount <= 0)
                    return 5;
                
                return individualPagesDisplayedCount;
            }
            set
            {
                individualPagesDisplayedCount = value;
            }
        }

        /// <summary>
        /// Gets the current page index
        /// </summary>
        public int PageIndex
        {
            get
            {
                if (this.pageIndex < 0)
                {
                    return 0;
                }
                return this.pageIndex;
            }
            set
            {
                this.pageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        public int PageSize
        {
            get
            {
                return (pageSize <= 0) ? 10 : pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "first"
        /// </summary>
        public bool ShowFirst
        {
            get
            {
                return showFirst ?? true;
            }
            set
            {
                showFirst = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "individual pages"
        /// </summary>
        public bool ShowIndividualPages
        {
            get
            {
                return showIndividualPages ?? true;
            }
            set
            {
                showIndividualPages = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "last"
        /// </summary>
        public bool ShowLast
        {
            get
            {
                return showLast ?? true;
            }
            set
            {
                showLast = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "next"
        /// </summary>
        public bool ShowNext
        {
            get
            {
                return showNext ?? true;
            }
            set
            {
                showNext = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show pager items
        /// </summary>
        public bool ShowPagerItems
        {
            get
            {
                return showPagerItems ?? true;
            }
            set
            {
                showPagerItems = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "previous"
        /// </summary>
        public bool ShowPrevious
        {
            get
            {
                return showPrevious ?? true;
            }
            set
            {
                showPrevious = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "total summary"
        /// </summary>
        public bool ShowTotalSummary
        {
            get
            {
                return showTotalSummary ?? false;
            }
            set
            {
                showTotalSummary = value;
            }
        }

        /// <summary>
        /// Gets a total pages count
        /// </summary>
        public int TotalPages
        {
            get
            {
                if ((this.TotalRecords == 0) || (this.PageSize == 0))
                {
                    return 0;
                }
                int num = this.TotalRecords / this.PageSize;
                if ((this.TotalRecords % this.PageSize) > 0)
                {
                    num++;
                }
                return num;
            }
        }

        /// <summary>
        /// Gets or sets a total records count
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the first button text
        /// </summary>
        public string FirstButtonText
        {
            get
            {
                return (!string.IsNullOrEmpty(firstButtonText)) ?
                    firstButtonText :
                    _localizationService.GetResource("Pager.First");
            }
            set
            {
                firstButtonText = value;
            }
        }

        /// <summary>
        /// Gets or sets the last button text
        /// </summary>
        public string LastButtonText
        {
            get
            {
                return (!string.IsNullOrEmpty(lastButtonText)) ?
                    lastButtonText :
                    _localizationService.GetResource("Pager.Last");
            }
            set
            {
                lastButtonText = value;
            }
        }

        /// <summary>
        /// Gets or sets the next button text
        /// </summary>
        public string NextButtonText
        {
            get
            {
                return (!string.IsNullOrEmpty(nextButtonText)) ?
                    nextButtonText :
                    _localizationService.GetResource("Pager.Next");
            }
            set
            {
                nextButtonText = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous button text
        /// </summary>
        public string PreviousButtonText
        {
            get
            {
                return (!string.IsNullOrEmpty(previousButtonText)) ?
                    previousButtonText :
                    _localizationService.GetResource("Pager.Previous");
            }
            set
            {
                previousButtonText = value;
            }
        }

        /// <summary>
        /// Gets or sets the current page text
        /// </summary>
        public string CurrentPageText
        {
            get
            {
                return (!string.IsNullOrEmpty(currentPageText)) ?
                    currentPageText :
                    _localizationService.GetResource("Pager.CurrentPage");
            }
            set
            {
                currentPageText = value;
            }
        }

        /// <summary>
        /// Gets or sets the route name or action name
        /// </summary>
        public string RouteActionName { get; set; }

        /// <summary>
        /// Gets or sets whether the links are created using RouteLink instead of Action Link 
        /// (for additional route values such as slugs or page numbers)
        /// </summary>
        public bool UseRouteLinks { get; set; }

        /// <summary>
        /// Gets or sets the RouteValues object. Allows for custom route values other than page.
        /// </summary>
        public IRouteValues RouteValues { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets first individual page index
        /// </summary>
        /// <returns>Page index</returns>
        public int GetFirstIndividualPageIndex()
        {
            if ((this.TotalPages < this.IndividualPagesDisplayedCount) ||
                ((this.PageIndex - (this.IndividualPagesDisplayedCount / 2)) < 0))
            {
                return 0;
            }
            if ((this.PageIndex + (this.IndividualPagesDisplayedCount / 2)) >= this.TotalPages)
            {
                return (this.TotalPages - this.IndividualPagesDisplayedCount);
            }
            return (this.PageIndex - (this.IndividualPagesDisplayedCount / 2));
        }

        /// <summary>
        /// Get last individual page index
        /// </summary>
        /// <returns>Page index</returns>
        public int GetLastIndividualPageIndex()
        {
            int num = this.IndividualPagesDisplayedCount / 2;
            if ((this.IndividualPagesDisplayedCount % 2) == 0)
            {
                num--;
            }
            if ((this.TotalPages < this.IndividualPagesDisplayedCount) ||
                ((this.PageIndex + num) >= this.TotalPages))
            {
                return (this.TotalPages - 1);
            }
            if ((this.PageIndex - (this.IndividualPagesDisplayedCount / 2)) < 0)
            {
                return (this.IndividualPagesDisplayedCount - 1);
            }
            return (this.PageIndex + num);
        }

        #endregion Methods
    }

    #region Classes

    /// <summary>
    /// Interface for custom RouteValues objects
    /// </summary>
    public interface IRouteValues
    {
        int page { get; set; }
    }

    /// <summary>
    /// Class that has a slug and page for route values. Used for Topic (posts) and 
    /// Forum (topics) pagination
    /// </summary>
    public partial class RouteValues : IRouteValues
    {
        public int id { get; set; }
        public string slug { get; set; }
        public int page { get; set; }
    }

    /// <summary>
    /// Class that has search options for route values. Used for Search result pagination
    /// </summary>
    public partial class ForumSearchRouteValues : IRouteValues
    {
        public string searchterms { get; set; }
        public string adv { get; set; }
        public string forumId { get; set; }
        public string within { get; set; }
        public string limitDays { get; set; }
        public int page { get; set; }
    }

    /// <summary>
    /// Class that has a slug and page for route values. Used for Private Messages pagination
    /// </summary>
    public partial class PrivateMessageRouteValues : IRouteValues
    {
        public string tab { get; set; }
        public int page { get; set; }
    }

    /// <summary>
    /// Class that has only page for route value. Used for Active Discussions (forums) pagination
    /// </summary>
    public partial class ForumActiveDiscussionsRouteValues : IRouteValues
    {
        public int page { get; set; }
    }

    /// <summary>
    /// Class that has only page for route value. Used for (My Account) Forum Subscriptions pagination
    /// </summary>
    public partial class ForumSubscriptionsRouteValues : IRouteValues
    {        
        public int page { get; set; }
    }

    /// <summary>
    /// Class that has only page for route value. Used for (My Account) Back in stock subscriptions pagination
    /// </summary>
    public partial class BackInStockSubscriptionsRouteValues : IRouteValues
    {
        public int page { get; set; }
    }

    /// <summary>
    /// Class that has only page for route value. Used for (My Account) Reward Points pagination
    /// </summary>
    public partial class RewardPointsRouteValues : IRouteValues
    {
        public int page { get; set; }
    }

    #endregion Classes
}