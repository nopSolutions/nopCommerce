using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MvcContrib.Pagination;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class PagingFilteringModel : IPagination
    {
        #region Constructors

        public PagingFilteringModel()
        {
            this.Specs = new List<int>();
            this.PriceRangeFilter = new PriceRangeFilterModel();
        }

        #endregion

        #region Methods

        public IEnumerator GetEnumerator()
        {
            return new List<string>().GetEnumerator();
        }

        public virtual void LoadPagedList<T>(IPagedList<T> pagedList)
        {
            FirstItem = (pagedList.PageIndex * pagedList.PageSize) + 1;
            HasNextPage = pagedList.HasNextPage;
            HasPreviousPage = pagedList.HasPreviousPage;
            LastItem = Math.Min(pagedList.TotalCount, ((pagedList.PageIndex * pagedList.PageSize) + pagedList.PageSize));
            PageNumber = pagedList.PageIndex + 1;
            PageSize = pagedList.PageSize;
            TotalItems = pagedList.TotalCount;
            TotalPages = pagedList.TotalPages;
        }

        #endregion

        #region Properties

        public int CategoryId { get; set; }

        public int ManufacturerId { get; set; }

        /// <summary>
        /// Price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Product sorting
        /// </summary>
        [NopResourceDisplayName("Categories.OrderBy")]
        public int OrderBy { get; set; }

        /// <summary>
        /// Product sorting
        /// </summary>
        [NopResourceDisplayName("Categories.ViewMode")]
        public string ViewMode { get; set; }

        public IList<int> Specs { get; set; }



        public int FirstItem { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public int LastItem { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        #endregion

        #region Nested classes

        public class PriceRangeFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "price";

            #endregion 

            #region Ctor

            public PriceRangeFilterModel()
            {
                this.Items = new List<PriceRangeFilterItem>();
            }

            #endregion

            #region Utilities

            public virtual PriceRange GetSelectedPriceRange(IWebHelper webHelper, string priceRangesStr)
            {
                string range = webHelper.QueryString(QUERYSTRINGPARAM);
                if (String.IsNullOrEmpty(range))
                    return null;
                string[] fromTo = range.Trim().Split(new char[] { '-' });
                if (fromTo.Length == 2)
                {
                    decimal? from = null;
                    if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));
                    decimal? to = null;
                    if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                    var priceRangeList = GetPriceRangeList(priceRangesStr);
                    foreach (var pr in priceRangeList)
                    {
                        if (pr.From == from && pr.To == to)
                            return pr;
                    }
                }
                return null;
            }

            /// <summary>
            /// Gets parsed price ranges
            /// </summary>
            protected virtual IList<PriceRange> GetPriceRangeList(string priceRangesStr)
            {
                var priceRanges = new List<PriceRange>();
                if (string.IsNullOrWhiteSpace(priceRangesStr))
                    return priceRanges;
                string[] rangeArray = priceRangesStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str1 in rangeArray)
                {
                    string[] fromTo = str1.Trim().Split(new char[] { '-' });

                    decimal? from = null;
                    if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));

                    decimal? to = null;
                    if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                    priceRanges.Add(new PriceRange() { From = from, To = to });
                }
                return priceRanges;
            }

            protected virtual string ExcludePriceFilterQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                        url = webHelper.RemoveQueryString(url, exclude);
                }

                return url;
            }

            #endregion

            #region Methods
            
            public virtual void LoadPriceRangeFilters(Category category, IWebHelper webHelper, IPriceFormatter priceFormatter)
            {
                if (category == null)
                    throw new ArgumentNullException("category");

                var priceRangeList = GetPriceRangeList(category.PriceRanges);
                if (priceRangeList.Count > 0)
                {
                    this.Enabled = true;

                    var selectedPriceRange = GetSelectedPriceRange(webHelper, category.PriceRanges);

                    this.Items = priceRangeList.ToList().Select(x =>
                    {
                        //from&to
                        var item = new PriceRangeFilterItem();
                        if (x.From.HasValue)
                            item.From = priceFormatter.FormatPrice(x.From.Value, true, false);
                        if (x.To.HasValue)
                            item.To = priceFormatter.FormatPrice(x.To.Value, true, false);
                        string fromQuery = string.Empty;
                        if (x.From.HasValue)
                            fromQuery = x.From.Value.ToString(new CultureInfo("en-US"));
                        string toQuery = string.Empty;
                        if (x.To.HasValue)
                            toQuery = x.To.Value.ToString(new CultureInfo("en-US"));

                        //is selected?
                        if (selectedPriceRange != null
                            && selectedPriceRange.From == x.From
                            && selectedPriceRange.To == x.To)
                            item.Selected = true;

                        //filter URL
                        string url = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + fromQuery + "-" + toQuery, null);
                        url = ExcludePriceFilterQueryStringParams(url, webHelper);
                        item.FilterUrl = url;


                        return item;
                    }).ToList();

                    if (selectedPriceRange != null)
                    {
                        //remove filter URL
                        string url = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                        url = ExcludePriceFilterQueryStringParams(url, webHelper);
                        this.RemoveFilterUrl = url;
                    }
                }
                else
                {
                    this.Enabled = false;
                }
            }
            
            #endregion

            #region Properties
            public bool Enabled { get; set; }
            public IList<PriceRangeFilterItem> Items { get; set; }
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        public class PriceRangeFilterItem : BaseNopModel
        {
            public string From { get; set; }
            public string To { get; set; }
            public string FilterUrl { get; set; }
            public bool Selected { get; set; }
        }

        #endregion
    }
}