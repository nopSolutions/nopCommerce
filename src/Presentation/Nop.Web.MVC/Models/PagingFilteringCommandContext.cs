using System;
using System.Collections.Generic;
using MvcContrib.Pagination;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.MVC.Models
{
    public class PagingFilteringModel : IPagination
    {
		#region Constructors 

        public PagingFilteringModel()
        {
            Specs = new List<int>();
            ProductSorting = ProductSortingEnum.Position;
        }

		#endregion Constructors 

		#region Properties 

        public int CategoryId {get;set;}

        public int FirstItem
        {
            get;
            set;
        }

        public bool HasNextPage
        {
            get;
            set;
        }

        public bool HasPreviousPage
        {
            get;
            set;
        }

        public int LastItem
        {
            get;
            set;
        }

        public int ManufacturerId {get;set;}

        public int PageNumber
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public decimal? PriceMax {get;set;}

        public decimal? PriceMin {get;set;}

        public ProductSortingEnum ProductSorting {get;set;}

        public IList<int> Specs {get;set;}

        public int TotalItems
        {
            get;
            set;
        }

        public int TotalPages
        {
            get;
            set;
        }

		#endregion Properties 

		#region Methods 

		#region Public Methods 

        public System.Collections.IEnumerator GetEnumerator()
        {
            return new List<string>().GetEnumerator();
        }

        public void LoadPagedList<T>(IPagedList<T> pagedList)
        {
            FirstItem = (pagedList.PageIndex * pagedList.PageSize) + 1;
            HasNextPage = pagedList.HasNextPage;
            HasPreviousPage = pagedList.HasPreviousPage;
            var value1 = ((pagedList.PageIndex*pagedList.PageSize) + pagedList.PageSize);
            LastItem = Math.Min(pagedList.TotalCount, value1);
            PageNumber = pagedList.PageIndex + 1;
            PageSize = pagedList.PageSize;
            TotalItems = pagedList.TotalCount;
            TotalPages = pagedList.TotalPages;
        }

		#endregion Public Methods 

		#endregion Methods 
    }
}