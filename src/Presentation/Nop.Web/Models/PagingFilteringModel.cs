using System;
using System.Collections;
using System.Collections.Generic;
using MvcContrib.Pagination;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;

namespace Nop.Web.Models
{
    public class PagingFilteringModel : IPagination
    {
        #region Constructors

        public PagingFilteringModel()
        {
            Specs = new List<int>();
        }

        #endregion

        #region Methods

        public IEnumerator GetEnumerator()
        {
            return new List<string>().GetEnumerator();
        }

        public void LoadPagedList<T>(IPagedList<T> pagedList)
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

        public int FirstItem { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public int LastItem { get; set; }

        public int ManufacturerId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public decimal? PriceMax { get; set; }

        public decimal? PriceMin { get; set; }

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

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        #endregion
    }
}